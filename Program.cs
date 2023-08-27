using Arbitrage.DataGetters;
using Arbitrage.DataLoader;
using Arbitrage.EntityFramework;
using Arbitrage.EntityFramework.Models;
using System.Runtime.InteropServices;


//var getter = new MozzartGetter();
//getter.GetMatches(null);
//getter.GetOdds();

//int numDays = 7;

//for (int i = 0; i < numDays; i++)
//{
//    var date = DateTime.Today.AddDays(i + 1);
//    Console.WriteLine(date);
//    getter.GetMatches(date);
//    getter.InsertTeams();
//    Thread.Sleep(2000);
//}

//var db = new ArbitrageDb.Instance();

//var counb = db.Context.Teams.ToList();

//db.InsertTeam(new Team { Name = "Zimbabwe", ShortName = "TT"});

//getter.GetOdds();

DataLoader dataLoader = new DataLoader();

List<Match> matches = dataLoader.GetMatches(DateTime.Now);

foreach (Match match in matches)
{
    Console.WriteLine(match.ToString());
}

// TODO
/*
 * Refactoring getter wip
 * 
 * */