// Copyright (c) 2020 Louwtjie (Loki) Taljaard a.k.a. HashToBeWild
// Distributed under the MIT software license, see the accompanying
// file COPYING / LICENSE if available or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using SwarmPower.Utilities.Models;

namespace SwarmPower.Utilities
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
                Created = new DateTime(2020, 2, 14),
                DomainName = "genesisnetwork.io",
                GenesisHash = "0x00000a894c64302f8ba3d69da0837eeecb982b4e80b77c1d63bfa8f5d09abac0",
                GenesisMerkle = "0x56a33d28d432786c44ac2b617688c5ba87f568fed658b8dcb392f332c3d01042",
                GenesisNonce = 1477007,
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
                ProofOfWorkReward = 100,
                SeedListDns = new System.Collections.Generic.Dictionary<string, string>(),
                SeedListIp = new System.Collections.Generic.List<string>(),
                SeedNodeCount = 5,
                SubsidyHalvingInterval = 524160, // 60 * 24 * 7 * 4 * 13
                Updated = System.DateTime.Now,
            };

            // Set testnet changes from mainnet
            NetworkSeed genesisTestnet = genesisMainnet.ShallowCopy();
            genesisTestnet.NetworkType = NetworkType.Testnet;
            genesisTestnet.CoinTicker = "T" + genesisMainnet.CoinTicker;
            genesisTestnet.CoinType = 1;
            genesisTestnet.GenesisNonce = 53585;
            genesisTestnet.GenesisHash = "0x00008058705674dd363f3b82f2f36f0c09950e7f793669e0c3b0eec271f4ee5c";
            genesisTestnet.GenesisMerkle = "0x56a33d28d432786c44ac2b617688c5ba87f568fed658b8dcb392f332c3d01042";

            // Set regtestnet changes from testnet
            NetworkSeed genesisRegtestnet = genesisTestnet.ShallowCopy();
            genesisRegtestnet.NetworkType = NetworkType.Regtestnet;
            genesisRegtestnet.CoinTicker = "R" + genesisMainnet.CoinTicker;
            genesisRegtestnet.GenesisNonce = 0;
            genesisRegtestnet.GenesisHash = "0x67bbdb0c41cca41b286c5c0bbbfe06175f1693ec6611b04fae6dccace7ad1662";
            genesisRegtestnet.GenesisMerkle = "0x56a33d28d432786c44ac2b617688c5ba87f568fed658b8dcb392f332c3d01042";

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