using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;


namespace MainClass
{
    public class XmlCreator
    {
        private XmlWriter xmlWriter;// { get; set; }
        private XmlWriterSettings settings;

        public XmlCreator()
        {
            settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineOnAttributes = true;
        }


        // creating the XML file
        public void CreateXml(string inputPath, string outputPath, List<BNFCollection> bnfCollections)
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

            xmlWriter = XmlWriter.Create(outputPath, settings);
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

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement(bnfCollections[0].token); // insert root


                for (int i = 0; i < bnfCollections.Count; i++)
                {
                    Regex nonTerminalRegex = new Regex(RegexAndPatterns.NonTerminalRegexString);
                    Match nonTerminalMatch = nonTerminalRegex.Match(bnfCollections[i].definition);

                    while (nonTerminalMatch.Success)
                    {
                        string nonTerminalValue = nonTerminalMatch.Value.Trim('<', '>');
                        for (int j = 0; j < bnfCollections.Count; j++)
                        {
                            if (bnfCollections[j].token == nonTerminalValue)
                            {
                                WriteInXml(bnfCollections, j, i, matchedToken);
                                break;
                            }
                        }

                        nonTerminalMatch = nonTerminalMatch.NextMatch();
                    }
                }

                xmlWriter.WriteEndElement();
                xmlWriter.Close();
            }
            else
            {
                Console.WriteLine("bnf and input do not match");
            }
        }

        // check if definition has nonterminal token
        private bool IsTerminal(string definition)
        {
            Regex nonTerminalRegex = new Regex(RegexAndPatterns.NonTerminalRegexString);
            Match nonTermiranlMatch = nonTerminalRegex.Match(definition);
            if (nonTermiranlMatch.Success)
                return false;
            return true;
        }

        // recursion - finding terminal token and writing XML tree
        private void WriteInXml(List<BNFCollection> bnfCollections, int tokenIndex, int listIndex, List<string> matchedToken)
        {
            if ( IsTerminal(bnfCollections[tokenIndex].definition) )
            {
                Regex subTokenRegex = new Regex(bnfCollections[tokenIndex].regex);
                Match subTokenMatch = subTokenRegex.Match(matchedToken[listIndex]);

                if (subTokenMatch.Success)
                {
                    xmlWriter.WriteStartElement(bnfCollections[tokenIndex].token);
                    xmlWriter.WriteValue(subTokenMatch.Value);
                    xmlWriter.WriteEndElement();

                    string tmp = matchedToken[listIndex].Replace(subTokenMatch.Value, "");
                    matchedToken[listIndex] = tmp;
                }
            }
            else
            {
                Regex tokenRegex = new Regex(RegexAndPatterns.NonTerminalRegexString);
                Match tokenMatcher = tokenRegex.Match(bnfCollections[tokenIndex].token);

                xmlWriter.WriteStartElement(bnfCollections[tokenIndex].token);

                while (tokenMatcher.Success)
                {
                    string trimedNonTerminal = tokenMatcher.Value.Trim('<', '>');
                    for (int i = 0; i < bnfCollections.Count; i++)
                    {
                        if (bnfCollections[i].token == trimedNonTerminal)
                        {
                            WriteInXml(bnfCollections, i, listIndex, matchedToken);
                            break;
                        }
                    }

                    tokenMatcher = tokenMatcher.NextMatch();
                }

                xmlWriter.WriteEndElement();
            }
        }
    }
}