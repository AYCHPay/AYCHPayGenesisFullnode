using System;
using System.Collections.Generic;

namespace SwarmPower.Utilities.Models
{
    public class NetworkSeed
    {
        public NetworkSeed()
        {

        }

        public NetworkSeed ShallowCopy()
        {
            return (NetworkSeed)this.MemberwiseClone();
        }

        public string Base58ConfirmationCode { get; set; }
        public byte Base58EncryptedSecretKey { get; set; }
        public byte Base58EncryptedSecretKeyEc { get; set; }
        public string Base58PassPhraseCode { get; set; }
        public long BaseTxFee { get; set; }
        public Dictionary<int, CheckpointInfo> CheckPoints { get; set; }
        public long CoinbaseMaturity { get; set; }
        public string CoinTicker { get; set; }
        public int CoinType { get; set; } = 105;
        public DateTime Created { get; set; }
        public string DomainName { get; set; }
        public string GenesisHash { get; set; }
        public string GenesisMerkle { get; set; }
        public uint GenesisNonce { get; set; }
        public string GenesisTimestampString { get; set; }
        public long Id { get; set; }
        public int LastPowBlock { get; set; }
        public int MajorityWindow { get; set; }
        public uint MaxBlockBaseSize { get; set; }
        public string NetworkName { get; set; }
        public NetworkType NetworkType { get; set; }
        public int PowTargetSpacing { get; set; }
        public int PowTargetTimespan { get; set; }
        public long PremineHeight { get; set; } = 2;
        public long PremineReward { get; set; }
        public string ProjectName { get; set; }
        public long ProofOfWorkReward { get; set; }
        public Dictionary<string, string> SeedListDns { get; set; }
        public List<string> SeedListIp { get; set; }
        public int SeedNodeCount { get; set; }
        public int SubsidyHalvingInterval { get; set; }
        public DateTime Updated { get; set; }
    }
}