using Arbitrage.DataGetters.Mozzart;
using Arbitrage.DataGetters.Meridian;
using Arbitrage.DataLoader;

using Match = Arbitrage.Utils.Match;
using Arbitrage.Utils;
using Arbitrage.MatchMatcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Arbitrage.ArbitrageCalculator;
using Arbitrage.DataGetters.MaxBet;
using Arbitrage.DataGetters.Admiralbet;
using Arbitrage.DataGetters.AdmiralBet;

// TODO Skidanje ostalih kvota sa meridijana !!!


// TODO LOGGING
//var log = Logger.GetLogger();

//log.LogDebug("This is a debug");
//log.LogInformation("This is an information");
//log.LogWarning("This is a warning");
//log.LogError("This is an error");
//log.LogCritical("This is a critical");

//var a = 1 + 2;


// Prepare data loaders
List<DataLoader> dataLoaders = new List<DataLoader>()
{
    new DataLoader(new AdmiralBetParser(), "Admiral"),
    new DataLoader(new MozzartParser(), "Mozzart"),
    new DataLoader(new MeridianParser(), "Meridian"),
    new DataLoader(new MaxBetParser(), "MaxBet")
};

// Load the data in parallel
Parallel.ForEach(dataLoaders, loader =>
{
    loader.Load();
});

var unmatched = dataLoaders.Select(x => x.GetMatches()).Where(x => x != null).ToList();


var matched = MatchMatcher.MatchMatches(unmatched!);

var success = matched.Where(x => x.odds.Count > 1).ToList();

var arb = new ArbitrageCalculator();

var res = arb.GetResults(success);

int a = success.Count;

int b = success.Count;