# Active Shooter Archive

## Purpose
~~Capture~~ Attempt to capture the digital identity of an active shooter as soon as the individual is identified.

## Process

### program

Two functions:
1. `Get()`csharp the name of an active shooter is picked up by continually scraping relevant news articles 
2. ``` Scrape() ``` the name is used to scrape various sites & APIs, and the data is dumped into an unstructured data store for sorting & retrieval

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
