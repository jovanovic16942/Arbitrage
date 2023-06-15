using Arbitrage.DataGetters;
using Arbitrage.EntityFramework;
using Arbitrage.EntityFramework.Models;
using System.Runtime.InteropServices;


//var getter = new MozzartGetter();
//getter.GetMatches(null);

var db = new ArbitrageDb();

var counb = db.Context.Teams.ToList();

db.InsertTeam(new Team { Name = "TestTeam", ShortName = "TT"});

//getter.GetOdds();
