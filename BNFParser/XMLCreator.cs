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
        private XmlWriter xmlWriter; // { get; set; }
        private XmlWriterSettings settings;

        public XmlCreator()
        {
            settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineOnAttributes = true;
        }


        // creating the XML file
        public void CreateXml(string inputPath, string outputPath, List<BnfCollection> bnfCollections)
        {
            StreamReader streamReader = new StreamReader(inputPath);
            string readLine = streamReader.ReadLine();
            streamReader.Close();
            if (readLine == null)
                throw new Exception("input file readline is null!");

            List<string> matchedToken = new string[bnfCollections.Count].Select(x => "").ToList();
            matchedToken[0] = readLine;

            xmlWriter = XmlWriter.Create(outputPath, settings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement(bnfCollections[0].Token); // insert root

            for (int i = 0; i < bnfCollections.Count; i++)
            {
                Regex nonTerminalRegex = new Regex(RegexAndPatterns.NonTerminalRegexString);
                Match nonTerminalMatch = nonTerminalRegex.Match(bnfCollections[i].Definition);

                while (nonTerminalMatch.Success)
                {
                    string nonTerminalValue = nonTerminalMatch.Value.Trim('<', '>');
                    for (int j = 0; j < bnfCollections.Count; j++)
                    {
                        if (bnfCollections[j].Token == nonTerminalValue)
                        {
                            WriteInXml(bnfCollections, j, i, matchedToken);
                            break;
                        }
                    }

                    nonTerminalMatch = nonTerminalMatch.NextMatch(); // go to next nonTerminal
                }
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Close();
            
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
        private void WriteInXml(List<BnfCollection> bnfCollections, int tokenIndex, int listIndex, List<string> matchedToken)
        {
            if (IsTerminal(bnfCollections[tokenIndex].Definition))
            {
                Regex subTokenRegex = new Regex(bnfCollections[tokenIndex].Regex);
                Match subTokenMatch = subTokenRegex.Match(matchedToken[listIndex]); // terminal match in matchedToken

                if (subTokenMatch.Success)    
                {
                    xmlWriter.WriteStartElement(bnfCollections[tokenIndex].Token);
                    xmlWriter.WriteValue(subTokenMatch.Value);
                    xmlWriter.WriteEndElement();

                    string tmp = matchedToken[listIndex].Replace(subTokenMatch.Value, "");
                    matchedToken[listIndex] = tmp;
                }
            }
            else
            {
                Regex tokenRegex = new Regex(RegexAndPatterns.NonTerminalRegexString);
                Match tokenMatcher = tokenRegex.Match(bnfCollections[tokenIndex].Token);

                xmlWriter.WriteStartElement(bnfCollections[tokenIndex].Token);

                while (tokenMatcher.Success)
                {
                    string trimedNonTerminal = tokenMatcher.Value.Trim('<', '>');
                    for (int i = 0; i < bnfCollections.Count; i++)
                    {
                        if (bnfCollections[i].Token == trimedNonTerminal)
                        {
                            WriteInXml(bnfCollections, i, listIndex, matchedToken);
                            xmlWriter.WriteEndElement();
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