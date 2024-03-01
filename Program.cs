using Arbitrage.DataLoader;
using Arbitrage.MatchMatcher;
using Arbitrage.ArbitrageCalculator;
using Arbitrage.General;

// TODO
// topbet
// efbet
// TODO over under kvote proveriti gde se koriste bez overtimea

// Prepare data loaders
List <DataLoader> dataLoaders = new()
{
    new(BettingHouse.BalkanBet),  // TODO basket/football odds
    new(BettingHouse.StarBet),  // TODO basket/football odds
    //new(BettingHouse.Olimp),  // TODO bugs basket/football odds
    new(BettingHouse.Bet365),  // TODO basket/football odds
    new(BettingHouse.SoccerBet), // TODO basket odds
    new(BettingHouse.Meridian),
    new(BettingHouse.PinnBet),
    new(BettingHouse.Mozzart),
    new(BettingHouse.AdmiralBet),
    new(BettingHouse.SuperBet),
    new(BettingHouse.MerkurXTip),
    new(BettingHouse.MaxBet),
    new(BettingHouse.OktagonBet),
    new(BettingHouse.BetOle),
};

List<Sport> sportsToGet = new()
{
    Sport.Football,
    Sport.Basketball,
};

// Load the data in parallel
/* TODO vidi sa mikicom: 
 * Mozda bi ovo vredelo prepakovati
 * 1. Load da vrati odma, on da startuje novi thread u kome se skidaju podaci
 * 2. Da zovemo drugi metod koji ceka i vraca podatke u kome mozemo podesiti timeout ako nesto zabode da vrati error
 * 
 */
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

var t0 = allCombos.Where(x => x.profit < 0.01).OrderByDescending(x => x.profit).ToList();
var t1 = allCombos.Where(x => x.profit >= 0.01 && x.profit < 0.04).OrderByDescending(x => x.profit).ToList();
var t2 = allCombos.Where(x => x.profit >= 0.04 && x.profit < 0.08).OrderByDescending(x => x.profit).ToList();
var t3 = allCombos.Where(x => x.profit >= 0.08).OrderByDescending(x => x.profit).ToList();


var BP = "BreakPoint";
// Debug stop
var stop = 0;