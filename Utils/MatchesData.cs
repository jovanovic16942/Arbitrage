using Arbitrage.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Utils
{
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
    }

    public class SubGame
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public string Description { get; set; }
    }

    public class Game
    {
        public int Id { get; set; }
        public List<SubGame> SubGames { get; set; }
    }

    public class Match
    {
        public int MatchId { get; set; }

        public Participant Team1 { get; set; }
        public Participant Team2 { get; set; }
        public DateTime StartTime { get; set; }
        
        public Dictionary<BettingGames, double> BetGames { get; } = new Dictionary<BettingGames, double>();


        // TODO izbaci ovaj constructor, logika u mozart parser
        public Match(int matchId, long startTimeMilis, Participant participant1, Participant participant2)
        {
            MatchId = matchId;

            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            StartTime = unixEpoch.AddMilliseconds(startTimeMilis).AddHours(2);

            Team1 = participant1;
            Team2 = participant2;

        }

        public Match(DateTime startTime, Participant participant1, Participant participant2)
        {
            MatchId = 0; // beskorisno za sad
            StartTime = startTime;

            Team1 = participant1;
            Team2 = participant2;

        }

        public void AddBetGame(BettingGames game, double value)
        {
            BetGames.Add(game, value);
        }

        public override string ToString()
        {
            return string.Format("{0} vs {1} - {2}",
                Team1.Name, Team2.Name, StartTime);
        }

    }

    /// <summary>
    /// Data obtained from a single source
    /// </summary>
    public class MatchesData
    {
        public BettingHouses bettingHouse;
        private List<Match> Matches { get; }

        public MatchesData(BettingHouses bettingHouse)
        {
            this.bettingHouse = bettingHouse;
            Matches = new List<Match>();
        }
        public List<Match> GetMatches()
        {
            return Matches;
        }

        public void Insert(Match match)
        {
            Matches.Add(match);
        }

        public void InsertRange(List<Match> newMatches) 
        { 
            Matches.AddRange(newMatches);
        }

        public void UpdateMatchSubgame(int matchID, BettingGames betGame, double value)
        {
            var match = Matches.FirstOrDefault(x => x.MatchId == matchID);

            if (match == null) return;

            match.AddBetGame(betGame, value);
        }

    }

    //MatchesData() -> Matches -> foodbal
    //MatchesData.parse(mozzartGetter)



}
