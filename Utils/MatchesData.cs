using Arbitrage.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Utils
{
    /// <summary>
    /// DEPRECATED
    /// </summary>
    public class Participant
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Description { get; set; }

        public Participant(int id, string name, string shortname, string description)
        {
            Id = id;
            Name = name;
            ShortName = shortname;
            Description = description;
        }
        public Participant(string name)
        {
            Id = 0;
            Name = name;
            ShortName = name;
            Description = "";
        }
    }

    /// <summary>
    /// DEPRECATED
    /// </summary>
    public class Match
    {
        public Sport sport { get; set; }
        public int matchId { get; set; }

        public Participant team1 { get; set; }
        public Participant team2 { get; set; }
        public DateTime startTime { get; set; }
        
        public Dictionary<BettingGames, double> betGames { get; } = new Dictionary<BettingGames, double>();


        /// <summary>
        /// This constructor is used by MozzartParser, because matchID from json response needs to be saved
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="startTime"></param>
        /// <param name="participant1"></param>
        /// <param name="participant2"></param>
        public Match(int matchId, DateTime startTime, Participant participant1, Participant participant2)
        {
            this.matchId = matchId;
            this.startTime = startTime;

            team1 = participant1;
            team2 = participant2;

        }

        public Match(DateTime startTime, Participant participant1, Participant participant2, Sport sport = Sport.Football)
        {
            matchId = 0; // beskorisno za sad
            this.startTime = startTime;

            team1 = participant1;
            team2 = participant2;

            this.sport = sport;
        }

        public void AddBetGame(BettingGames game, double value)
        {
            betGames.Add(game, value);
        }

        public void TryAddBetGame(BettingGames game, double value)
        {
            betGames.TryAdd(game, value);
        }

        public override string ToString()
        {
            return string.Format("{0} vs {1} - {2}",
                team1.Name, team2.Name, startTime);
        }

    }

    /// <summary>
    /// DEPRECATED
    /// Data obtained from a single source
    /// </summary>
    public class MatchesData
    {
        public BettingHouse bettingHouse;
        private List<Match> matches { get; }

        public MatchesData(BettingHouse bettingHouse)
        {
            this.bettingHouse = bettingHouse;
            matches = new List<Match>();
        }
        public List<Match> GetMatches()
        {
            return matches;
        }

        public void Insert(Match match)
        {
            matches.Add(match);
        }

        public void InsertRange(List<Match> newMatches) 
        { 
            matches.AddRange(newMatches);
        }

        public void UpdateMatchSubgame(int matchID, BettingGames betGame, double value)
        {
            var match = matches.FirstOrDefault(x => x.matchId == matchID);

            if (match == null) return;

            match.AddBetGame(betGame, value);
        }

        public double GetSubgameValue(int matchID, BettingGames betGame)
        {
            var match = matches.FirstOrDefault(x => x.matchId == matchID);

            if (match == null) return 0;

            return match.betGames[betGame];
        }
    }

    //MatchesData() -> matches -> foodbal
    //MatchesData.parse(mozzartGetter)



}
