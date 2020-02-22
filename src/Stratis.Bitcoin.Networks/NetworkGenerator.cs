// Copyright (c) 2020 Louwtjie (Loki) Taljaard a.k.a. HashToBeWild
// Distributed under the MIT software license, see the accompanying
// file COPYING / LICENSE if available or http://www.opensource.org/licenses/mit-license.php.

namespace Stratis.Bitcoin.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using NBitcoin;
    using NBitcoin.BouncyCastle.Math;
    using NBitcoin.DataEncoders;
    using NBitcoin.Protocol;
    using Stratis.Bitcoin.Features.Consensus.Rules.CommonRules;
    using Stratis.Bitcoin.Features.Consensus.Rules.ProvenHeaderRules;
    using Stratis.Bitcoin.Features.MemoryPool.Rules;
    using Stratis.Bitcoin.Networks.Deployments;
    using Stratis.Bitcoin.Networks.Policies;
    using HashToBeWild.Utilities;
    using HashToBeWild.Utilities.Models;

    public class NetworkGenerator : Network
    {
        private readonly NetworkSeed seed;

        public NetworkGenerator(NetworkSeed seed)
        {
            this.PhoneMnemonicsMap = new PhoneMnemonicsMapper();
            this.AlphabetMap = new AlphabetMapper();
            this.BtcAddressMap = new BtcAddressMapper();
            this.MagicNumberGenerator = new MagicNumberGenerator();
            this.ChecksumGenerator = new ChecksumGenerator();
            this.seed = seed;
            this.Init();
        }

        public AlphabetMapper AlphabetMap { get; set; }

        public BtcAddressMapper BtcAddressMap { get; set; }

        public ChecksumGenerator ChecksumGenerator { get; set; }

        public MagicNumberGenerator MagicNumberGenerator { get; set; }

        public PhoneMnemonicsMapper PhoneMnemonicsMap { get; set; }

        public Target PowLimit { get; set; }

        protected void RegisterMempoolRules(IConsensus consensus)
        {
            consensus.MempoolRules = new List<Type>()
            {
                typeof(CheckConflictsMempoolRule),
                typeof(CheckCoinViewMempoolRule),
                typeof(CreateMempoolEntryMempoolRule),
                typeof(CheckSigOpsMempoolRule),
                typeof(CheckFeeMempoolRule),
                typeof(CheckRateLimitMempoolRule),
                typeof(CheckAncestorsMempoolRule),
                typeof(CheckReplacementMempoolRule),
                typeof(CheckAllInputsMempoolRule),
                typeof(CheckTxOutDustRule)
            };
        }

        protected void RegisterRules(IConsensus consensus)
        {
            consensus.ConsensusRules
                .Register<HeaderTimeChecksRule>()
                .Register<HeaderTimeChecksPosRule>()
                .Register<StratisBugFixPosFutureDriftRule>()
                .Register<CheckDifficultyPosRule>()
                .Register<StratisHeaderVersionRule>()
                .Register<ProvenHeaderSizeRule>()
                .Register<ProvenHeaderCoinstakeRule>();

            consensus.ConsensusRules
                .Register<BlockMerkleRootRule>()
                .Register<PosBlockSignatureRepresentationRule>()
                .Register<PosBlockSignatureRule>();

            consensus.ConsensusRules
                .Register<SetActivationDeploymentsPartialValidationRule>()
                .Register<PosTimeMaskRule>()

                // rules that are inside the method ContextualCheckBlock
                .Register<TransactionLocktimeActivationRule>()
                .Register<CoinbaseHeightActivationRule>()
                .Register<WitnessCommitmentsRule>()
                .Register<BlockSizeRule>()

                // rules that are inside the method CheckBlock
                .Register<EnsureCoinbaseRule>()
                .Register<CheckPowTransactionRule>()
                .Register<CheckPosTransactionRule>()
                .Register<CheckSigOpsRule>()
                .Register<PosCoinstakeRule>();

            consensus.ConsensusRules
                .Register<SetActivationDeploymentsFullValidationRule>()

                .Register<CheckDifficultyHybridRule>()

                // rules that require the store to be loaded (coinview)
                .Register<LoadCoinviewRule>()
                .Register<TransactionDuplicationActivationRule>()
                .Register<PosCoinviewRuleGnet>() // implements BIP68, MaxSigOps and BlockReward calculation
                                             // Place the PosColdStakingRule after the PosCoinviewRule to ensure that all input scripts have been evaluated
                                             // and that the "IsColdCoinStake" flag would have been set by the OP_CHECKCOLDSTAKEVERIFY opcode if applicable.
                .Register<PosColdStakingRule>()
                .Register<SaveCoinviewRule>();
        }

        private void AssignNetworkType()
        {
            switch (this.seed.NetworkType)
            {
                case HashToBeWild.Utilities.Models.NetworkType.Unknown:
                    break;

                case HashToBeWild.Utilities.Models.NetworkType.Mainnet:
                    {
                        this.NetworkType = NBitcoin.NetworkType.Mainnet;
                        this.PowLimit = new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"));
                        this.GenesisBits = 0x1e0fffff;
                    }
                    break;

                case HashToBeWild.Utilities.Models.NetworkType.Testnet:
                    {
                        this.NetworkType = NBitcoin.NetworkType.Testnet;
                        this.PowLimit = new Target(new uint256("0000ffff00000000000000000000000000000000000000000000000000000000"));
                        this.GenesisBits = this.PowLimit;
                    }
                    break;

                case HashToBeWild.Utilities.Models.NetworkType.Regtestnet:
                    {
                        this.NetworkType = NBitcoin.NetworkType.Regtest;
                        this.PowLimit = new Target(new uint256("7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"));
                        this.GenesisBits = this.PowLimit;
                    }
                    break;

                default:
                    break;
            }
        }

        private void BuildBase58Prefixes()
        {
            this.Base58Prefixes = new byte[12][];

            // Use the upper case first char of the ticker
            var base58Lookup_PUBKEY_ADDRESS = this.CoinTicker[0].ToString();
            var lookedup_PUBKEY_ADDRESS = this.BtcAddressMap.Lookup(base58Lookup_PUBKEY_ADDRESS);
            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = lookedup_PUBKEY_ADDRESS;

            // Use the lower case first char of the ticker
            var base58Lookup_SCRIPT_ADDRESS = this.CoinTicker.ToLowerInvariant()[0].ToString();
            var lookedup_SCRIPT_ADDRESS = this.BtcAddressMap.Lookup(base58Lookup_SCRIPT_ADDRESS);
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = lookedup_SCRIPT_ADDRESS;

            // Use the phone mnemonic of the first letter of the ticker
            var base58Lookup_SECRET_KEY = this.PhoneMnemonicsMap.ResolveLong(this.CoinTicker[0].ToString(), 1).ToString();
            var lookedup_SECRET_KEY = this.BtcAddressMap.Lookup(base58Lookup_SECRET_KEY);
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = lookedup_SECRET_KEY;

            // Shared prefix
            var extPrefix = this.AlphabetMap.Lookup(this.CoinTicker[0].ToString());

            var base58Lookup_EXT_PUBLIC_KEY = this.CoinTicker.ToUpperInvariant();
            StringBuilder sb_EXT_PUBLIC_KEY = new StringBuilder(base58Lookup_EXT_PUBLIC_KEY);
            sb_EXT_PUBLIC_KEY[0] = (char)(base58Lookup_EXT_PUBLIC_KEY[0] - 2);
            sb_EXT_PUBLIC_KEY[1] = (char)(base58Lookup_EXT_PUBLIC_KEY[1] - 2);
            base58Lookup_EXT_PUBLIC_KEY = sb_EXT_PUBLIC_KEY.ToString();
            var lookedup_EXT_PUBLIC_KEY = this.BtcAddressMap.Lookup(base58Lookup_EXT_PUBLIC_KEY, 4);
            this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = lookedup_EXT_PUBLIC_KEY;

            string base58Lookup_EXT_SECRET_KEY = this.CoinTicker.ToLowerInvariant();
            StringBuilder sb_EXT_SECRET_KEY = new StringBuilder(base58Lookup_EXT_SECRET_KEY);
            sb_EXT_SECRET_KEY[0] = base58Lookup_EXT_PUBLIC_KEY[0];
            sb_EXT_SECRET_KEY[1] = base58Lookup_EXT_PUBLIC_KEY[1];
            base58Lookup_EXT_SECRET_KEY = sb_EXT_SECRET_KEY.ToString();
            var lookedup_EXT_SECRET_KEY = this.BtcAddressMap.Lookup(base58Lookup_EXT_SECRET_KEY, 4);
            this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = lookedup_EXT_SECRET_KEY;

            // 7 digits
            var base58Lookup_PASSPHRASE_CODE = this.seed.Base58PassPhraseCode;
            var lookedup_PASSPHRASE_CODE = Encoding.ASCII.GetBytes(base58Lookup_PASSPHRASE_CODE);
            this.Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = lookedup_PASSPHRASE_CODE;

            // 5 Digits
            var base58Lookup_CONFIRMATION_CODE = this.seed.Base58ConfirmationCode;
            var lookedup_CONFIRMATION_CODE = Encoding.ASCII.GetBytes(base58Lookup_CONFIRMATION_CODE);
            this.Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = lookedup_CONFIRMATION_CODE;

            // 1 digit up from the first letter of the ticker
            var base58Lookup_STEALTH_ADDRESS = ((char)(base58Lookup_EXT_PUBLIC_KEY[0] + 1)).ToString();
            var lookedup_STEALTH_ADDRESS = this.BtcAddressMap.Lookup(base58Lookup_STEALTH_ADDRESS);
            this.Base58Prefixes[(int)Base58Type.STEALTH_ADDRESS] = lookedup_STEALTH_ADDRESS;

            // 1 digit down from the first letter of the ticker
            var base58Lookup_ASSET_ID = ((char)(base58Lookup_EXT_PUBLIC_KEY[0] - 1)).ToString();
            var lookedup_ASSET_ID = this.BtcAddressMap.Lookup(base58Lookup_ASSET_ID);
            this.Base58Prefixes[(int)Base58Type.ASSET_ID] = lookedup_ASSET_ID;

            var base58Lookup_COLORED_ADDRESS = extPrefix[0].ToString();
            var lookedup_COLORED_ADDRESS = this.BtcAddressMap.Lookup(base58Lookup_COLORED_ADDRESS);
            this.Base58Prefixes[(int)Base58Type.COLORED_ADDRESS] = lookedup_COLORED_ADDRESS;

            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, this.seed.Base58EncryptedSecretKey };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, this.seed.Base58EncryptedSecretKeyEc };
        }

        private void BuildBechEncoders()
        {
            var encoder = new Bech32Encoder(this.CoinTicker.ToLowerInvariant());
            this.Bech32Encoders = new Bech32Encoder[2];
            this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;
        }

        private void BuildCheckpoints()
        {
            this.Checkpoints = new Dictionary<int, NBitcoin.CheckpointInfo>();
            foreach (KeyValuePair<int, HashToBeWild.Utilities.Models.CheckpointInfo> item in this.seed.CheckPoints)
            {
                this.Checkpoints[item.Key] = new NBitcoin.CheckpointInfo(new uint256(item.Value.Hash), new uint256(item.Value.StakeModifierV2));
            }
        }

        private void BuildConsensus(PosConsensusFactory consensusFactory)
        {
            // Taken from StratisX.
            PosConsensusOptions consensusOptions = this.BuildConsensusOptions();

            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 0,
                [BuriedDeployments.BIP65] = 0,
                [BuriedDeployments.BIP66] = 0
            };

            var bip9Deployments = new StratisBIP9Deployments()
            {

                [StratisBIP9Deployments.CSV] = new BIP9DeploymentsParameters(
                    "CSV",
                    0,
                    this.seed.Created.ToUniversalTime(),
                    this.seed.Created.ToUniversalTime(),
                    BIP9DeploymentsParameters.AlwaysActive),

                [StratisBIP9Deployments.Segwit] = new BIP9DeploymentsParameters(
                    "Segwit",
                    1,
                    this.seed.Created.ToUniversalTime(),
                    this.seed.Created.ToUniversalTime(),
                    BIP9DeploymentsParameters.AlwaysActive),

                [StratisBIP9Deployments.ColdStaking] = new BIP9DeploymentsParameters(
                    "ColdStaking",
                    2,
                    this.seed.Created.ToUniversalTime(),
                    this.seed.Created.ToUniversalTime(),
                    BIP9DeploymentsParameters.AlwaysActive),

                [StratisBIP9Deployments.TestDummy] = new BIP9DeploymentsParameters(
                    "TestDummy",
                    28,
                    this.seed.Created.ToUniversalTime(),
                    this.seed.Created.ToUniversalTime(),
                    BIP9DeploymentsParameters.AlwaysActive)

            };

            this.Consensus = new NBitcoin.Consensus(
                consensusFactory: consensusFactory,
                consensusOptions: consensusOptions,
                coinType: this.seed.CoinType,
                hashGenesisBlock: this.Genesis.GetHash(),
                subsidyHalvingInterval: this.seed.SubsidyHalvingInterval,
                majorityWindow: this.seed.MajorityWindow,
                buriedDeployments: buriedDeployments,
                bip9Deployments: bip9Deployments,
                bip34Hash: this.Genesis.GetHash(),
                defaultAssumeValid: this.Genesis.GetHash(),
                maxMoney: long.MaxValue,
                coinbaseMaturity: this.seed.CoinbaseMaturity,
                premineHeight: this.seed.PremineHeight,
                premineReward: Money.Coins(this.seed.PremineReward),
                proofOfWorkReward: Money.Coins(this.seed.ProofOfWorkReward),
                targetTimespan: TimeSpan.FromSeconds(this.seed.PowTargetTimespan),
                targetSpacing: TimeSpan.FromSeconds(this.seed.PowTargetSpacing),
                powAllowMinDifficultyBlocks: false,
                posNoRetargeting: false,
                powNoRetargeting: false,
                powLimit: this.PowLimit,
                minimumChainWork: null,
                isProofOfStake: true,
                lastPowBlock: this.seed.LastPowBlock,
                proofOfStakeLimit: new BigInteger(uint256.Parse("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeLimitV2: new BigInteger(uint256.Parse("000000000000ffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeReward: Money.COIN,
                maxReorgLength: (uint)Math.Round((decimal)(this.seed.MajorityWindow / 2)),
                majorityEnforceBlockUpgrade: (int)Math.Round(this.seed.MajorityWindow * 0.75),
                majorityRejectBlockOutdated: (int)Math.Round(this.seed.MajorityWindow * 0.95),
                minerConfirmationWindow: (int)Math.Round((decimal)(this.seed.PowTargetTimespan / this.seed.PowTargetSpacing))
            );
        }

        private PosConsensusOptions BuildConsensusOptions()
        {
            return new PosConsensusOptions(
                maxBlockBaseSize: this.seed.MaxBlockBaseSize,
                maxStandardVersion: 2,
                maxStandardTxWeight: (int)(this.seed.MaxBlockBaseSize / 10),
                maxBlockSigopsCost: (int)(this.seed.BaseTxFee * 2),
                maxStandardTxSigopsCost: (int)((this.seed.BaseTxFee * 2) / 5),
                witnessScaleFactor: 4
            );
        }

        private void BuildGenesisBlock(PosConsensusFactory consensusFactory)
        {
            this.GenesisTime = (uint)new DateTimeOffset(this.seed.Created).ToUnixTimeSeconds();
            this.GenesisNonce = this.seed.GenesisNonce;
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;
            string genesisCheck = this.seed.GenesisHash;

            this.Genesis = this.CreateGenesisBlock(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);
            if (this.Genesis.GetHash() != uint256.Parse(genesisCheck))
            {
                Console.WriteLine(this.ToString(false));
                Console.WriteLine("Regenerating Genesis Block for " + this.NetworkType);
                this.Genesis = this.SimpleGenesisBlockPow(consensusFactory);
            }
        }

        public string ToString(bool showBase58 = false)
        {
            StringBuilder bob = new StringBuilder();
            bob.AppendLine("this.Name: " + this.Name + ";");
            bob.AppendLine("this.CoinTicker: " + this.CoinTicker + ";");
            bob.AppendLine("this.NetworkType: " + this.NetworkType + ";");
            bob.AppendLine("this.Magic: " + this.Magic + "; // 0x"  + this.Magic.ToString("X"));
            bob.AppendLine("this.DefaultPort: " + this.DefaultPort + ";");
            bob.AppendLine("this.DefaultRPCPort: " + this.DefaultRPCPort + ";");
            bob.AppendLine("this.DefaultAPIPort: " + this.DefaultAPIPort + ";");
            bob.AppendLine("this.DefaultSignalRPort: " + this.DefaultSignalRPort + ";");
            bob.AppendLine("this.DefaultMaxOutboundConnections: " + this.DefaultMaxOutboundConnections + ";");
            bob.AppendLine("this.DefaultMaxInboundConnections: " + this.DefaultMaxInboundConnections + ";");
            bob.AppendLine("this.MaxTipAge: " + this.MaxTipAge + ";");
            bob.AppendLine("this.MinTxFee: " + this.MinTxFee + ";");
            bob.AppendLine("this.FallbackFee: " + this.FallbackFee + ";");
            bob.AppendLine("this.MinRelayTxFee: " + this.MinRelayTxFee + ";");
            bob.AppendLine("this.RootFolderName: " + this.RootFolderName + ";");
            bob.AppendLine("this.DefaultConfigFilename: " + this.DefaultConfigFilename + ";");
            bob.AppendLine("this.MaxTimeOffsetSeconds: " + this.MaxTimeOffsetSeconds + ";");
            bob.AppendLine("this.DefaultBanTimeSeconds: " + this.DefaultBanTimeSeconds + ";");
            bob.AppendLine("this.GenesisTime: " + this.GenesisTime + "; // " + this.seed.Created.ToString("u") );
            bob.AppendLine("this.GenesisNonce: " + this.GenesisNonce + ";");
            bob.AppendLine("this.GenesisBits: " + this.GenesisBits + ";");
            bob.AppendLine("this.GenesisVersion: " + this.GenesisVersion + ";");
            bob.AppendLine("this.GenesisReward: " + this.GenesisReward.ToString() + ";");
            if (showBase58)
            {
                for (int i = 0; i < this.Base58Prefixes.Length; i++)
                {
                    List<string> bytes = new List<string>();
                    foreach (var item in this.Base58Prefixes[i])
                    {
                        bytes.Add("0x" + item.ToString("X2"));
                    }
                    bob.AppendLine("this.Base58Prefixes[" + i + "] = new byte[] {" + string.Join(", ", bytes) + "}; // " + ((Base58Type)i).ToString() );
                }
            }
            //bob.AppendLine("this.: " + this.);
            return bob.ToString();
        }

        private void BuildNetworkSeeds()
        {
            string prefix = this.NetworkType.ToString().Split('.').Last().ToLowerInvariant();

            // DNS Nodes
            this.DNSSeeds = new List<DNSSeedData>();
            if (this.seed.SeedListDns.Count > 0)
            {
                // items are explicitly listed
                foreach (KeyValuePair<string, string> item in this.seed.SeedListDns)
                {
                    this.DNSSeeds.Add(new DNSSeedData(item.Key, item.Value));
                }
            }
            else
            {
                // items are generated
                for (int i = 0; i < this.seed.SeedNodeCount; i++)
                {
                    var nodeName = prefix + i.ToString("D3") + "." + this.seed.DomainName;
                    this.DNSSeeds.Add(new DNSSeedData(nodeName, nodeName));
                }
            }

            // IP Address Nodes
            this.SeedNodes = new List<NetworkAddress>();
            foreach (var item in this.seed.SeedListIp)
            {
                this.SeedNodes.Add(new NetworkAddress(IPAddress.Parse(item), this.DefaultPort));
            }
        }

        private void BuildPortNumbers()
        {
            int baseMultiplier = this.ChecksumGenerator.ByteSum(this.CoinTicker);
            var basePort = this.PhoneMnemonicsMap.ResolveLong(this.CoinTicker[0].ToString(), 1) * baseMultiplier;
            this.DefaultPort = (int)this.PhoneMnemonicsMap.ResolveLong(this.CoinTicker, 4);
            this.DefaultRPCPort = (int)(basePort + this.PhoneMnemonicsMap.ResolveLong(Constants.PORT_NAME_RPC, 3));
            this.DefaultAPIPort = (int)(basePort + this.PhoneMnemonicsMap.ResolveLong(Constants.PORT_NAME_API, 3));
            this.DefaultSignalRPort = (int)(basePort + this.PhoneMnemonicsMap.ResolveLong(Constants.PORT_NAME_SIGNALR, 3));
        }

        private Block CreateGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            string pszTimestamp = this.seed.GenesisTimestampString;

            Transaction txNew = consensusFactory.CreateTransaction();
            txNew.Version = 1;
            if (txNew is IPosTransactionWithTime posTx)
            {
                posTx.Time = nTime;
            }
            txNew.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(0), new Op()
                {
                    Code = (OpcodeType)0x1,
                    PushData = new[] { (byte)42 }
                }, Op.GetPushOp(Encoders.ASCII.DecodeData(pszTimestamp)))
            });
            txNew.AddOutput(new TxOut()
            {
                Value = genesisReward,
            });

            Block genesis = consensusFactory.CreateBlock();
            genesis.Header.BlockTime = Utils.UnixTimeToDateTime(nTime);
            genesis.Header.Bits = nBits;
            genesis.Header.Nonce = nNonce;
            genesis.Header.Version = nVersion;
            genesis.Transactions.Add(txNew);
            genesis.Header.HashPrevBlock = uint256.Zero;
            genesis.UpdateMerkleRoot();
            return genesis;
        }

        private void Init()
        {
            // Get direct values
            this.CoinTicker = this.seed.CoinTicker.ToUpperInvariant();
            this.AssignNetworkType();
            string suffix = this.NetworkType.ToString().Split('.').Last();
            this.Name = this.seed.NetworkName.Replace(" ", string.Empty) + suffix;

            this.Magic = this.MagicNumberGenerator.FromString(this.CoinTicker);

            var consensusFactory = new PosConsensusFactory();

            this.BuildPortNumbers();
            this.BuildGenesisBlock(consensusFactory);
            this.BuildConsensus(consensusFactory);
            this.BuildBase58Prefixes();
            this.BuildCheckpoints();
            this.BuildBechEncoders();
            this.BuildNetworkSeeds();

            // Basic Connectivity
            this.DefaultMaxInboundConnections = 16;
            this.DefaultMaxOutboundConnections = this.DefaultMaxInboundConnections * 8;

            // Fees
            this.MinTxFee = this.seed.BaseTxFee;
            this.FallbackFee = this.MinTxFee;
            this.MinRelayTxFee = this.MinTxFee;

            // Storage
            this.RootFolderName = this.seed.CoinTicker.ToLowerInvariant();
            this.DefaultConfigFilename = this.RootFolderName + ".conf";

            // Time thresholds
            this.MaxTipAge = this.seed.PowTargetSpacing * 144; // same ratio as bitcoin
            this.MaxTimeOffsetSeconds = this.seed.PowTargetSpacing * 7; // same ratio as bitcoin
            this.DefaultBanTimeSeconds = (int)(this.Consensus.MaxReorgLength * this.Consensus.TargetSpacing.TotalSeconds) / 2;

            // Standard Scripts
            this.StandardScriptsRegistry = new StratisStandardScriptsRegistry();

            // Sanity checks
            this.SanityCheck();

            // Register rules
            this.RegisterRules(this.Consensus);
            this.RegisterMempoolRules(this.Consensus);

            // Show the generated settings...
            Console.WriteLine(this.ToString(true));
        }

        private void SanityCheck()
        {
            // General Checks to make sure things are as the are expected to be
            Assert(this.Consensus.HashGenesisBlock == uint256.Parse(this.seed.GenesisHash));
            Assert(this.Genesis.Header.HashMerkleRoot == uint256.Parse(this.seed.GenesisMerkle));
        }

        private Block SimpleGenesisBlockPow(PosConsensusFactory consensusFactory)
        {
            // We need to recheck the genesis block
            this.Genesis = this.CreateGenesisBlock(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);
            while (this.Genesis.GetHash() > this.PowLimit.ToUInt256())
            {
                this.GenesisNonce++;
                this.Genesis = this.CreateGenesisBlock(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);
            }
            
            Console.WriteLine("Genesis Block Hash: " + this.Genesis.GetHash());
            Console.WriteLine("Genesis Block Merkel Root: " + this.Genesis.GetMerkleRoot().Hash);
            Console.WriteLine("Genesis Block Nonce: " + this.Genesis.Header.Nonce);
            return this.Genesis;
        }
    }
}