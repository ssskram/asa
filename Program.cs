using System;
using System.Threading.Tasks;

namespace ASA
{
    class Program
    {
        // enter here
        static async Task Main()
        {
            Get g = new Get();
            await g.getNames();
            var names = g.getNames().Result;

            Scrape t = new Scrape();
            await t.Queries(names);
        }
    }
}
