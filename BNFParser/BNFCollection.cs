using System;
using System.Collections.Generic;

namespace MainClass
{
    public class BNFCollection
    {
        public string token { get; set; }        // left side of expression
        public string definition { get; set; }    // config file token definition
        public string regex { get; set; }       

//        public BNFCollection()
//        { }

        public BNFCollection(string token, string definition, string regex)
        {
            this.token = token;
            this.definition = definition;
            this.regex = regex;
        }

        public override string ToString()
        {
            return "token: "+token + '\n' + "definition: "+definition + '\n' + "regex: "+regex;
            
        }

    }
}