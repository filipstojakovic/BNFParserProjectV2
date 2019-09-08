using System;
using System.IO;

namespace MainClass
{
    internal class Program
    {
        public const string configFile =
            @"C:\Users\filip\RiderProjects\BNFParserProjectV2\BNFParser\Fajlovi\myConfig.bnf";
        
        public static void Main(string[] args)
        {
//            Console.WriteLine(RegexAndPatterns.BNFLineRegexString);
            
            BNFMaker bnfMaker = new BNFMaker();

            try
            {
                bnfMaker.readConfigFile(configFile);

                bnfMaker.bnfCollections.ForEach(Console.WriteLine);




            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex);

            }
            catch (BNFLineExceptions ex)
            {
                Console.WriteLine(ex);
            }
            catch (UriFormatException ex)
            {
                Console.WriteLine("invalid URL");
                Console.WriteLine(ex);
            }

        }
    }
}