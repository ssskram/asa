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
    class Scrape
    {
        HttpClient client = new HttpClient();

        public async Task Queries(object names)
        {
            List<aggregatedName> a = names as List<aggregatedName>;
            foreach (var name in a)
            {
                Console.WriteLine("Name = " + name.value + "and score  =  " + name.score);
            }
            await Task.Delay(1);

            // hit social media APIs
            // 
        }

    }
}
