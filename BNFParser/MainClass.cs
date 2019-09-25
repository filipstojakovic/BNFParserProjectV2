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

        public const string ConfigFile1 = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\Fajlovi\numberBNF.txt";
        public const string ConfigFile3 = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\Fajlovi\config.bnf";
        public const string ConfigFile =  @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\Fajlovi\myConfig.bnf";

        public const string IntputFile = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\input.txt";
        public const string OutputFile = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\output.xml";

        public static void Main(string[] args)
        {

            BnfMaker bnfMaker = new BnfMaker();
            try
            {
                bnfMaker.ReadConfigFile(ConfigFile);

//                bnfMaker.BnfCollections.ForEach(x =>
//                    {
//                        Console.WriteLine(x);
//                        Console.WriteLine();
//                    }
//                );
                
//                Console.WriteLine('\n'+bnfMaker.BnfCollections[0].Regex); // bnfFinalRegex
                Console.WriteLine("Input: " + new StreamReader(IntputFile).ReadLine());
                Console.Write("result: ");
                if (bnfMaker.IsInputFileMatched(IntputFile))
                {
                    Console.WriteLine("Matched Successful!");
                    XmlCreator xmlCreator = new XmlCreator();
                    xmlCreator.CreateXml(IntputFile, OutputFile, bnfMaker.BnfCollections);
                    
                }
                else
                {
                    Console.WriteLine("Match NOT Successful!");
                }

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