using Arbitrage.DataGetters.Mozzart;
using Arbitrage.DataGetters.Meridian;
using Arbitrage.DataLoader;

using Match = Arbitrage.Utils.Match;

//TODO ukloniti meceve koji zahtevaju n+ mmeceva na tiketu MOZZART
DataLoader loaderMozzart = new DataLoader(new MozzartParser());
List<Match> matchesMozzart = loaderMozzart.GetMatches(DateTime.Now);

DataLoader loaderMeridian = new DataLoader(new MeridianParser());
List<Match> matchesMeridian = loaderMeridian.GetMatches(DateTime.Now);

var isti = matchesMozzart.Where(x => matchesMeridian.Any(y => y.Team1.Name.ToLower().Trim() == x.Team1.Name.ToLower().Trim()));

matchesMozzart = matchesMozzart.OrderBy(x => x.StartTime).ThenBy(x => x.Team1.Name).ToList();
matchesMeridian = matchesMeridian.OrderBy(x => x.StartTime).ThenBy(x => x.Team1.Name).ToList();

matchesMozzart.ForEach(x => File.AppendAllText("..\\..\\..\\Temp\\mozzartMatches.txt", x.ToString() + Environment.NewLine));
matchesMeridian.ForEach(x => File.AppendAllText("..\\..\\..\\Temp\\meridianMatches.txt", x.ToString() + Environment.NewLine));

int a = 5;
//var matcher = new MatchMatcher();

//matcher.MatchMatches();