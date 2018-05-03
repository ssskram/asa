using System;
using System.Collections.Generic;
using System.Linq;


namespace ASA
{
    class Score
    {
        public int scoreNames(string name, int index, string article)  
        {   
            int score = 0;

            // split each name into individual words
            var name_split = name.Where(Char.IsPunctuation).Distinct().ToArray();
            var name_parts = name.Split().Select(x => x.Trim(name_split));

            string[] hot = new string[] {
                "authorities",
                "arrested",
                "identified",
                "investigators",
                "attack",
                "gunman",
                "dead",
                "deadly",
                "killed",
                "identify",
                "confirmed",
                "accused",
                "suspect",
                "opened",
                "fire",
                "suspected",
                "police",
                "gunshot",
                "alleged",
                "assailant",
                "wound",
                "wounded",
                "fatally",
                "shooting",
                "shooter",
                "shot",
                "according",
                "victims",
                "attacker",
                "motive",
                "self-inflicted",
                "alleged",
            };
            string[] warm  = new string[] {
                "reportedly",
                "reported",
                "related",
                "described",
                "reporting",
                "reports",
                "opened",
                "gun",
                "accused",
                "shot",
                "turning",
                "discovered",
                "disgruntled",
                "mental",
                "health",
                "active",
                "fire",
                "resident",
                "motive",
                "investigate",
                "reported",
                "evidence",
                "individuals",
                "targeted",
                "time",
                "source",
                "evidence",
                "year",
                "sources",
                "sunday",    
                "monday",
                "tuesday",
                "wednesday",
                "thursday",
                "friday",
                "saturday"
            };
            string [] cool = new string[] {
                "have",
                "who",
                "that",
                "the",
                "in",
                "the",
                "was",
                "a",
                "who",
                "to",
                "of",
                "as",
                "knew"
            };
            string [] cold = new string[] {
                "have",
                "who",
                "that",
                "the",
                "in",
                "the",
                "was",
                "a",
                "who",
                "to",
                "of",
                "as",
                "and",
                "knew",
                "sign",
                "in",
                "facebook",
                "twitter",
                "instagram",
                "google",
                "news",
                "world",
                "his",
                "her",
                "about",
                "us",
                "tag",
                "log",
                "ad",
                "click",
                "content",
                "end",
                "your",
                "address",
                "email",
                "press"
            };

            // bypass names that contain substrings of hot[] or warm[] or cold[]
            int wc = 0;
            foreach (string x in hot)
            {
                bool contains = name.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0;
                if (contains == true)
                {
                    wc++;
                }
            }
            foreach (string x in warm)
            {
                bool contains = name.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0;
                if (contains == true)
                {
                    wc++;
                }
            }
            foreach (string x in cold)
            {
                foreach (string p in name_parts)
                {
                    if (p.Equals(x, StringComparison.InvariantCultureIgnoreCase))
                    {
                        wc++;
                    }
                }
            }

            if (wc == 0)
            {
                foreach (string x in hot)
                {
                    // get index for each occurence of hot word
                    int[] occ_hot = allIndexes(article, x);

                    foreach (string p in name_parts)
                    {
                        // get index for each occurence of name part
                        int[] occ_np = allIndexes(article, p);

                        foreach (int i in occ_hot)
                        {
                            foreach (int n in occ_np)
                            {
                                bool check = findDistance(n, i, article, "hot");
                                if (check == true)
                                {
                                    score = score + 5;
                                }
                            }
                        }
                    }
                }
                foreach (string x in warm)
                {
                    // get index for each occurence of warm word
                    int[] occ_warm = allIndexes(article, x);

                    foreach (string p in name_parts)
                    {
                        // get index for each occurence of name part
                        int[] occ_np = allIndexes(article, p);

                        foreach (int i in occ_warm)
                        {
                            foreach (int n in occ_np)
                            {
                                bool check = findDistance(n, i, article, "warm");
                                if (check == true)
                                {
                                    score = score + 3;
                                }
                            }
                        }
                    }
                }
                foreach (string x in cool)
                {
                    // get index for each occurence of warm word
                    int[] occ_cool = allIndexes(article, x);

                    foreach (string p in name_parts)
                    {
                        // get index for each occurence of name part
                        int[] occ_np = allIndexes(article, p);

                        foreach (int i in occ_cool)
                        {
                            foreach (int n in occ_np)
                            {
                                bool check = findDistance(n, i, article, "cool");
                                if (check == true)
                                {
                                    score = score + 1;
                                }
                            }
                        }
                    }
                }
            }

            return score;            
        }

        public static int[] allIndexes(string str, string substr, bool ignoreCase = true)
        {
            var indexes = new List<int>();
            int index = 0;

            while ((index = str.IndexOf(substr, index, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) != -1)
            {
                indexes.Add(index++);
            }

            return indexes.ToArray();
        }

        bool findDistance(int index, int target, string article, string type) 
        {
            bool result = false;
            int before = 0;
            int after = 0;
            int distance;

            if (type == "hot")
            {
                distance = 150;
            }
            else if (type == "warm")
            {
                distance = 200;
            }
            else // cool
            {
                distance = 250;
            }
            
            // check target preceding name
            try
            {
                int startIndex = target;
                int endIndex = index;
                if (endIndex > startIndex)
                {
                    before = endIndex - startIndex;
                }
            }
            catch {}

            // check target after name 
            try
            {
                int startIndex = index;
                int endIndex = target;
                if (endIndex > startIndex)
                {
                    after = endIndex - startIndex;
                }
            }
            catch {} 

            if ((before < distance && before > 0 && before != 0) || 
                (after < distance && after  > 0 && before != 0))
                {
                    result = true;
                }

            return result;
        }
    }
}