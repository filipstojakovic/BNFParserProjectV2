using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;


namespace MainClass
{
    public class XMLCreator
    {
        public XmlWriter XmlWriter { get; set; }
        private XmlWriterSettings settings;

        public XMLCreator()
        {
            settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineOnAttributes = true;
        }


        // creating the XML file
        public void createXML(string inputPath, string outputPath, List<BNFCollection> bnfCollections)
        {
            StreamReader streamReader = new StreamReader(inputPath);
            string readLine = streamReader.ReadLine();
            streamReader.Close();
            if(readLine==null)
                throw new Exception("input file readline is null!");

            string bnfFinalRegex = bnfCollections[0].regex;
            bnfFinalRegex = "^" + bnfFinalRegex + "$";
            Regex bnfMatchRegex = new Regex(bnfFinalRegex);
            Match inputMatch = bnfMatchRegex.Match(readLine);

            XmlWriter = XmlWriter.Create(outputPath, settings);
            if (inputMatch.Success)
            {
                Console.WriteLine("Matched Successful");
                List<string> matchedToken = new string[bnfCollections.Count].Select(x => "").ToList();
//                List<string> matchedToken = new List<string>(); //
//                for (int i = 0; i < bnfCollections.Count; i++)
//                {
//                    matchedToken.Add("");
//                }
                
                matchedToken.Add(inputMatch.Value);

                XmlWriter.WriteStartDocument();
                XmlWriter.WriteStartElement(bnfCollections[0].token); // insert root


                for (int i = 0; i < bnfCollections.Count; i++)
                {
                    Regex nonTerminalRegex = new Regex(RegexAndPatterns.nonTerminalRegexString);
                    Match nonTerminalMatch = nonTerminalRegex.Match(bnfCollections[i].definition);

                    while (nonTerminalMatch.Success)
                    {
                        string nonTerminalValue = nonTerminalMatch.Value.Trim('<', '>');
                        for (int j = 0; j < bnfCollections.Count; j++)
                        {
                            if (bnfCollections[j].token == nonTerminalValue)
                            {
                                Recursion(bnfCollections, j, i, matchedToken);
                                break;
                            }
                        }

                        nonTerminalMatch = nonTerminalMatch.NextMatch();
                    }
                }

                XmlWriter.WriteEndElement();
                XmlWriter.Close();
            }
            else
            {
                Console.WriteLine("bnf and input do not match");
            }
        }

        // check if definition has nonterminal token
        public bool IsTerminal(string definition)
        {
            Regex nonTerminalRegex = new Regex(RegexAndPatterns.nonTerminalRegexString);
            Match nonTermiranlMatch = nonTerminalRegex.Match(definition);
            if (nonTermiranlMatch.Success)
                return false;
            return true;
        }

        // recursion - finding terminal token and writing XML tree
        private void Recursion(List<BNFCollection> bnfCollections, int tokenIndex, int listIndex, List<string> matchedToken)
        {
            if ( IsTerminal(bnfCollections[tokenIndex].definition) )
            {
                Regex subTokenRegex = new Regex(bnfCollections[tokenIndex].regex);
                Match subTokenMatch = subTokenRegex.Match(matchedToken[listIndex]);

                if (subTokenMatch.Success)
                {
                    XmlWriter.WriteStartElement(bnfCollections[tokenIndex].token);
                    XmlWriter.WriteValue(subTokenMatch.Value);
                    XmlWriter.WriteEndElement();

                    string tmp = matchedToken[listIndex].Replace(subTokenMatch.Value, "");
                    matchedToken[listIndex] = tmp;
                }
            }
            else
            {
                Regex tokenRegex = new Regex(RegexAndPatterns.nonTerminalRegexString);
                Match tokenMatcher = tokenRegex.Match(bnfCollections[tokenIndex].token);

                XmlWriter.WriteStartElement(bnfCollections[tokenIndex].token);

                while (tokenMatcher.Success)
                {
                    string trimedNonTerminal = tokenMatcher.Value.Trim('<', '>');
                    for (int i = 0; i < bnfCollections.Count; i++)
                    {
                        if (bnfCollections[i].token == trimedNonTerminal)
                        {
                            Recursion(bnfCollections, i, listIndex, matchedToken);
                            break;
                        }
                    }

                    tokenMatcher = tokenMatcher.NextMatch();
                }

                XmlWriter.WriteEndElement();
            }
        }
    }
}