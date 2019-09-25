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
        private XmlWriter _xmlWriter; // { get; set; }
        private XmlWriterSettings _settings;

        public XmlCreator()
        {
            _settings = new XmlWriterSettings();
            _settings.Indent = true;
            _settings.IndentChars = "\t";
            _settings.NewLineOnAttributes = true;
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

            _xmlWriter = XmlWriter.Create(outputPath, _settings);
            _xmlWriter.WriteStartDocument();
            _xmlWriter.WriteStartElement(bnfCollections[0].Token); // insert root

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

                    nonTerminalMatch = nonTerminalMatch.NextMatch();
                }
            }

            _xmlWriter.WriteEndElement();
            _xmlWriter.Close();
            
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
                Match subTokenMatch = subTokenRegex.Match(matchedToken[listIndex]);

                if (subTokenMatch.Success)
                {
                    _xmlWriter.WriteStartElement(bnfCollections[tokenIndex].Token);
                    _xmlWriter.WriteValue(subTokenMatch.Value);
                    _xmlWriter.WriteEndElement();

                    string tmp = matchedToken[listIndex].Replace(subTokenMatch.Value, "");
                    matchedToken[listIndex] = tmp;
                }
            }
            else
            {
                Regex tokenRegex = new Regex(RegexAndPatterns.NonTerminalRegexString);
                Match tokenMatcher = tokenRegex.Match(bnfCollections[tokenIndex].Token);

                _xmlWriter.WriteStartElement(bnfCollections[tokenIndex].Token);

                while (tokenMatcher.Success)
                {
                    string trimedNonTerminal = tokenMatcher.Value.Trim('<', '>');
                    for (int i = 0; i < bnfCollections.Count; i++)
                    {
                        if (bnfCollections[i].Token == trimedNonTerminal)
                        {
                            WriteInXml(bnfCollections, i, listIndex, matchedToken);
                            break;
                        }
                    }

                    tokenMatcher = tokenMatcher.NextMatch();
                }

                _xmlWriter.WriteEndElement();
            }
        }
    }
}