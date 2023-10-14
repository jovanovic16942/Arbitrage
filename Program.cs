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

// TODO Skidanje ostalih kvota sa meridijana !!!

//var log = Logger.GetLogger();

//log.LogDebug("This is a debug");
//log.LogInformation("This is an information");
//log.LogWarning("This is a warning");
//log.LogError("This is an error");
//log.LogCritical("This is a critical");

//var a = 1 + 2;


DataLoader loaderMozzart = new DataLoader(new MozzartParser());
MatchesData mozzartData = loaderMozzart.GetMatches(DateTime.Now);


DataLoader loaderMeridian = new DataLoader(new MeridianParser());
MatchesData meridianData = loaderMeridian.GetMatches(DateTime.Now);


DataLoader loaderMozzart2 = new DataLoader(new MozzartParser());
MatchesData mozzartData2 = loaderMozzart2.GetMatches(DateTime.Now.AddDays(1));


mozzartData.InsertRange(mozzartData2.GetMatches());

//var matchesMozzart = mozzartData.GetMatches().OrderBy(x => x.StartTime).ThenBy(x => x.Team1.Name).ToList();
//matchesMozzart.AddRange(mozzartData2.GetMatches().OrderBy(x => x.StartTime).ThenBy(x => x.Team1.Name).ToList());

//var matchesMeridian = meridianData.GetMatches().OrderBy(x => x.StartTime).ThenBy(x => x.Team1.Name).ToList();

//matchesMozzart.ForEach(x => File.AppendAllText("..\\..\\..\\Temp\\mozzartMatches.txt", x.ToString() + Environment.NewLine));
//matchesMeridian.ForEach(x => File.AppendAllText("..\\..\\..\\Temp\\meridianMatches.txt", x.ToString() + Environment.NewLine));




//foreach (Match match in matchesMozzart)
//{
//    File.AppendAllText("..\\..\\..\\Temp\\mozzartTeams.txt", match.Team1.Name + Environment.NewLine);
//    File.AppendAllText("..\\..\\..\\Temp\\mozzartTeams.txt", match.Team2.Name + Environment.NewLine);
//}

//foreach (Match match in matchesMeridian)
//{
//    File.AppendAllText("..\\..\\..\\Temp\\meridianTeams.txt", match.Team1.Name + Environment.NewLine);
//    File.AppendAllText("..\\..\\..\\Temp\\meridianTeams.txt", match.Team2.Name + Environment.NewLine);
//}


var unmatched = new List<MatchesData>
{
    mozzartData,
    meridianData
};

var matched = MatchMatcher.MatchMatches(unmatched);

var success = matched.Where(x => x.odds.Count > 1).ToList();
//var fail = matched.Where(x => x.odds.Count <= 1).ToList();


var arb = new ArbitrageCalculator();

arb.GetResults(success);

int a = success.Count;