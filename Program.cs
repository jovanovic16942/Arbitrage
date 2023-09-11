using Arbitrage.DataGetters.Mozzart;
using Arbitrage.DataGetters.Meridian;
using Arbitrage.DataLoader;

using Match = Arbitrage.Utils.Match;
using Arbitrage.Utils;
using Arbitrage.MatchMatcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

var log = Logger.GetLogger();

log.LogDebug("This is a debug");
log.LogInformation("This is an information");
log.LogWarning("This is a warning");
log.LogError("This is an error");
log.LogCritical("This is a critical");

var a = 1 + 2;

/*
DataLoader loaderMozzart = new DataLoader(new MozzartParser());
MatchesData mozzartData = loaderMozzart.GetMatches(DateTime.Now);

DataLoader loaderMeridian = new DataLoader(new MeridianParser());
MatchesData meridianData = loaderMeridian.GetMatches(DateTime.Now);

var matchesMozzart = mozzartData.GetMatches().OrderBy(x => x.StartTime).ThenBy(x => x.Team1.Name).ToList();
var matchesMeridian = meridianData.GetMatches().OrderBy(x => x.StartTime).ThenBy(x => x.Team1.Name).ToList();

matchesMozzart.ForEach(x => File.AppendAllText("..\\..\\..\\Temp\\mozzartMatches.txt", x.ToString() + Environment.NewLine));
matchesMeridian.ForEach(x => File.AppendAllText("..\\..\\..\\Temp\\meridianMatches.txt", x.ToString() + Environment.NewLine));

var matcher = new MatchMatcher();

//matcher.MatchMatches();

*/