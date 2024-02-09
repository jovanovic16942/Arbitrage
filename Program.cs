using Arbitrage.DataLoader;
using Arbitrage.MatchMatcher;
using Arbitrage.ArbitrageCalculator;
using Arbitrage.General;

// TODO TOP BET

// TODO over under kvote proveriti gde se koriste bez overtimea

// Prepare data loaders
List <DataLoader> dataLoaders = new()
{
    //new DataLoader(new SoccerBetParser()),
    //new DataLoader(new BalkanBetParser()),
    //new DataLoader(new StarBetParser()),
    //new DataLoader(new OlimpParser()),
    //new DataLoader(new Bet365Parser()),
    new DataLoader(BettingHouse.Meridian), // IN PROGRESS
    new DataLoader(BettingHouse.PinnBet),
    new DataLoader(BettingHouse.Mozzart),
    new DataLoader(BettingHouse.AdmiralBet),
    new DataLoader(BettingHouse.SuperBet),
    new DataLoader(BettingHouse.MerkurXTip),
    new DataLoader(BettingHouse.MaxBet),
    new DataLoader(BettingHouse.OktagonBet),
    new DataLoader(BettingHouse.BetOle),
};

List<Sport> sportsToGet = new()
{
    Sport.Football,
    Sport.Basketball,
};

// Load the data in parallel
Parallel.ForEach(dataLoaders, loader =>
{
    _ = loader.Load(sportsToGet, false);
});

// Run MatchMatcher
var loadedData = dataLoaders.Select(x => x.GetData()).Where(x => x != null && x.Any()).ToList();
var allEvents = MatchMatcher.MatchMatches(loadedData!);
var matchedEvents = allEvents.Where(x => x.data.Count > 1).ToList();

var bestMatched = allEvents.MaxBy(x => x.data.Count);

// Run Arbitrage
var arb = new ArbitrageCalculator();
arb.ProcessResults(matchedEvents);
var results = matchedEvents.Where(x => x.combinations.Any()).OrderByDescending(x => x.combinations.First().profit);

var allCombos = new List<Combination>();
foreach(var s in results)
{
    allCombos.AddRange(s.combinations);
}

var t0 = allCombos.Where(x => x.profit < 0.02).OrderByDescending(x => x.profit).ToList();
var t1 = allCombos.Where(x => x.profit >= 0.02 && x.profit < 0.05).OrderByDescending(x => x.profit).ToList();
var t2 = allCombos.Where(x => x.profit >= 0.05 && x.profit < 0.08).OrderByDescending(x => x.profit).ToList();
var t3 = allCombos.Where(x => x.profit >= 0.08).OrderByDescending(x => x.profit).ToList();


var BP = "BreakPoint";
// Debug stop
var stop = 0;