using System;
using System.Threading.Tasks;

namespace ASA
{
    class Program
    {
        // enter here
        static async Task Main()
        {
            Get go = new Get();
            var names = await go.getNames();

            Scrape run = new Scrape();
            await run.Queries(names);
        }
    }
}
