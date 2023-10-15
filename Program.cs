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

// TODO Skidanje ostalih kvota sa meridijana !!!

//var log = Logger.GetLogger();

//log.LogDebug("This is a debug");
//log.LogInformation("This is an information");
//log.LogWarning("This is a warning");
//log.LogError("This is an error");
//log.LogCritical("This is a critical");

//var a = 1 + 2;


Console.WriteLine("Downloading Mozzart...");
DataLoader loaderMozzart = new DataLoader(new MozzartParser());
MatchesData mozzartData = loaderMozzart.GetMatches(DateTime.Now);
Console.WriteLine("Mozzart complete");

Console.WriteLine("Downloading Meridian...");
DataLoader loaderMeridian = new DataLoader(new MeridianParser());
MatchesData meridianData = loaderMeridian.GetMatches(DateTime.Now);
Console.WriteLine("Meridian complete");


Console.WriteLine("Downloading MaxBet...");
DataLoader loaderMaxBet = new DataLoader(new MaxBetParser());
MatchesData maxbetData = loaderMaxBet.GetMatches(DateTime.Now);
Console.WriteLine("MaxBet complete");




var unmatched = new List<MatchesData>
{
    mozzartData,
    meridianData,
    maxbetData,
};

var matched = MatchMatcher.MatchMatches(unmatched);

var success = matched.Where(x => x.odds.Count > 1).ToList();

var extraSucess = matched.Where(x => x.odds.Count > 2).ToList();

//var fail = matched.Where(x => x.odds.Count <= 1).ToList();


var arb = new ArbitrageCalculator();

arb.GetResults(success);

int a = success.Count;