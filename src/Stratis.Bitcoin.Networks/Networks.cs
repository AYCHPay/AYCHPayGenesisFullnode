using NBitcoin;
using HashToBeWild.Utilities;

namespace Stratis.Bitcoin.Networks
{
    public static class Networks
    {
        public static NetworksSelector Bitcoin
        {
            get
            {
                return new NetworksSelector(() => new BitcoinMain(), () => new BitcoinTest(), () => new BitcoinRegTest());
            }
        }

        public static NetworksSelector Stratis
        {
            get
            {
                return new NetworksSelector(() => new StratisMain(), () => new StratisTest(), () => new StratisRegTest());
            }
        }

        public static NetworksSelector Genesis
        {
            get
            {
                var mainNet = new NetworkGenerator(NetworkSeedGenerator.GenerateSeed("GenesisMainnet"));
                var testNet = new NetworkGenerator(NetworkSeedGenerator.GenerateSeed("GenesisTestnet"));
                var regtestNet = new NetworkGenerator(NetworkSeedGenerator.GenerateSeed("GenesisRegtestnet"));
                return new NetworksSelector(() => mainNet, () => testNet, () => regtestNet);
            }
        }

    }
}
