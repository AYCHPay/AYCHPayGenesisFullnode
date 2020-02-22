// Copyright (c) 2020 Louwtjie (Loki) Taljaard a.k.a. HashToBeWild
// Distributed under the MIT software license, see the accompanying
// file COPYING / LICENSE if available or http://www.opensource.org/licenses/mit-license.php.

using System.Collections.Generic;
using System.Linq;

namespace HashToBeWild.Utilities
{
    public class AlphabetMapper
    {
        public AlphabetMapper()
        {
            this.Map = new Dictionary<int, string>();
            Init();
        }

        public Dictionary<int, string> Map { get; set; }

        public byte[] Lookup(string input)
        {
            List<byte> output = new List<byte>();
            foreach (var item in input.ToCharArray())
            {
                int lookupResult = -1;
                lookupResult = this.Map.Where(x => x.Value == item.ToString()).FirstOrDefault().Key;
                if (lookupResult > -1)
                {
                    output.Add((byte)lookupResult);
                }
            }
            return output.ToArray();
        }

        private void Init()
        {
            this.Map[0] = "A";
            this.Map[1] = "B";
            this.Map[2] = "C";
            this.Map[3] = "D";
            this.Map[4] = "E";
            this.Map[5] = "F";
            this.Map[6] = "G";
            this.Map[7] = "H";
            this.Map[8] = "I";
            this.Map[9] = "J";
            this.Map[10] = "K";
            this.Map[11] = "L";
            this.Map[12] = "M";
            this.Map[13] = "N";
            this.Map[14] = "O";
            this.Map[15] = "P";
            this.Map[16] = "Q";
            this.Map[17] = "R";
            this.Map[18] = "S";
            this.Map[19] = "T";
            this.Map[20] = "U";
            this.Map[21] = "V";
            this.Map[22] = "W";
            this.Map[23] = "X";
            this.Map[24] = "Y";
            this.Map[25] = "Z";
        }
    }
}