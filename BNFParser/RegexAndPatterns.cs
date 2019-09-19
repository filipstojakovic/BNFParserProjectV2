using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MainClass
{
    public class RegexAndPatterns
    {
        //________________________________________________________
        public const string spaces = @"\s*";
        public const string nonTerminalRegexString = @"(<[\w -]+>)";
        public const string terminalRegexString = @"(""(.+?)"")";

        public const string allTableStandardExpressions = @"(broj_telefona)" + "|" + "(mejl_adresa)" + "|" + "(web_link)" + "|" +
                                               "(brojevna_konstanta)" + "|" + "(veliki_grad)";

        public const string regexRegexString = @"regex\((.*?)\)";
        //________________________________________________________

        public const string leftSide = nonTerminalRegexString + spaces;

        public const string rightSide = @"(" + spaces + "|" + nonTerminalRegexString + "|" + terminalRegexString + "|" +
                                        allTableStandardExpressions + "|" + regexRegexString + "|\\|"+ ")+";

        public const string BNFLineRegexString = "^"+leftSide + "::=" + rightSide + "$";
        //________________________________________________________
        
        public const string broj_telefona = @"(((\()?(00|\+)387(65|66|51)(\))?)|(0(65|66|51)))?[- \/]?\d{3}[ -]?\d{3}";
        public const string mejl_adresa = @"\w+([.]\w+){0,3}@\w+([-.]\w+){0,1}\.\w+([-.]\w+){0,3}";

        public const string web_link =
            @"((https?:\/\/){0,1}(www\.){0,1}){1}\w+([-.]\w+){0,1}\.\w+([-.]\w+){0,3}([\w\/?=&]+)?";

        public const string brojevna_konstanta = @"-?\d+(\.\d+)?";
        //________________________________________________________


        public const string link = "http://worldpopulationreview.com/world-cities/";

        // returns html content from site
        public static string getSiteHtml() 
        {
            HttpWebRequest Http = (HttpWebRequest) WebRequest.Create(link);    // throws UriFormatException
            HttpWebResponse WebResponse = (HttpWebResponse) Http.GetResponse();

            Stream responseStream = responseStream = WebResponse.GetResponseStream();
            if (WebResponse.ContentEncoding.ToLower().Contains("gzip"))
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            else if (WebResponse.ContentEncoding.ToLower().Contains("deflate"))
                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

            StreamReader Reader = new StreamReader(responseStream, Encoding.Default);

            string html = Reader.ReadToEnd();

            WebResponse.Close();
            responseStream.Close();

            return html;
        }

        // creating list of big cities
        public static List<string> getCitiesFromHtml() 
        {
            string html = getSiteHtml();
            List<string> cityList = new List<string>();

            const string cityHtmlString = @"<td><a.*?>([\w ]+)<\/a><\/td>";
            Regex cityHtmlRegex = new Regex(cityHtmlString);

            int i = 0;
            foreach (Match cityHtmlMatch in cityHtmlRegex.Matches(html))
            {
                GroupCollection groups = cityHtmlMatch.Groups;
                cityList.Add(groups[1].Value.Trim());
                i++;
                if (i >= 204)
                    break;
            }

            return cityList;
        }

        // creating regexPattern from city list
        public static string makeCityRegexString() 
        {
            var cities = getCitiesFromHtml();

            string cityRegexString = null;

            foreach (var city in cities)
            {
                cityRegexString += city + "|";
            }

            return cityRegexString.Trim('|');
        }
    }
}