using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Consensus.Rules;
using Stratis.Bitcoin.Utilities;

namespace Stratis.Bitcoin.Features.Consensus.Rules.CommonRules
{
    /// <summary>
    /// Calculate the difficulty for a POS block and check that it is correct.
    /// This rule is only activated after the POW epoch is finished according to the value in <see cref="Consensus.LastPOWBlock"/>.
    /// </summary>
    public class CheckDifficultyPosRuleGnet : HeaderValidationConsensusRule
    {
        /// <summary>Allow access to the POS parent.</summary>
        protected PosConsensusRuleEngine PosParent;

        /// <inheritdoc />
        public override void Initialize()
        {
            this.PosParent = this.Parent as PosConsensusRuleEngine;

            Guard.NotNull(this.PosParent, nameof(this.PosParent));
        }

        /// <inheritdoc />
        /// <exception cref="ConsensusErrors.BadDiffBits">Thrown if proof of stake is incorrect.</exception>
        public override void Run(RuleContext context)
        {
            if (this.Parent.Network.Consensus.PosNoRetargeting)
            {
                this.Logger.LogTrace("(-)[POS_NO_RETARGETING]");
                return;
            }

            // TODO: In the future once we migrated to fully C# network it might be good to consider signaling in the block header the network type.

            ChainedHeader chainedHeader = context.ValidationContext.ChainedHeaderToValidate;
            var waypoint = this.Parent.Network.Consensus.LastPOWBlock + (int)System.Math.Ceiling(this.Parent.Network.Consensus.TargetSpacing.TotalSeconds * 1.5d);
            if (chainedHeader.Height >= waypoint)
            {
                // I can hardcode the true value for the calculate retarget, because we're inside an if that already enforces that.
                Target nextWorkRequired = this.PosParent.StakeValidator.CalculateRetarget(chainedHeader, true);

                BlockHeader header = context.ValidationContext.ChainedHeaderToValidate.Header;

                // Check proof of stake.
                if (header.Bits.Difficulty < nextWorkRequired.Difficulty)
                {
                    if (chainedHeader.Height != waypoint || (chainedHeader.Height == waypoint && header.Bits != new Target(this.Parent.Network.Consensus.ProofOfStakeLimit)))
                    {
                        this.Logger.LogTrace("(-)[BAD_DIFF_BITS]");
                        ConsensusErrors.BadDiffBits.Throw();
                    }
                }
            }
        }
    }
}