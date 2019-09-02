using System;

namespace MainClass
{
    public class BNFLineExceptions : Exception
    {
        public const string lineError = "Error while parsing line num: ";

        public BNFLineExceptions(int lineNum) : base(lineError+lineNum)
        {
        }


    }
}