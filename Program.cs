using Arbitrage.DataGetters;
using Arbitrage.DataGetters.Meridian;
using Arbitrage.DataLoader;
using Arbitrage.EntityFramework;
using Arbitrage.EntityFramework.Models;
using Arbitrage.Utils;
using System.Runtime.InteropServices;


//DataLoader loaderMozzart = new DataLoader(new MozzartParser());

//List<Match> matchesMozzart = loaderMozzart.GetMatches(DateTime.Now);


//DataLoader loaderMeridian = new DataLoader(new MeridianParser());
//List<Match> matchesMeridian = loaderMeridian.GetMatches(DateTime.Now);

var parser = new MeridianParser();
parser.GetMatches(DateTime.Now);