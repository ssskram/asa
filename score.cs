using System;

namespace ASA
{
    class Score
    {
        public int scoreNames(string name, int index, string article)  
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
                "related",
                "Related",
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
                        bool check = findDistance(index, x, article, "hot");
                        if (check == true)
                        {
                            score = score + 7;
                        }
                    }
                }
                foreach (string x in warm)
                {
                    if (article.Contains(x))
                    {
                        bool check = findDistance(index, x, article, "warm");
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
                        bool check = findDistance(index, x, article, "cool");
                        if (check == true)
                        {
                            score = score + 1;
                        }
                    }
                }
            }

            return score;            
        }

        bool findDistance(int index, string target, string article, string type) 
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
                int startIndex = article.IndexOf(target);
                int endIndex = index;
                if (endIndex > startIndex)
                {
                    string newString = article.Substring(startIndex, endIndex - startIndex);
                    before = newString.Length;
                }
            }
            catch {}

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