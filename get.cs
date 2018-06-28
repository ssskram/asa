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
            // object of "winning" names to be passed back to Main()
            object ws = null;
            // list to populate with scored names    
            List<scoredName> names = new List<scoredName>();
            // scoring class
            Score scorecard = new Score();
            Console.WriteLine("Getting all articles...");
            // query parameters
            string keywords = "identified OR police OR suspect OR confirmed OR active";
            string antikeywords = "drill OR training";
            string pagesize = "1";
            string page = "4";
            string key = "df76585c7c104053896b14dd3be4d007";
            // dev time frame
            string from = "2018-05-27";
            string to = "2018-05-29";
            // prod time frame is same as cron
            // string from = DateTime.UtcNow.AddMinutes(-5).ToString("s");
            // string to = DateTime.UtcNow.ToString("s");
            string endpoint = String.Format
                ("https://newsapi.org/v2/everything?q='+shooter' AND ({0}) NOT ({1})&language=en&pageSize={2}&page={3}&from={4}&to{5}&apiKey={6}",
                    keywords, // 0
                    antikeywords, // 1
                    pagesize, // 2
                    page, // 3
                    from, // 4
                    to, // 5
                    key); // 6
            // get em
            string articles = await client.GetStringAsync(endpoint);
            if (articles != null)
            {
                dynamic all = JObject.Parse(articles)["articles"];
                foreach (var item in all)
                {
                    string article = null;
                    string url = item.url.ToString();
                    Console.WriteLine("Getting article:");
                    Console.WriteLine(url);
                    try
                    {
                        article = await client.GetStringAsync(url);
                        // take the body
                        int pFrom = article.IndexOf("<body") + "<body".Length;
                        int pTo = article.LastIndexOf("</body>");
                        article = article.Substring(pFrom, pTo - pFrom);
                    }
                    catch {}
                    // get possible names
                    var possiblenames = getPossibleNames(article);
                    if (possiblenames != null)
                    {
                        List<possibleName> ns = possiblenames as List<possibleName>;
                        Console.WriteLine("Scoring each possible name...");
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
                var an = aggregateNames(names);
                ws = await selectWinners(an);
            }
            return(ws);
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
                foreach (Match match in n.Matches(article))
                {
                    // only take 1-3 word strings
                    int wordcount = countWords(match.ToString());
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
                return possiblenames;
            }
            return null;
        }

        public static object aggregateNames(object sn)
        {
            Console.WriteLine("...aggregating results...");
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
                else if (name.score > 0)
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

        public async Task<object> selectWinners(object an)
        {
            Console.WriteLine("...selecting the winners...");
            // list to populate with the winners
            List<winner> ws = new List<winner>();
            List<aggregatedName> ns = an as List<aggregatedName>;
            var top = ns.OrderByDescending(i => i.score).Take(7);

            foreach (var i in ns)
            {
                winner w = new winner()
                {
                    value = i.value,
                    score = i.score
                };
                ws.Add(w);
            }

            // foreach (var i in top)
            // {
            //     // attempt to geocode the string to root out city names
            //     string key = "AIzaSyA8hIHTerE_b51886Q761BNQ53sQUsI97E";
            //     var endpoint =
            //         String.Format 
            //         ("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}",
            //         i.value, // 0
            //         key); // 1
            //     client.DefaultRequestHeaders.Clear();
            //     try
            //     {
            //         string response = await client.GetStringAsync(endpoint);
            //         dynamic status_check = JObject.Parse(response)["status"];
            //         if (status_check == "OK")
            //         {
            //             // loooooooooser
            //             continue;
            //         }
            //         else
            //         {
            //             winner w = new winner()
            //             {
            //                 value = i.value,
            //                 score = i.score
            //             };
            //             ws.Add(w);
            //         }
            //     }
            //     catch {}
            // }

            return ws;
        }

        public static int countWords(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }
    }
}
