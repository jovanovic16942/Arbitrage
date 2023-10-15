﻿using Arbitrage.DataGetters.Mozzart;
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
using Arbitrage.DataGetters.SoccerBet;

// TODO Skidanje ostalih kvota sa meridijana !!!
// TODO LOGGING

// Prepare data loaders
List<DataLoader> dataLoaders = new List<DataLoader>()
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

var unmatched = dataLoaders.Select(x => x.GetMatches()).Where(x => x != null).ToList();


var matched = MatchMatcher.MatchMatches(unmatched!);

var success = matched.Where(x => x.odds.Count > 1).ToList();

var arb = new ArbitrageCalculator();

var res = arb.GetResults(success);

int a = success.Count;

int b = success.Count;