namespace MainClass
{
    public class BNFCollection
    {
        public string token;
        public string definition;
        public string regex;

        public BNFCollection()
        { }

        public BNFCollection(string token, string definition, string regex)
        {
            this.token = token;
            this.definition = definition;
            this.regex = regex;
        }

    }
}