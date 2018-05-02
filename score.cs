using System;
using System.Collections.Generic;


namespace ASA
{
    class Score
    {
        public int scoreNames(string name, int index, string article)  
        {
            int score = 0;

            string[] hot = new string[] {
                "identified",
                "identify",
                "confirmed",
                "suspect",
                "suspected",
                "police",
                "wound",
                "wounded",
                "fatally",
                "shooting",
                "shooter",
                "shot",
                "according",
                "victims",
                "attacker",
                "self-inflicted",
                "sunday",   
                "monday",
                "tuesday",
                "wednesday",
                "thursday",
                "friday",
                "saturday"
            };
            string[] warm  = new string[] {
                "reportedly",
                "reported",
                "related",
                "described",
                "gun",
                "accused",
                "shot",
                "turning",
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
                "sources"
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
                "to",
                "of",
                "as",
                "knew"
            };

            int wc = 0;

            foreach (string x in hot)
            {
                if (name.Contains(x))
                {
                    wc++;
                }
            }
            foreach (string x in warm)
            {
                if (name.Contains(x))
                {
                    wc++;
                }
            }

            if (wc == 0)
            {
                foreach (string x in hot)
                {
                    int[] occ = allIndexes(article, x);
                    foreach(int i in occ)
                    {
                        bool check = findDistance(index, i, article, "hot");
                        if (check == true)
                        {
                            score = score + 7;
                        }
                    }
                }
                foreach (string x in warm)
                {
                    int[] occ = allIndexes(article, x);
                    foreach(int i in occ)
                    {
                        bool check = findDistance(index, i, article, "warm");
                        if (check == true)
                        {
                            score = score + 2;
                        }
                    }
                }
                foreach (string x in cool)
                {
                    int[] occ = allIndexes(article, x);
                    foreach(int i in occ)
                    {
                        bool check = findDistance(index, i, article, "cool");
                        if (check == true)
                        {
                            score = score + 1;
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
                distance = 250;
            }
            else if (type == "warm")
            {
                distance = 400;
            }
            else // cool
            {
                distance = 500;
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