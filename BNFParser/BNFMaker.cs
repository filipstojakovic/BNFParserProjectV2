using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MainClass
{
    public class BnfMaker
    {
        public List<BNFCollection> bnfCollections { get; }

        public BnfMaker() => bnfCollections = new List<BNFCollection>(); // one line constructor

        // reading and making
        public void ReadConfigFile(string configFile)
        {
            if (!File.Exists(configFile))
                throw new FileNotFoundException("config file path not valid!");

            this.GetAllLines(configFile);

            this.CheckForDuplicates();

            this.SwapTableExpressions();

            this.RemoveNonTerminlNodes();
        }

        // remove all non-terminal nodes from regex.
        private void RemoveNonTerminlNodes()
        {
            Regex nonTerminalRegex = new Regex(RegexAndPatterns.NonTerminalRegexString);

            for (int i = bnfCollections.Count-1; i>=0; i--) // backwards is better
            {
                bool flag = false;
                Match nonTerminalMatch = nonTerminalRegex.Match(bnfCollections[i].regex);
                while (nonTerminalMatch.Success)
                {
                    if (nonTerminalMatch.Value == bnfCollections[i].token)
                    {
                        bnfCollections[i].regex = bnfCollections[i].regex.Replace(nonTerminalMatch.Value, "");
                        bnfCollections[i].regex = "(" + bnfCollections[i].regex + " ?)+";
                    }
                    else
                    {
                        foreach (var bnfCollection in bnfCollections)
                        {
                            if (nonTerminalMatch.Value == bnfCollection.token)
                            {
                                bnfCollections[i].regex = bnfCollections[i].regex.Replace(nonTerminalMatch.Value, "(" + bnfCollection.regex + ")");
                                flag = true;
                            }
                        }
                    }

                    if (flag)
                        nonTerminalMatch = nonTerminalRegex.Match(bnfCollections[i].regex); // u slucaju da postoji jos ne terminalnih cvorova 
                    else
                        nonTerminalMatch = nonTerminalMatch.NextMatch();
                }

                bnfCollections[i].regex = RemoveSpaceAndQuote(bnfCollections[i].regex);
            }

            foreach (var bnfCollection in bnfCollections)
                bnfCollection.token = bnfCollection.token.Trim('<', '>');
        }

        //remove spaces and quotes
        private string RemoveSpaceAndQuote(string str)
        {
            str = str.Replace(")+ ", ")+");
            str = str.Replace("\"", "");
            str = str.Replace(" |", "|");
            str = str.Replace("| ", "|");
            return str;
        }


        // swap table expressions with there regexString and also regex patternt
        private void SwapTableExpressions()
        {
            string cityRegexString = RegexAndPatterns.MakeCityRegexString(); // mozda try/catch za internet

            string tableExpressionAndRegex = RegexAndPatterns.AllTableStandardExpressions + '|' + RegexAndPatterns.RegexRegexString;
            Regex tableRegex = new Regex(tableExpressionAndRegex);

            foreach (var bnfCollection in bnfCollections)
            {
                Match standardExpressionMatch = tableRegex.Match(bnfCollection.definition);

                while (standardExpressionMatch.Success)
                {
                    switch (standardExpressionMatch.Value)
                    {
                        case "broj_telefona":
                            bnfCollection.regex = bnfCollection.definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.BrojTelefona + ")");
                            break;
                        case "mejl_adresa":
                            bnfCollection.regex = bnfCollection.definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.MejlAdresa + ")");
                            break;
                        case "web_link":
                            bnfCollection.regex = bnfCollection.definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.WebLink + ")");
                            break;
                        case "brojevna_konstanta":
                            bnfCollection.regex = bnfCollection.definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.BrojevnaKonstanta + ")");
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
        private void CheckForDuplicates()
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
        private void GetAllLines(string configFile)
        {
            StreamReader configFileReader = new StreamReader(configFile);
            string readLine = null;

            Regex bnfLineRegex = new Regex(RegexAndPatterns.BnfLineRegexString, RegexOptions.IgnorePatternWhitespace);
            int lineNumber = 1;

            while ((readLine = configFileReader.ReadLine()) != null)
            {
                if (bnfLineRegex.IsMatch(readLine))
                {
                    string[] splitLine = Regex.Split(readLine, "::=").Select(p => p.Trim()).ToArray();

                    bnfCollections.Add(new BNFCollection(splitLine[0], splitLine[1], splitLine[1]));
                }
                else
                    throw new BnfLineExceptions(lineNumber, readLine);

                lineNumber++;
            }

            configFileReader.Close();
        }
    }
}