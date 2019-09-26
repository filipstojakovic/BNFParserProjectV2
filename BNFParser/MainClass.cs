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

        public static string ConfigFile = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\Fajlovi\config.bnf";
        public static string ConfigFile2 = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\Fajlovi\numberBNF.bnf";
        public static string ConfigFile1 =  @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\Fajlovi\manDog.bnf";

        public const string IntputFile = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\input.txt";
        public static string OutputFile = @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\output.xml";

        public static void Main(string[] args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            if (args.Length == 2)
            {
                ConfigFile = args[0];
                OutputFile = args[1];
            }

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