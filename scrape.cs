using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace ASA
{
    class Scrape
    {
        public async Task Queries(object names)
        {
            // high point
            List<winner> a = names as List<winner>;

            if (names != null)
            {
                foreach (var name in a)
                {
                    Console.WriteLine("Name = " + name.value + " and score = " + name.score);
                }
                await Task.Delay(1);

                // begin scraping here
                // await getFacebook()
                // await getTwitter()
                // await getYoutube()
                // await getYaddaYadda()
            }
        }

    }
}
