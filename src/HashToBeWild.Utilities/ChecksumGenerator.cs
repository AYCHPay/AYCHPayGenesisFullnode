using System;
using System.Collections.Generic;
using System.Text;

namespace HashToBeWild.Utilities
{
    public class ChecksumGenerator
    {
        public int ByteSum(string input)
        {
            int output = 0;
            var bytes = Encoding.ASCII.GetBytes(input);
            foreach (var item in bytes)
            {
                output += item;
            }
            return output;
        }
    }
}
