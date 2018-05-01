﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Data;

namespace ASA
{
    class Program
    {
        HttpClient client = new HttpClient();

        // enter here
        static async Task Main()
        {
            Program run = new Program();
            await run.getNames();
        }

        public async Task getNames()
        {
            // list to populate with scored names    
            List<scoredName> names = new List<scoredName>();
            
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
                    // // ...just for development purposes ->
                    // string url = "http://www.businessinsider.com/youtube-shooting-suspect-name-age-description-headquarters-san-bruno-2018-4";
                    
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
                            int score = scoreNames(pn.value, pn.index, article);
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

                await aggregateNames(names);
            }
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

        int scoreNames(string name, int index, string article)
        {
            int score = 0;

            string[] hot = new string[] {
                "identified",
                "Identified",
                "identify",
                "Identify",
                "Confirmed",
                "confirmed",
                "suspect",
                "Suspect",
                "suspected",
                "Suspected",
                "police",
                "Police",
                "wounded",
                "Wounded",
                "fatally",
                "shooting",
                "Shooting",
                "shooter",
                "Shooter",
                "according",
                "Victims",
                "victims"
            };
            string[] warm  = new string[] {
                "reportedly",
                "Reportedly",
                "described",
                "Described",
                "active",
                "fire",
                "Active",
                "resident",
                "Resident",
                "Sunday",   
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday",
                "motive",
                "investigate",
                "reported",
                "Reported",
                "evidence",
                "individuals",
                "targeted",
                "source",
                "year",
                "Sources"
            };
            string [] cool = new string[] {
                "have",
                "that",
                "the",
                "in",
                "the",
                "was",
                "a",
                "who",
                "of",
                "as",
                "knew"
            };

            int hwc = 0;

            foreach (string x in hot)
            {
                if (name.Contains(x))
                {
                    hwc++;
                }
            }
            foreach (string x in warm)
            {
                if (name.Contains(x))
                {
                    hwc++;
                }
            }

            if (hwc == 0)
            {
                foreach (string x in hot)
                {
                    if (article.Contains(x))
                    {
                        bool check = FindDistance(index, x, article, "hot");
                        if (check == true)
                        {
                            score = score + 6;
                        }
                    }
                }
                foreach (string x in warm)
                {
                    if (article.Contains(x))
                    {
                        bool check = FindDistance(index, x, article, "warm");
                        if (check == true)
                        {
                            score = score + 2;
                        }
                    }
                }
                foreach (string x in cool)
                {
                    if (article.Contains(x))
                    {
                        bool check = FindDistance(index, x, article, "cool");
                        if (check == true)
                        {
                            score = score + 1;
                        }
                    }
                }
            }

            return score;            
        }

        bool FindDistance(int index, string target, string article, string type) 
        {
            bool result = false;
            int before = 0;
            int after = 0;
            int distance;

            if (type == "hot")
            {
                distance = 250;
            }
            else if (type == "warm")
            {
                distance = 400;
            }
            else
            {
                distance = 500;
            }
            

            // check target preceding name
            try
            {
                int startIndex = article.IndexOf(target);
                int endIndex = index;
                if (endIndex > startIndex)
                {
                    string newString = article.Substring(startIndex, endIndex - startIndex);
                    before = newString.Length;
                }
            }
            catch (Exception e)
            {Console.WriteLine(e);}

            // check target after name 
            try
            {
                int startIndex = index;
                int endIndex = article.LastIndexOf(target);
                if (endIndex > startIndex)
                {
                    string newString = article.Substring(startIndex, endIndex - startIndex);
                    after = newString.Length;
                }
            }
            catch (Exception e)
            {Console.WriteLine(e);} 

            if ((before < distance && before > 0 && before != 0) || 
                (after < distance && after  > 0 && before != 0))
                {
                    result = true;
                }

            return result;
        }

        public async Task aggregateNames(object sn)
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
            await pushNames(an);
        }

        public async Task pushNames(object an)
        {
            List<aggregatedName> a = an as List<aggregatedName>;

            foreach (var name in a)
            {
                Console.WriteLine("Name = " + name.value + "and score  =  " + name.score);
            }

            // push high scoring names off to scraping function

            await Task.Delay(1);
        }
    }
}
