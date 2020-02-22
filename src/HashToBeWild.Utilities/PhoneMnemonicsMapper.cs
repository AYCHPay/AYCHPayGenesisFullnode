// Copyright (c) 2020 Louwtjie (Loki) Taljaard a.k.a. HashToBeWild
// Distributed under the MIT software license, see the accompanying
// file COPYING / LICENSE if available or http://www.opensource.org/licenses/mit-license.php.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashToBeWild.Utilities
{
    public class PhoneMnemonicsMapper
    {
        public PhoneMnemonicsMapper()
        {
            this.Layout = new Dictionary<string, List<string>>();
            Init();
        }

        public Dictionary<string, List<string>> Layout { get; set; }

        public long ResolveLong(string inputString, int lengthLimit)
        {
            var tempest = new string(ResolveString(inputString, lengthLimit).Where(c => char.IsDigit(c)).ToArray());
            long output = 0;
            long.TryParse(tempest, out output);
            return output;
        }

        public string ResolveString(string inputString, int lengthLimit)
        {
            StringBuilder output = new StringBuilder();
            var realLimit = lengthLimit < inputString.Length ? lengthLimit : inputString.Length;
            for (int i = 0; i < realLimit; i++)
            {
                output.Append(this.Layout.FirstOrDefault(x => x.Value.Contains(inputString[i].ToString())).Key);
            }
            return output.ToString();
        }

        private void Init()
        {
            this.Layout["1"] = new List<string>() { "1" };
            this.Layout["2"] = new List<string>() { "2", "A", "B", "C" };
            this.Layout["3"] = new List<string>() { "3", "D", "E", "F" };
            this.Layout["4"] = new List<string>() { "4", "G", "H", "I" };
            this.Layout["5"] = new List<string>() { "5", "J", "K", "L" };
            this.Layout["6"] = new List<string>() { "6", "M", "N", "O" };
            this.Layout["7"] = new List<string>() { "7", "P", "Q", "R", "S" };
            this.Layout["8"] = new List<string>() { "8", "T", "U", "V" };
            this.Layout["9"] = new List<string>() { "9", "W", "X", "Y", "Z" };
            this.Layout["0"] = new List<string>() { "0", "+" };
            this.Layout["#"] = new List<string>() { "#" };
            this.Layout["*"] = new List<string>() { "*" };
        }
    }
}