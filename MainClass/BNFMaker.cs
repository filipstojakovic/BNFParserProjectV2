using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MainClass
{
    public class BNFMaker
    {
        private List<BNFCollection> bnfCollections;

        public BNFMaker() => bnfCollections = new List<BNFCollection>(); // one line constructor

        public void readConfigFile(string configFile)
        {
            if (!File.Exists(configFile))
                throw new FileNotFoundException("config file path not valid!");

            this.getAllLines(configFile);
        }

        private void getAllLines(string configFile)
        {
            StreamReader configFileReader = new StreamReader(configFile);
            string readLine = null;


//            Regex bnfLineRegex = new Regex();
            int lineNumber = 1;
            while ((readLine = configFileReader.ReadLine()) != null)
            {

                // OVO SAD RAIDS!! takodje checkDuplicates

            }
        }
    }
}