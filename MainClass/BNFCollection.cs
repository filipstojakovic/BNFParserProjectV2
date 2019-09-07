using System;

namespace MainClass
{
    public class BNFCollection
    {
        public string token;        // left side of expression
        public string definition;    // config file definition
        public string regex;

        public BNFCollection()
        { }

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