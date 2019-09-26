using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MainClass
{
    public class BnfMaker
    {
        public List<BnfCollection> BnfCollections { get; }

        public BnfMaker() => BnfCollections = new List<BnfCollection>(); // one line constructor

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

        // match input file with regex
        public bool IsInputFileMatched(string inputFile)
        {
            StreamReader streamReader = new StreamReader(inputFile);
            string readLine = streamReader.ReadLine();
            streamReader.Close();
            if (readLine == null)
                throw new Exception("input file readline is null!");

            string bnfFinalRegex = BnfCollections[0].Regex;
            bnfFinalRegex = "^" + bnfFinalRegex + "$";
            Regex bnfMatchRegex = new Regex(bnfFinalRegex);
            Match inputMatch = bnfMatchRegex.Match(readLine);

            return inputMatch.Success;
        }

        // remove all non-terminal nodes from regex.
        private void RemoveNonTerminlNodes()
        {
            Regex nonTerminalRegex = new Regex(RegexAndPatterns.NonTerminalRegexString);

            for (int i = BnfCollections.Count - 1; i >= 0; i--)
            {
                bool flag = false;
                Match nonTerminalMatch = nonTerminalRegex.Match(BnfCollections[i].Regex);
                while (nonTerminalMatch.Success)
                {
                    if (nonTerminalMatch.Value == BnfCollections[i].Token) // for recursion
                    {
                        BnfCollections[i].Regex = BnfCollections[i].Regex.Replace(nonTerminalMatch.Value, "");
                        BnfCollections[i].Regex = BnfCollections[i].Regex.Trim('|');
                        BnfCollections[i].Regex = "((" + BnfCollections[i].Regex + ") ?)+";
                    }
                    else
                    {
                        foreach (var bnfCollection in BnfCollections)
                        {
                            if (nonTerminalMatch.Value == bnfCollection.Token)
                            {
                                BnfCollections[i].Regex = BnfCollections[i].Regex.Replace(nonTerminalMatch.Value, "(" + bnfCollection.Regex + ")");
                                flag = true;
                            }
                        }
                    }

                    if (flag)
                        nonTerminalMatch = nonTerminalRegex.Match(BnfCollections[i].Regex); // check Replaced Regex for NonTerminal tokens
                    else
                        nonTerminalMatch = nonTerminalMatch.NextMatch(); // else continue search
                }

                BnfCollections[i].Regex = RemoveSpaceAndQuote(BnfCollections[i].Regex);
            }

            foreach (var bnfCollection in BnfCollections)
                bnfCollection.Token = bnfCollection.Token.Trim('<', '>');
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

            foreach (var bnfCollection in BnfCollections)
            {
                Match standardExpressionMatch = tableRegex.Match(bnfCollection.Definition);

                while (standardExpressionMatch.Success)
                {
                    switch (standardExpressionMatch.Value)
                    {
                        case "broj_telefona":
                            bnfCollection.Regex = bnfCollection.Definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.BrojTelefona + ")");
                            break;
                        case "mejl_adresa":
                            bnfCollection.Regex = bnfCollection.Definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.MejlAdresa + ")");
                            break;
                        case "web_link":
                            bnfCollection.Regex = bnfCollection.Definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.WebLink + ")");
                            break;
                        case "brojevna_konstanta":
                            bnfCollection.Regex = bnfCollection.Definition.Replace(standardExpressionMatch.Value, "(" + RegexAndPatterns.BrojevnaKonstanta + ")");
                            break;
                        case "veliki_grad":
                            bnfCollection.Regex = bnfCollection.Definition.Replace(standardExpressionMatch.Value, "(" + cityRegexString + ")");
                            break;

                        default:
                            string regexPattern = standardExpressionMatch.Value.Replace("regex(", "");
                            regexPattern = regexPattern.TrimEnd(')');
                            bnfCollection.Regex = bnfCollection.Definition.Replace(standardExpressionMatch.Value, "(" + regexPattern + ")");
                            break;
                    }

                    standardExpressionMatch = standardExpressionMatch.NextMatch();
                }
            }
        }

        // finding duplicates and combining them
        private void CheckForDuplicates()
        {
            for (int i = 0; i < BnfCollections.Count; i++)
            for (int j = i + 1; j < BnfCollections.Count; j++)
                if (BnfCollections[i].Token == BnfCollections[j].Token)
                {
                    BnfCollections[i].Definition += ("|" + BnfCollections[j].Definition);
                    BnfCollections[i].Regex = BnfCollections[i].Definition;
                    BnfCollections.RemoveAt(j);
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

                    BnfCollections.Add(new BnfCollection(splitLine[0], splitLine[1], splitLine[1]));
                }
                else
                    throw new BnfLineExceptions(lineNumber, readLine);

                lineNumber++;
            }

            configFileReader.Close();
        }
    }
}