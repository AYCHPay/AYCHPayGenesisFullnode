// Copyright (c) 2020 Louwtjie (Loki) Taljaard a.k.a. HashToBeWild
// Distributed under the MIT software license, see the accompanying
// file COPYING / LICENSE if available or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using HashToBeWild.Utilities.Models;

namespace HashToBeWild.Utilities
{
    /// <summary>
    /// A simple class to generate network seed object, for known dynamic networks
    /// </summary>
    public static class NetworkSeedGenerator
    {
        public static NetworkSeed GenerateSeed(string knownNetworkName)
        {
            // Set "sensible" defaults for mainnet
            NetworkSeed genesisMainnet = new NetworkSeed
            {
                Base58ConfirmationCode = "NTWRK",
                Base58EncryptedSecretKey = 56,
                Base58EncryptedSecretKeyEc = 54,
                Base58PassPhraseCode = "GENESIS",
                BaseTxFee = 10_000,
                CheckPoints = new Dictionary<int, CheckpointInfo>(),
                CoinbaseMaturity = 60,
                CoinTicker = "GNET",
                CoinType = 4637,
                Created = new DateTime(2020, 2, 14, 0, 0, 0, DateTimeKind.Utc),
                DomainName = "genesisnetwork.io",
                GenesisHash = "0x000004911fff23b63c60b60bc35b2c6a0abb97dd221c1f158e0aec62918bc226",
                GenesisMerkle = "0xc15b2714e59aaa54c333f03a44edcef5150fa0e8934e1a87e19dc249bb1bc6e2",
                GenesisNonce = 2265032,
                GenesisTimestampString = "Genesis Network was Created with Love on Valentines Day 2020. Never Forget: Family First. Always.",
                LastPowBlock = 100,
                MajorityWindow = 1000,
                MaxBlockBaseSize = 10_000_000,
                NetworkName = "Genesis Network",
                NetworkType = NetworkType.Mainnet,
                PowTargetSpacing = 60,
                PowTargetTimespan = 120960,
                PremineHeight = 2,
                PremineReward = 500000000,
                ProjectName = "Genesis Network Official",
                ProofOfWorkReward = 1000,
                SeedListDns = new System.Collections.Generic.Dictionary<string, string>(),
                SeedListIp = new System.Collections.Generic.List<string>(),
                SeedNodeCount = 5,
                SubsidyHalvingInterval = 524160, // 60 * 24 * 7 * 4 * 13
                Updated = System.DateTime.Now.ToUniversalTime(),
            };

            // Set testnet changes from mainnet
            NetworkSeed genesisTestnet = genesisMainnet.ShallowCopy();
            genesisTestnet.NetworkType = NetworkType.Testnet;
            genesisTestnet.CoinTicker = "T" + genesisMainnet.CoinTicker;
            genesisTestnet.CoinType = 1;
            genesisTestnet.GenesisNonce = 228307;
            genesisTestnet.GenesisHash = "0x0000cbb2d92858627721fd445a25ebdb9da9dc7a2ffb0213b1b7a9b488112dba";
            genesisTestnet.CoinbaseMaturity = 5;

            // Set regtestnet changes from testnet
            NetworkSeed genesisRegtestnet = genesisTestnet.ShallowCopy();
            genesisRegtestnet.NetworkType = NetworkType.Regtestnet;
            genesisRegtestnet.CoinTicker = "R" + genesisMainnet.CoinTicker;
            genesisRegtestnet.GenesisNonce = 0;
            genesisRegtestnet.GenesisHash = "0x2d2daa04ad98068b34bc382acab077159826fb026d85fb2861f839adf4200301";

            switch (knownNetworkName)
            {
                case "GenesisMainnet":
                    {
                        return genesisMainnet;
                    }

                case "GenesisTestnet":
                    {
                        return genesisTestnet;
                    }

                case "GenesisRegtestnet":
                    {
                        return genesisRegtestnet;
                    }
                default:
                    throw new InvalidOperationException(knownNetworkName + " is not a known network");
            }
        }
    }
}