using System;

namespace MainClass
{
    public class BnfLineExceptions : Exception
    {
        private const string LineError = "Error in line number ";

        public BnfLineExceptions(int lineNum, string line) : base(LineError + lineNum + ". : " + line)
        {
        }
    }
}