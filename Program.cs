using Arbitrage.DataGetters.Mozzart;
using Arbitrage.DataGetters.Meridian;
using Arbitrage.DataLoader;

using Arbitrage.MatchMatcher;
using Arbitrage.ArbitrageCalculator;
using Arbitrage.DataGetters.MaxBet;
using Arbitrage.DataGetters.AdmiralBet;
using Arbitrage.DataGetters.SoccerBet;
using Arbitrage.DataGetters.MerkurXTip;
using Arbitrage.General;
using Arbitrage.DataGetters.Bet365;
using Arbitrage.DataGetters.Pinnbet;
using System.ComponentModel.Design;
using Arbitrage.DataGetters.SuperBet;
using Arbitrage.DataGetters.BalkanBet;
using Arbitrage.DataGetters.StarBet;

void EstimateProfit(int weeklyBets, int numMonths, int numInvestments, double investAmount, double initialSum, double profitPerTransaction)
{
    double total = initialSum;
    double totalInvest = initialSum;

    int betsPerMonth = weeklyBets * 4;

    for (int i = 0; i < betsPerMonth * numMonths; i++)
    {
        if (i % betsPerMonth == 0 && numInvestments-- > 0) 
        {
            total += investAmount;
            totalInvest += investAmount;
        }

        total += total * profitPerTransaction / 100;

        if (i % betsPerMonth == 0)
        {
            Console.WriteLine("Invest: " + totalInvest);
            Console.WriteLine("Return: " + total);
            Console.WriteLine();
        }
    }
}

// TODO Skidanje ostalih kvota sa meridijana !!!
// TODO LOGGING
//EstimateProfit(3, 12, 6, 200, 2000, 3);


// Prepare data loaders
List <DataLoader> dataLoaders = new()
{
    new DataLoader(new MozzartParser()),
    new DataLoader(new MeridianParser()),
    new DataLoader(new MaxBetParser()),
    new DataLoader(new AdmiralBetParser()),
    new DataLoader(new SoccerBetParser()),
    new DataLoader(new MerkurXTipParser()),
    new DataLoader(new PinnBetParser()),
    new DataLoader(new SuperBetParser()),
    new DataLoader(new BalkanBetParser()),
    new DataLoader(new StarBetParser()),
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

var x = matched.MaxBy(x => x.odds.Count);

// Get betting advice
var arb = new ArbitrageCalculator();
var res = arb.GetResults(success);
var best = res.Where(x => x.profit > 0.01).ToList();

//ArbitrageCalculator.PrintCombinations(arb.GetBetList());
//ArbitrageCalculator.ShowStakes(arb.GetBetList(), 10000);


var a = 2;


