# Active Shooter Archive

## Purpose
~~Capture~~ Attempt to capture the digital identity of an active shooter as soon as the individual is identified.

## Process

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
        var possiblenames = getPossibleNames(article)

            // for each possible name, return a score 
            int score = scorecard.scoreNames(pn.value, pn.index, article)

    // aggregate all possible names, and merge duplicates
    var an = aggregateNames(names);

    // pass names through geocoder to remove city names
    // then, take the top tier
    ws = await selectWinners(an);

    return(ws)
}
```

### score

` scoreNames() ` : main method that delegates tasks and returns a score for each name

```csharp
public int scoreNames(string name, int index, string article)
{
    // collect points here
    int score;

    // return keywords that are most valuable in relation to name
    string[] hot = getHot();
    // return keywords that are pretty valuable in relation to name
    string[] warm = getWarm();  
    // return additional keywords to filter names against
    string[] cold = getCold();         

    // filter out names against exact matches of name & keywords

    // split name into it's comoosite parts, "name_parts"
        
        // for every instance of name_part
        int[] occ_np = allIndexes(article, p);

            // find distance between every instance of hot word
            int[] occ_hot = allIndexes(article, x);

                // if distance is < 75 characters, add 3 points

            // find distance between every instance of warm word
            int[] occ_hot = allIndexes(article, x);

                // if distance is < 150 characters, add 1 points

    return score;
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
