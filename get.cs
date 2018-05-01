using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ASA
{
    class Get
    {
        HttpClient client = new HttpClient();

        public async Task<object> getNames()
        {
            // object of qualified names to be passed back to main
            object an = null;

            // list to populate with scored names    
            List<scoredName> names = new List<scoredName>();

            // instantiate class to score possible names
            Score scorecard = new Score();
            
            Console.WriteLine("Getting all articles...");
    
            var time = DateTime.UtcNow.AddDays(-30).ToString("s");
            var key = "df76585c7c104053896b14dd3be4d007";
            string endpoint = String.Format
                ("https://newsapi.org/v2/everything?q='active+shooter' AND (identified OR suspect) NOT (drill OR training)&language=en&pageSize=15&from=2018-04-04&to2018-04-04&apiKey={1}",
                    time, // 0
                    key); // 1
            string articles = await client.GetStringAsync(endpoint);

            if (articles != null)
            {
                dynamic all = JObject.Parse(articles)["articles"];

                foreach (var item in all)
                {
                    string url = item.url.ToString();
                    Console.WriteLine(url);
                    Console.WriteLine("Getting article...");

                    string article = null;
                    try
                    {
                        article = await client.GetStringAsync(url);
                        // take the body
                        int pFrom = article.IndexOf("<body") + "<body".Length;
                        int pTo = article.LastIndexOf("</body>");
                        article = article.Substring(pFrom, pTo - pFrom);
                    }
                    catch
                    {}

                    // get possible names
                    var possiblenames = getPossibleNames(article);

                    if (possiblenames != null)
                    {
                        List<possibleName> ns = possiblenames as List<possibleName>;

                        foreach (var pn in ns)
                        {
                            int score = scorecard.scoreNames(pn.value, pn.index, article);
                            scoredName qn = new scoredName()
                            {
                                value = pn.value,
                                score = score
                            };
                            names.Add(qn);
                        }
                    }
                    Console.WriteLine("Done! Moving on...");
                }

                an = aggregateNames(names);
            }

            return(an);
        }

        public static object getPossibleNames(string article)
        {
            if (article != null)
            {
                // pattern to identify possible names
                string namePattern = @"(([A-Z]([a-z]+|\.+))+(\s[A-Z][a-z]+)+)";
                Regex n = new Regex(namePattern);

                // list to populate with possible names
                List<possibleName> possiblenames = new List<possibleName>();

                Console.WriteLine("Getting possible names...");

                // get the proper nouns
                foreach (Match match in n.Matches(article))
                {
                    int wordcount = CountWords(match.ToString());
                    if (match != null && wordcount > 1 && wordcount < 4)
                    {
                        possibleName pona = new possibleName()
                        {
                            value = match.Value.ToString(),
                            index = match.Index
                        };
                        possiblenames.Add(pona);
                    }
                }
                Console.WriteLine("OK, that's all of em.  Sending them back to be scored...");
                return possiblenames;
            }

            return null;
        }

        public static int CountWords(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }

        public static object aggregateNames(object sn)
        {
            List<scoredName> ns = sn as List<scoredName>;

            // hash to track duplicates to be merged
            HashSet<string> alreadyEncountered = new HashSet<string>();

            // list to populate with aggregated names
            List<aggregatedName> an = new List<aggregatedName>();

            // aggregate the scores from repeated names
            foreach (var name in ns)
            {
                if (alreadyEncountered.Contains(name.value))
                {
                    an.Where(w => w.value == name.value).ToList().ForEach(s => s.score = s.score + name.score);
                }
                else if (name.score != 0)
                {
                    alreadyEncountered.Add(name.value); 
                    aggregatedName aa = new aggregatedName() 
                    {
                        value = name.value,
                        score = name.score
                    };
                    an.Add(aa);
                }
            }

           return(an);
        }
    }
}
