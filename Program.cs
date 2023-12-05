using Arbitrage.DataLoader;
using Arbitrage.MatchMatcher;
using Arbitrage.ArbitrageCalculator;
using Arbitrage.General;


// TODO Skidanje ostalih kvota sa meridijana !!!
// TODO LOGGING
// TODO TOP BET

// Prepare data loaders
List <DataLoader> dataLoaders = new()
{
    //new DataLoader(new PinnBetParser()),  // TODO sad su nasli da rade update
    //new DataLoader(new MozzartParser()),
    //new DataLoader(new MeridianParser()),
    //new DataLoader(new MaxBetParser()),
    new DataLoader(BettingHouse.AdmiralBet),
    //new DataLoader(new SoccerBetParser()),
    //new DataLoader(new MerkurXTipParser()),
    new DataLoader(BettingHouse.SuperBet),
    //new DataLoader(new BalkanBetParser()),
    //new DataLoader(new StarBetParser()),
    //new DataLoader(new OktagonBetParser()),
    //new DataLoader(new BetOleParser()),
    //new DataLoader(new OlimpParser()),
    //new DataLoader(new Bet365Parser()),
};

// Load the data in parallel
Parallel.ForEach(dataLoaders, loader =>
{
    _ = loader.Load(Sport.Football);
});


// Match data from different sources
var unmatched = dataLoaders.Select(x => x.GetData()).Where(x => x != null && x.Any()).ToList();
var matched = MatchMatcher.MatchMatches(unmatched!);
var success = matched.Where(x => x.data.Count > 1).ToList();

var bestMatched = matched.MaxBy(x => x.data.Count);

// Get betting advice
var arb = new ArbitrageCalculator();
//var res = arb.GetResults(success);
//var best = res.Where(x => x.profit > 0.02).ToList();

//ArbitrageCalculator.PrintCombinations(arb.GetBetList());
//ArbitrageCalculator.ShowStakes(arb.GetBetList(), 10000);


var a = 2;


