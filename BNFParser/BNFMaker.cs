using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MainClass
{
    public class BNFMaker
    {
        public List<BNFCollection> bnfCollections; // make it private

        public BNFMaker() => bnfCollections = new List<BNFCollection>(); // one line constructor

        // reading and making
        public void readConfigFile(string configFile)
        {
            if (!File.Exists(configFile))
                throw new FileNotFoundException("config file path not valid!");

            this.getAllLines(configFile);

            this.checkForDuplicates();

            this.swapTableExpressions();
        }


        // swap table expressions with there regexString and also regex patternt
        private void swapTableExpressions()
        {
            string cityRegexString = RegexAndPatterns.makeCityRegexString(); // mozda try/catch za internet

            string tableExpressionAndRegex = RegexAndPatterns.allTableStandardExpressions + '|' + RegexAndPatterns.regexRegexString;
            Regex tableRegex = new Regex(tableExpressionAndRegex);

            foreach (var bnfCollection in bnfCollections)
            {
                Match standardExpressionMatch = tableRegex.Match(bnfCollection.definition);

                while (standardExpressionMatch.Success)
                {
                    switch (standardExpressionMatch.Value)
                    {
                        case "broj_telefona":
                            bnfCollection.regex = bnfCollection.definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.broj_telefona + ")");
                            break;
                        case "mejl_adresa":
                            bnfCollection.regex = bnfCollection.definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.mejl_adresa + ")");
                            break;
                        case "web_link":
                            bnfCollection.regex = bnfCollection.definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.web_link + ")");
                            break;
                        case "brojevna_konstanta":
                            bnfCollection.regex = bnfCollection.definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.brojevna_konstanta + ")");
                            break;
                        case "veliki_grad":
                            bnfCollection.regex = bnfCollection.definition.Replace(standardExpressionMatch.Value, "(" + cityRegexString + ")");
                            break;
                        
                        default:
                            string regexPattern = standardExpressionMatch.Value.Replace("regex(", "");
                            regexPattern = regexPattern.TrimEnd(')');
                            bnfCollection.regex = bnfCollection.definition.Replace(standardExpressionMatch.Value, "(" + regexPattern + ")");
                            break;
                            
                    }

                    standardExpressionMatch = standardExpressionMatch.NextMatch();
                }
            }
        }

        // finding duplicates and combining them
        private void checkForDuplicates()
        {
            for (int i = 0; i < bnfCollections.Count; i++)
            for (int j = i + 1; j < bnfCollections.Count; j++)
                if (bnfCollections[i].token == bnfCollections[j].token)
                {
                    bnfCollections[i].definition += ("|" + bnfCollections[j].definition);
                    bnfCollections[i].regex = bnfCollections[i].definition;
                    bnfCollections.RemoveAt(j);
                }
        }

        // geting all lines from config file
        private void getAllLines(string configFile)
        {
            StreamReader configFileReader = new StreamReader(configFile);
            string readLine = null;


            Regex bnfLineRegex = new Regex(RegexAndPatterns.BNFLineRegexString, RegexOptions.IgnorePatternWhitespace);
            int lineNumber = 1;
            while ((readLine = configFileReader.ReadLine()) != null)
            {
                if (bnfLineRegex.IsMatch(readLine))
                {
                    string[] splitLine = Regex.Split(readLine, "::=").Select(p => p.Trim()).ToArray();

                    bnfCollections.Add(new BNFCollection(splitLine[0], splitLine[1], splitLine[1]));
                }
                else
                    throw new BNFLineExceptions(lineNumber,readLine);

                lineNumber++;
            }
        }
    }
}