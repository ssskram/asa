# Active Shooter Archive

## Purpose
~~Capture~~ Attempt to capture the digital identity of an active shooter as soon as the individual is identified.

## Process

Whole kit and caboodle can run as a serverless function, or as a scheduled task

### program

Two functions:

1. ` Get() ` : the potential name(s) for the active shooter are picked out of news articles
2. ` Scrape() ` : the name(s) are used to scrape various sites & APIs

```csharp
static async Task Main()
{
    Get go = new Get();
    var names = await go.getNames();

    Scrape run = new Scrape();
    await run.Queries(names);
}
```
### get

` getNames() ` : main method that delegates tasks and returns list of plausible names to ` Main() `

```csharp
public async Task<object> getNames()
{
    // get relevant articles

        // for each relevant article, extract possible names
        var possiblenames = getPossibleNames(article);

            // for each possible name, return a score 
            int score = scorecard.scoreNames(pn.value, pn.index, article);

    // aggregate all possible names, and merge duplicates
    var an = aggregateNames(names);

    // pass names through geocoder to remove city names
    // then, take the top 3
    ws = await selectWinners(an);

    return(ws);
}
```

### score

` scoreNames() ` : main method that delegates tasks and returns a score for each name
```csharp
public int scoreNames(string name, int index, string article)
{
    string[] hot = new string[] {
        // keywords that are the most valuable in relation to name
    }
    string[] warm = new string[] {
        // keywords that are pretty valuable in relation to name
    }
    string[] warm = new string[] {
        // other word bits that always seem to get used in declarative sentences
    }
    
    // give 7 points for each hot word within 250 char. of name
    bool check = findDistance(index, x, article, "hot");

    // give 2 points for each warm word within 400 char. of name
    bool check = findDistance(index, x, article, "warm");

    // give 1 point for each cool word within 500 char. of name
    bool check = findDistance(index, x, article, "cool");

    return result;
}
```

### scrape
...

## Installation

### Prerequisites
1. [.net sdk](https://www.microsoft.com/net/learn/get-started/) 

### Installation
1. Clone this repository in some place `~/nice`
2. Open `~/nice` in the terminal
3. Restore dependencies
```bash
dotnet restore
```
4. Build & run
```bash
dotnet build
dotnet run
```

## Contributors
* Eric Charlton
* Rebecca Forstater
* Paul Marks
