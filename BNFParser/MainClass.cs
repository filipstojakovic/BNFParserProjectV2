using System;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;

namespace MainClass
{
    internal class Program
    {
        public const string configFile2 =
            @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\Fajlovi\myConfig.bnf";

        public const string configFile = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\Fajlovi\config.bnf";

        public const string configFile1 = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\Fajlovi\numberBNF.txt";

        public const string intputFile1 = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\input.txt";
        public const string intputFile = @"C:\Users\filip\Desktop\formalne_projekat Kajganic\input.txt";

        public const string outputFile = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\output.xml";

        
        public static void Main(string[] args)
        {
//            Console.WriteLine(RegexAndPatterns.BNFLineRegexString); // config.bnf 

            BnfMaker bnfMaker = new BnfMaker();

            try
            {
                bnfMaker.ReadConfigFile(configFile);

//                Console.WriteLine("config regex: " + RegexAndPatterns.BNFLineRegexString);

                bnfMaker.bnfCollections.ForEach(x =>
                    {
                        Console.WriteLine(x);
                        Console.WriteLine();
                    }
                );

                Console.WriteLine('\n'+bnfMaker.bnfCollections[0].regex);
                Console.WriteLine("Input: " + new StreamReader(intputFile).ReadLine());
                Console.Write("result: ");
                XmlCreator xmlCreator = new XmlCreator();
                xmlCreator.CreateXml(intputFile, outputFile, bnfMaker.bnfCollections);

            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Invalid File path");
                Console.WriteLine(ex);
            }
            catch (BnfLineExceptions ex)
            {
                Console.WriteLine(ex);
            }
            catch (UriFormatException ex)
            {
                Console.WriteLine("invalid URL");
                Console.WriteLine(ex);
            }
            catch (IOException ex)
            {
                Console.WriteLine("IOException");
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}