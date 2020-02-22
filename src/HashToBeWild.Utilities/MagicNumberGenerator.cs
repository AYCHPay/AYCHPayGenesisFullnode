// Copyright (c) 2020 Louwtjie (Loki) Taljaard a.k.a. HashToBeWild
// Distributed under the MIT software license, see the accompanying
// file COPYING / LICENSE if available or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using System.Linq;

namespace HashToBeWild.Utilities
{
    public class MagicNumberGenerator
    {
        private readonly Random random = new Random();

        public uint FromRandom()
        {
            List<byte> chosen = new List<byte>();
            while (chosen.Count < 4)
            {
                var candidate = (byte)this.random.Next(128, 256);
                if (!chosen.Contains(candidate))
                {
                    chosen.Add(candidate);
                }
            }

            return BitConverter.ToUInt32(chosen.ToArray(), 0);
        }

        public uint FromString(string inputString)
        {
            var chosen = new List<byte>();
            var inputArray = new List<char>(inputString.ToArray());

            // Pad if needed
            while (inputArray.Count < 4)
            {
                inputArray.Add((char)255);
            }

            for (int i = 0; i < 4; i++)
            {
                byte inputItem = (byte)inputArray[i];
                if (inputItem < 128)
                {
                    inputItem = (byte)(inputItem + 128);
                }

                chosen.Add(inputItem);
            }

            return BitConverter.ToUInt32(chosen.ToArray(), 0);
        }
    }
}