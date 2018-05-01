using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace ASA
{
    class Scrape
    {
        public async Task Queries(object names)
        {
            // throw collected names for development purposes
            List<aggregatedName> a = names as List<aggregatedName>;
            foreach (var name in a)
            {
                Console.WriteLine("Name = " + name.value + "and score  =  " + name.score);
            }
            await Task.Delay(1);

            // begin scraping here
        }

    }
}
