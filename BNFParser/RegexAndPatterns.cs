using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MainClass
{
    public class RegexAndPatterns
    {
        //________________________________________________________
        public const string Spaces = @"\s*";
        public const string NonTerminalRegexString = @"(<[\w -]+>)";
        public const string TerminalRegexString = @"(""(.+?)"")";

        public const string AllTableStandardExpressions = @"(broj_telefona)" + "|" + "(mejl_adresa)" + "|" + "(web_link)" + "|" +
                                               "(brojevna_konstanta)" + "|" + "(veliki_grad)";

        public const string RegexRegexString = @"regex\((.*?)\)";
        //________________________________________________________

        public const string LeftSide = NonTerminalRegexString + Spaces;

        public const string RightSide = @"(" + Spaces + "|" + NonTerminalRegexString + "|" + TerminalRegexString + "|" +
                                        AllTableStandardExpressions + "|" + RegexRegexString + "|\\|"+ ")+";

        public const string BnfLineRegexString = "^"+LeftSide + "::=" + RightSide + "$";
        //________________________________________________________
        
        public const string BrojTelefona = @"(((\()?(00|\+)387(65|66|51)(\))?)|(0(65|66|51)))?[- \/]?\d{3}[ -]?\d{3}";
        public const string MejlAdresa = @"\w+([.]\w+){0,3}@\w+([-.]\w+){0,1}\.\w+([-.]\w+){0,3}";

        public const string WebLink =
            @"((https?:\/\/){0,1}(www\.){0,1}){1}\w+([-.]\w+){0,1}\.\w+([-.]\w+){0,3}([\w\/?=&]+)?";

        public const string BrojevnaKonstanta = @"-?\d+(\.\d+)?";
        //________________________________________________________


        public const string Link = "http://worldpopulationreview.com/continents/cities-in-europe/";

        // returns html content from site
        public static string GetSiteHtml() 
        {
            HttpWebRequest http = (HttpWebRequest) WebRequest.Create(Link);    // throws UriFormatException
            HttpWebResponse webResponse = (HttpWebResponse) http.GetResponse();

            Stream responseStream = responseStream = webResponse.GetResponseStream();
            if (webResponse.ContentEncoding.ToLower().Contains("gzip"))
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            else if (webResponse.ContentEncoding.ToLower().Contains("deflate"))
                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

            StreamReader reader = new StreamReader(responseStream, Encoding.Default);

            string html = reader.ReadToEnd();

            webResponse.Close();
            responseStream.Close();

            return html;
        }

        // creating list of big cities
        public static List<string> GetCitiesFromHtml() 
        {
            string html = GetSiteHtml();
            List<string> cityList = new List<string>();

            const string cityHtmlString = @"<tr><td>([\w ]+)<\/td><td><a href=""\/countries\/";    // group[1] == city
            Regex cityHtmlRegex = new Regex(cityHtmlString);

            int i = 0;
            foreach (Match cityHtmlMatch in cityHtmlRegex.Matches(html))
            {
                GroupCollection groups = cityHtmlMatch.Groups;
                cityList.Add(groups[1].Value.Trim());
                i++;
                if (i >= 200)    
                    break;
            }

            return cityList;
        }

        // creating regexPattern from city list
        public static string MakeCityRegexString() 
        {
            var cities = GetCitiesFromHtml();

            string cityRegexString = null;

            foreach (var city in cities)
            {
                cityRegexString += city + "|";
            }

            return cityRegexString.Trim('|');
        }
    }
}