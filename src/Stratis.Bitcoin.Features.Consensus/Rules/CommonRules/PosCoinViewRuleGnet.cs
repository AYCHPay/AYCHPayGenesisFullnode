using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Consensus.Rules;
using Stratis.Bitcoin.Features.Consensus.Interfaces;
using Stratis.Bitcoin.Utilities;

namespace Stratis.Bitcoin.Features.Consensus.Rules.CommonRules
{
    /// <summary>
    /// Proof of stake override for the coinview rules - BIP68, MaxSigOps and BlockReward checks.
    /// </summary>
    public sealed class PosCoinviewRuleGnet : CoinViewRule
    {
        /// <summary>Provides functionality for checking validity of PoS blocks.</summary>
        private IStakeValidator stakeValidator;

        /// <summary>Database of stake related data for the current blockchain.</summary>
        private IStakeChain stakeChain;

        /// <summary>The consensus of the parent Network.</summary>
        private IConsensus consensus;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            this.consensus = this.Parent.Network.Consensus;
            var consensusRules = (PosConsensusRuleEngine)this.Parent;

            this.stakeValidator = consensusRules.StakeValidator;
            this.stakeChain = consensusRules.StakeChain;
        }

        /// <inheritdoc />
        /// <summary>Compute and store the stake proofs.</summary>
        public override async Task RunAsync(RuleContext context)
        {
            this.CheckAndComputeStake(context);

            await base.RunAsync(context).ConfigureAwait(false);
            var posRuleContext = context as PosRuleContext;
            this.stakeChain.Set(context.ValidationContext.ChainedHeaderToValidate, posRuleContext.BlockStake);
        }

        /// <inheritdoc />
        public override void CheckBlockReward(RuleContext context, Money fees, int height, Block block)
        {
            if (BlockStake.IsProofOfStake(block))
            {
                var posRuleContext = context as PosRuleContext;
                uint stakeTxHeight = posRuleContext.UnspentOutputSet.AccessCoins(posRuleContext.CoinStakePrevOutputs.First().Key.PrevOut).Coins.Height;
                Money stakeReward = block.Transactions[1].TotalOut - posRuleContext.TotalCoinStakeValueIn;
                Money calcStakeReward = fees + this.GetProofOfStakeReward(height, posRuleContext.TotalCoinStakeValueIn, stakeTxHeight);

                this.Logger.LogDebug("Block stake reward is {0}, calculated reward is {1}.", stakeReward, calcStakeReward);
                if (stakeReward > calcStakeReward)
                {
                    this.Logger.LogTrace("(-)[BAD_COINSTAKE_AMOUNT]");
                    ConsensusErrors.BadCoinstakeAmount.Throw();
                }
            }
            else
            {
                Money blockReward = fees + this.GetProofOfWorkReward(height);
                this.Logger.LogDebug("Block reward is {0}, calculated reward is {1}.", block.Transactions[0].TotalOut, blockReward);
                if (block.Transactions[0].TotalOut > blockReward)
                {
                    this.Logger.LogTrace("(-)[BAD_COINBASE_AMOUNT]");
                    ConsensusErrors.BadCoinbaseAmount.Throw();
                }
            }
        }

        protected override Money GetTransactionFee(UnspentOutputSet view, Transaction tx)
        {
            return tx.IsCoinStake ? Money.Zero : view.GetValueIn(tx) - tx.TotalOut;
        }

        /// <inheritdoc />
        public override void UpdateCoinView(RuleContext context, Transaction transaction)
        {
            var posRuleContext = context as PosRuleContext;

            UnspentOutputSet view = posRuleContext.UnspentOutputSet;

            if (transaction.IsCoinStake)
            {
                posRuleContext.TotalCoinStakeValueIn = view.GetValueIn(transaction);
                posRuleContext.CoinStakePrevOutputs = transaction.Inputs.ToDictionary(txin => txin, txin => view.GetOutputFor(txin));
            }

            base.UpdateUTXOSet(context, transaction);
        }

        /// <inheritdoc />
        public override void CheckMaturity(UnspentOutput coins, int spendHeight)
        {
            base.CheckCoinbaseMaturity(coins, spendHeight);

            if (coins.Coins.IsCoinstake)
            {
                if ((spendHeight - coins.Coins.Height) < this.consensus.CoinbaseMaturity)
                {
                    this.Logger.LogDebug("Coinstake transaction height {0} spent at height {1}, but maturity is set to {2}.", coins.Coins.Height, spendHeight, this.consensus.CoinbaseMaturity);
                    this.Logger.LogTrace("(-)[COINSTAKE_PREMATURE_SPENDING]");
                    ConsensusErrors.BadTransactionPrematureCoinstakeSpending.Throw();
                }
            }
        }

        /// <inheritdoc />
        protected override void CheckInputValidity(Transaction transaction, UnspentOutput coins)
        {
            // TODO: Keep this check to avoid a network split
            if (transaction is IPosTransactionWithTime posTrx)
            {
                // Transaction timestamp earlier than input transaction - main.cpp, CTransaction::ConnectInputs
                if (coins.Coins.Time > posTrx.Time)
                    ConsensusErrors.BadTransactionEarlyTimestamp.Throw();
            }
        }

        /// <summary>
        /// Checks and computes stake.
        /// </summary>
        /// <param name="context">Context that contains variety of information regarding blocks validation and execution.</param>
        /// <exception cref="ConsensusErrors.PrevStakeNull">Thrown if previous stake is not found.</exception>
        /// <exception cref="ConsensusErrors.SetStakeEntropyBitFailed">Thrown if failed to set stake entropy bit.</exception>
        private void CheckAndComputeStake(RuleContext context)
        {
            ChainedHeader chainedHeader = context.ValidationContext.ChainedHeaderToValidate;
            Block block = context.ValidationContext.BlockToValidate;

            var posRuleContext = context as PosRuleContext;
            if (posRuleContext.BlockStake == null)
                posRuleContext.BlockStake = BlockStake.Load(context.ValidationContext.BlockToValidate);

            BlockStake blockStake = posRuleContext.BlockStake;

            // Verify hash target and signature of coinstake tx.
            if (BlockStake.IsProofOfStake(block))
            {
                ChainedHeader prevChainedHeader = chainedHeader.Previous;

                BlockStake prevBlockStake = this.stakeChain.Get(prevChainedHeader.HashBlock);
                if (prevBlockStake == null)
                    ConsensusErrors.PrevStakeNull.Throw();

                // Only do proof of stake validation for blocks that are after the assumevalid block or after the last checkpoint.
                if (!context.SkipValidation)
                {
                    this.stakeValidator.CheckProofOfStake(posRuleContext, prevChainedHeader, prevBlockStake, block.Transactions[1], chainedHeader.Header.Bits.ToCompact());
                }
                else this.Logger.LogDebug("POS validation skipped for block at height {0}.", chainedHeader.Height);
            }

            // PoW is checked in CheckBlock().
            if (BlockStake.IsProofOfWork(block))
                posRuleContext.HashProofOfStake = chainedHeader.Header.GetPoWHash();

            // Compute stake entropy bit for stake modifier.
            if (!blockStake.SetStakeEntropyBit(blockStake.GetStakeEntropyBit()))
            {
                this.Logger.LogTrace("(-)[STAKE_ENTROPY_BIT_FAIL]");
                ConsensusErrors.SetStakeEntropyBitFailed.Throw();
            }

            // Record proof hash value.
            blockStake.HashProof = posRuleContext.HashProofOfStake;

            int lastCheckpointHeight = this.Parent.Checkpoints.LastCheckpointHeight;
            if (chainedHeader.Height > lastCheckpointHeight)
            {
                // Compute stake modifier.
                ChainedHeader prevChainedHeader = chainedHeader.Previous;
                BlockStake blockStakePrev = prevChainedHeader == null ? null : this.stakeChain.Get(prevChainedHeader.HashBlock);
                blockStake.StakeModifierV2 = this.stakeValidator.ComputeStakeModifierV2(prevChainedHeader, blockStakePrev?.StakeModifierV2, blockStake.IsProofOfWork() ? chainedHeader.HashBlock : blockStake.PrevoutStake.Hash);
            }
            else if (chainedHeader.Height == lastCheckpointHeight)
            {
                // Copy checkpointed stake modifier.
                CheckpointInfo checkpoint = this.Parent.Checkpoints.GetCheckpoint(lastCheckpointHeight);
                blockStake.StakeModifierV2 = checkpoint.StakeModifierV2;
                this.Logger.LogDebug("Last checkpoint stake modifier V2 loaded: '{0}'.", blockStake.StakeModifierV2);
            }
            else this.Logger.LogDebug("POS stake modifier computation skipped for block at height {0} because it is not above last checkpoint block height {1}.", chainedHeader.Height, lastCheckpointHeight);
        }

        /// <inheritdoc />
        public override Money GetProofOfWorkReward(int height)
        {
            if (this.IsPremine(height))
                return this.consensus.PremineReward;

            return this.consensus.ProofOfWorkReward;
        }

        /// <summary>
        /// Gets the annual stake for the given amount.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="chainYear">The chain year.</param>
        /// <returns>The reward amount for a full year of staking</returns>
        private Money GetAnnualStake(Money amount, decimal chainYear)
        {
            Money output;
            // case values are expressed in number of years completed, so it is 0 based.
            switch (chainYear)
            {
                case 0:
                case 1:
                    {
                        // 12% 
                        output = (amount / 100) * 12;
                    }
                    break;
                case 2:
                    {
                        // 10% 
                        output = (amount / 100) * 10;
                    }
                    break;
                case 3:
                    {
                        // 8% 
                        output = (amount / 100) * 8;
                    }
                    break;
                case 4:
                    {
                        // 6% 
                        output = (amount / 100) * 6;
                    }
                    break;
                case 5:
                    {
                        // 4% 
                        output = (amount / 100) * 4;
                    }
                    break;
                case 6:
                default:
                    {
                        // 2% for-eva-eva
                        output = (amount / 100) * 2;
                    }
                    break;
            }
            return output;
        }

        /// <summary>
        /// Gets miner's coin stake reward.
        /// </summary>
        /// <param name="height">Target block height. As decimal to reduce explicit casting.</param>
        /// <param name="totalCoinStakeValueIn">The value of the stake.</param>
        /// <param name="stakeTxHeight">The height of the tx that is being staked</param>
        /// <returns>Miner's coin stake reward.</returns>
        public Money GetProofOfStakeReward(decimal height, Money totalCoinStakeValueIn, uint stakeTxHeight)
        {
            if (this.IsPremine((int)height))
                return this.consensus.PremineReward;

            // localize the variable
            decimal subsidyHalvingInterval = this.consensus.SubsidyHalvingInterval;

            // Calculate the chain age
            decimal completedChainYears;

            // Is halving in effect yet?
            if (height >= subsidyHalvingInterval)
            {
                // which phase / year are we in?
                completedChainYears = System.Math.Floor(height / subsidyHalvingInterval);
            }
            else
            {
                // we have not completed the first year yet...
                completedChainYears = 0;
            }
            // Calculate the day of the current year
            decimal blockOfCurrentChainYear = height % subsidyHalvingInterval;
            // calculate the percentage of the current year 
            decimal percentOfCurrentChainYear = (blockOfCurrentChainYear / subsidyHalvingInterval) * 100;

            // Calculate the age of the stake
            // subtract the staked tx height from the current height
            var stakedTxAge = height - stakeTxHeight;
            decimal completedStakeYears;

            if (stakedTxAge >= subsidyHalvingInterval)
            {
                // how far back do we need to go?
                completedStakeYears = System.Math.Floor(stakedTxAge / subsidyHalvingInterval);
            }
            else
            {
                completedStakeYears = 0;
            }
            decimal blockOfCurrentStakeYear = stakedTxAge % subsidyHalvingInterval;
            decimal percentOfCurrentStakeYear = (blockOfCurrentStakeYear / subsidyHalvingInterval) * 100;

            // Now that we know which year / phase both the chain and the stake are in, 
            // we can calculate and validate the accrual correctly

            // First, some basic sanity checks
            if (blockOfCurrentStakeYear > blockOfCurrentChainYear || percentOfCurrentStakeYear > percentOfCurrentChainYear)
            {
                this.Logger.LogDebug("Coinstake transaction at height {0} is at current year block height {1} ({2}%), but chain current year block height is at {3}({4}%).", height, blockOfCurrentStakeYear, percentOfCurrentStakeYear, blockOfCurrentChainYear, percentOfCurrentChainYear);
                this.Logger.LogTrace("(-)[COINSTAKE_CURRENT_YEAR_GREATER_THAN_CHAIN_YEAR]");
                ConsensusErrors.BadTransactionCoinstakeYearOlderThanChainYear.Throw();
            }

            if (completedStakeYears > completedChainYears)
            {
                this.Logger.LogDebug("Coinstake transaction at height {0} has completed {1} stake year(s), but chain has only completed {2} year(s).", height, completedStakeYears, completedChainYears);
                this.Logger.LogTrace("(-)[COINSTAKE_COMPLETED_YEAR_GREATER_THAN_CHAIN_COMPLETED_YEAR]");
                ConsensusErrors.BadTransactionCoinstakeCompletedMoreYearsThanChain.Throw();
            }

            // the simplest is that the stake is exclusively in the current chain year 
            if (completedStakeYears == 0)
            {
                // What would be the value of a complete year worth of staking?
                Money annualAmount = GetAnnualStake(totalCoinStakeValueIn, completedChainYears);
                // Return the appropriate fraction of this year
                return Money.Coins((long)annualAmount * percentOfCurrentStakeYear);
            }
            // slightly more interesting is when the stake span includes part of the previous chain year
            // the most interesting is when the stake spans multiple chain years, but they are handled the same
            else
            {
                // See how far back we need to go...
                var firstYear = completedChainYears - completedStakeYears;
                Money accruedAmount = Money.Zero;
                // For each staking year compound the accrual
                for (int i = (int)firstYear; i <= completedChainYears; i++)
                {
                    accruedAmount += GetAnnualStake(totalCoinStakeValueIn + accruedAmount, i);
                }
                // Add the current year's bit
                accruedAmount += Money.Coins((long)(accruedAmount + totalCoinStakeValueIn) * percentOfCurrentStakeYear);
                // Return the value
                return accruedAmount;
            }
        }
    }
}
