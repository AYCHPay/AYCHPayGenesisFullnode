// Copyright (c) 2020 Louwtjie (Loki) Taljaard a.k.a. HashToBeWild
// Distributed under the MIT software license, see the accompanying
// file COPYING / LICENSE if available or http://www.opensource.org/licenses/mit-license.php.

using System.Collections.Generic;
using System.Linq;
using HashToBeWild.Utilities.Models;

namespace HashToBeWild.Utilities
{
    // based on: https://en.bitcoin.it/wiki/List_of_address_prefixes
    public class BtcAddressMapper
    {
        public Dictionary<int, BtcAddressCharItem> Map { get; set; }

        public BtcAddressMapper()
        {
            this.Map = new Dictionary<int, BtcAddressCharItem>();
            Init();
        }

        public byte[] Lookup(string input, int lengthLimit = 0)
        {
            List<byte> output = new List<byte>();
            var realLimit = lengthLimit > 0 && lengthLimit < input.Length ? lengthLimit : input.Length;
            for (int i = 0; i < realLimit; i++)
            {
                var ca = input.ToCharArray();
                var item = ca[i];
                int lookupResult = -1;
                lookupResult = this.Map.Where(x => x.Value.Symbols.Contains(item) && x.Value.Symbols.Count == 1).FirstOrDefault().Key;
                if (lookupResult > -1)
                {
                    output.Add((byte)lookupResult);
                }
            }
            return output.ToArray();
        }

        private void Init()
        {
            this.Map[0] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '1' }
            };

            this.Map[1] = new BtcAddressCharItem()
            {
                AddressLength = 33,
                Symbols = new List<char> { 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'o' }
            };

            this.Map[2] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '2' }
            };

            this.Map[3] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '2' }
            };

            this.Map[4] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '2', '3' }
            };

            this.Map[5] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '3' }
            };

            this.Map[6] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '3' }
            };

            this.Map[7] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '3', '4' }
            };

            this.Map[8] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '4' }
            };

            this.Map[9] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '4', '5' }
            };

            this.Map[10] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '5' }
            };

            this.Map[11] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '5' }
            };

            this.Map[12] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '5', '6' }
            };

            this.Map[13] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '6' }
            };

            this.Map[14] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '6', '7' }
            };

            this.Map[15] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '7' }
            };

            this.Map[16] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '7' }
            };

            this.Map[17] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '7', '8' }
            };

            this.Map[18] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '8' }
            };

            this.Map[19] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '8', '9' }
            };

            this.Map[20] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '9' }
            };

            this.Map[21] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '9' }
            };

            this.Map[22] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { '9', 'A' }
            };

            this.Map[23] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'A' }
            };

            this.Map[24] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'A', 'B' }
            };

            this.Map[25] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'B' }
            };

            this.Map[26] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'B' }
            };

            this.Map[27] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'B', 'C' }
            };

            this.Map[28] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'C' }
            };

            this.Map[29] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'C', 'D' }
            };

            this.Map[30] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'D' }
            };

            this.Map[31] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'D' }
            };

            this.Map[32] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'D', 'E' }
            };

            this.Map[33] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'E' }
            };

            this.Map[34] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'E', 'F' }
            };

            this.Map[35] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'F' }
            };

            this.Map[36] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'F' }
            };

            this.Map[37] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'F', 'G' }
            };

            this.Map[38] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'G' }
            };

            this.Map[39] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'G', 'H' }
            };

            this.Map[40] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'H' }
            };

            this.Map[41] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'H' }
            };

            this.Map[42] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'H', 'J' }
            };

            this.Map[43] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'J' }
            };

            this.Map[44] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'J', 'K' }
            };

            this.Map[45] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'K' }
            };

            this.Map[46] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'K' }
            };

            this.Map[47] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'K', 'L' }
            };

            this.Map[48] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'L' }
            };

            this.Map[49] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'L', 'M' }
            };

            this.Map[50] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'M' }
            };

            this.Map[51] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'M' }
            };

            this.Map[52] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'M', 'N' }
            };

            this.Map[53] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'N' }
            };

            this.Map[54] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'N', 'P' }
            };

            this.Map[55] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'P' }
            };

            this.Map[56] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'P' }
            };

            this.Map[57] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'P', 'Q' }
            };

            this.Map[58] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'Q' }
            };

            this.Map[59] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'Q', 'R' }
            };

            this.Map[60] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'R' }
            };

            this.Map[61] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'R' }
            };

            this.Map[62] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'R', 'S' }
            };

            this.Map[63] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'S' }
            };

            this.Map[64] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'S', 'T' }
            };

            this.Map[65] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'T' }
            };

            this.Map[66] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'T' }
            };

            this.Map[67] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'T', 'U' }
            };

            this.Map[68] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'U' }
            };

            this.Map[69] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'U', 'V' }
            };

            this.Map[70] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'V' }
            };

            this.Map[71] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'V' }
            };

            this.Map[72] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'V', 'W' }
            };

            this.Map[73] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'W' }
            };

            this.Map[74] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'W', 'X' }
            };

            this.Map[75] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'X' }
            };

            this.Map[76] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'X' }
            };

            this.Map[77] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'X', 'Y' }
            };

            this.Map[78] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'Y' }
            };

            this.Map[79] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'Y', 'Z' }
            };

            this.Map[80] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'Z' }
            };

            this.Map[81] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'Z' }
            };

            this.Map[82] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'Z', 'a' }
            };

            this.Map[83] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'a' }
            };

            this.Map[84] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'a', 'b' }
            };

            this.Map[85] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'b' }
            };

            this.Map[86] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'b', 'c' }
            };

            this.Map[87] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'c' }
            };

            this.Map[88] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'c' }
            };

            this.Map[89] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'c', 'd' }
            };

            this.Map[90] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'd' }
            };

            this.Map[91] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'd', 'e' }
            };

            this.Map[92] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'e' }
            };

            this.Map[93] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'e' }
            };

            this.Map[94] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'e', 'f' }
            };

            this.Map[95] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'f' }
            };

            this.Map[96] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'f', 'g' }
            };

            this.Map[97] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'g' }
            };

            this.Map[98] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'g' }
            };

            this.Map[99] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'g', 'h' }
            };

            this.Map[100] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'h' }
            };

            this.Map[101] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'h', 'i' }
            };

            this.Map[102] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'i' }
            };

            this.Map[103] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'i' }
            };

            this.Map[104] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'i', 'j' }
            };

            this.Map[105] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'j' }
            };

            this.Map[106] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'j', 'k' }
            };

            this.Map[107] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'k' }
            };

            this.Map[108] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'k' }
            };

            this.Map[109] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'k', 'm' }
            };

            this.Map[110] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'm' }
            };

            this.Map[111] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'm', 'n' }
            };

            this.Map[112] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'n' }
            };

            this.Map[113] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'n' }
            };

            this.Map[114] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'n', 'o' }
            };

            this.Map[115] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'o' }
            };

            this.Map[116] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'o', 'p' }
            };

            this.Map[117] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'p' }
            };

            this.Map[118] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'p' }
            };

            this.Map[119] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'p', 'q' }
            };

            this.Map[120] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'q' }
            };

            this.Map[121] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'q', 'r' }
            };

            this.Map[122] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'r' }
            };

            this.Map[123] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'r' }
            };

            this.Map[124] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'r', 's' }
            };

            this.Map[125] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 's' }
            };

            this.Map[126] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 's', 't' }
            };

            this.Map[127] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 't' }
            };

            this.Map[128] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 't' }
            };

            this.Map[129] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 't', 'u' }
            };

            this.Map[130] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'u' }
            };

            this.Map[131] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'u', 'v' }
            };

            this.Map[132] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'v' }
            };

            this.Map[133] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'v' }
            };

            this.Map[134] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'v', 'w' }
            };

            this.Map[135] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'w' }
            };

            this.Map[136] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'w', 'x' }
            };

            this.Map[137] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'x' }
            };

            this.Map[138] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'x' }
            };

            this.Map[139] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'x', 'y' }
            };

            this.Map[140] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'y' }
            };

            this.Map[141] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'y', 'z' }
            };

            this.Map[142] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'z' }
            };

            this.Map[143] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'z' }
            };

            this.Map[144] = new BtcAddressCharItem()
            {
                AddressLength = 34,
                Symbols = new List<char> { 'z', '2' }
            };

            for (int i = 145; i < 256; i++)
            {
                this.Map[i] = new BtcAddressCharItem()
                {
                    AddressLength = 34,
                    Symbols = new List<char> { '2' }
                };
            }
        }
    }
}