using System;
using System.Collections.Generic;

namespace MainClass
{
    public class BnfCollection
    {
        public string Token { get; set; } // left side of expression
        public string Definition { get; set; } // config file token definition
        public string Regex { get; set; }

        public BnfCollection(string token, string definition, string regex)
        {
            this.Token = token;
            this.Definition = definition;
            this.Regex = regex;
        }

        public override string ToString()
        {
            return "token: " + Token + '\n' + "definition: " + Definition + '\n' + "regex: " + Regex;
        }
    }
}