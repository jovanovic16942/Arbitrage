using Arbitrage.DataGetters.Mozzart;
using Arbitrage.DataGetters.Meridian;
using Arbitrage.DataLoader;

using Arbitrage.MatchMatcher;
using Arbitrage.ArbitrageCalculator;
using Arbitrage.DataGetters.MaxBet;
using Arbitrage.DataGetters.AdmiralBet;
using Arbitrage.DataGetters.SoccerBet;

// TODO Skidanje ostalih kvota sa meridijana !!!
// TODO LOGGING

int proba = 213;
proba += proba;

// Prepare data loaders
List<DataLoader> dataLoaders = new()
{
    new DataLoader(new MozzartParser()),
    new DataLoader(new MeridianParser()),
    new DataLoader(new MaxBetParser()),
    new DataLoader(new AdmiralBetParser()),
    new DataLoader(new SoccerBetParser())
};

// Load the data in parallel
Parallel.ForEach(dataLoaders, loader =>
{
    _ = loader.Load();
});

// Match data from different sources
var unmatched = dataLoaders.Select(x => x.GetMatches()).Where(x => x != null).ToList();
var matched = MatchMatcher.MatchMatches(unmatched!);
var success = matched.Where(x => x.odds.Count > 1).ToList();

// Get betting advice
var arb = new ArbitrageCalculator();
var res = arb.GetResults(success);

ArbitrageCalculator.PrintAllCombinations(res);