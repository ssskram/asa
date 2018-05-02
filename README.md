# Active Shooter Archive

## Purpose
~~Capture~~ Attempt to capture the digital identity of an active shooter as soon as the individual is identified.

## Process

### program

Two modules:
1. First, the name of an active shooter is picked up by continually scraping relevant news articles 
2. Second, the name is used to query various APIs & scrape verious sites

```csharp
[Authorize]
static async Task Main()
{
    Get go = new Get();
    var names = await go.getNames();

    Scrape run = new Scrape();
    await run.Queries(names);
}
```
### get
...
### score
...
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
