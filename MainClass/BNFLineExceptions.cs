using System;

namespace MainClass
{
    public class BNFLineExceptions : Exception
    {
        public const string lineError = "Error in line number ";

        public BNFLineExceptions(int lineNum, string line) : base(lineError + lineNum + ". : " + line)
        {
        }
    }
}