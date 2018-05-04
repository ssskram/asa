using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ASA
{
    class Score
    {
        public int scoreNames(string name, int index, string article)  
        {   
            int score = 0;
            int wd = 0;

            // split each name into individual words
            var name_split = name.Where(Char.IsPunctuation).Distinct().ToArray();
            var name_parts = name.Split().Select(x => x.Trim(name_split));

            // get arrays of keywords
            string[] hot = getHot();
            string[] warm = getWarm();
            string[] cold = getCold();       

            foreach (string x in hot)
            {
                bool contains = name.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0;
                if (contains == true)
                {
                    wd++;
                }
            }
            foreach (string x in warm)
            {
                bool contains = name.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0;
                if (contains == true)
                {
                    wd++;
                }
            }
            foreach (string p in name_parts)
            {
                foreach (string x in cold)
                {
                    if (p.ToLower().Equals(x))
                    {
                        wd++;
                    }
                }
            }

            if (wd == 0)
            {
                foreach (string p in name_parts)
                {
                    // get index for every occurence of name part
                    int[] occ_np = allIndexes(article, p);

                    foreach (string x in hot)
                    {
                        // get index for each occurence of hot word
                        int[] occ_hot = allIndexes(article, x);

                        // for each occurence of hot word, find distance between hot word & indexes of name part
                        foreach(int i in occ_hot)
                        {
                            foreach (int z in occ_np)
                            {
                                bool check = findDistance(i, z, "hot");
                                if (check == true)
                                {
                                    score = score + 3;
                                }
                            }
                        }
                    }

                    foreach (string x in warm)
                    {
                        // get index for each occurence of warm word
                        int[] occ_warm = allIndexes(article, x);

                        // for each occurence of hot word, find distance between warm word & indexes of name part
                        foreach(int i in occ_warm)
                        {
                            foreach (int z in occ_np)
                            {
                                bool check = findDistance(i, z, "warm");
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

        bool findDistance(int index, int target, string type) 
        {
            bool result = false;
            int before = 0;
            int after = 0;
            int distance;

            if (type == "hot")
            {
                distance = 75;
            }
            else // warm
            {
                distance = 150;
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

        public static string[] getHot()
        {
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

            return hot;
        }
        public static string[] getWarm()
        {
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

            return warm;
        }
        public static string[] getCold()
        {
            string [] cold = new string[] {
                "have",
                "who",
                "that",
                "the",
                "you",
                "your",
                "in",
                "they",
                "was",
                "a",
                "who",
                "to",
                "of",
                "as",
                "and",
                "knew",
                "not",
                "real",
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
                "site",
                "header",
                "footer",
                "email",
                "press"
            };     

            return cold;
        }
    }
}