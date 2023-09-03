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
        // 
        //public List<Game> Games { get; set; }

        public Dictionary<string, double> SubGames { get; } = new Dictionary<string, double>();

        //public FootballMatch()

        // TODO izbaci ovaj constructor, logika u mozart parer
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

        public void AddSubGame(string name, double value)
        {
            SubGames.TryAdd(name, value);
            //SubGames.Add(name, value);
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

        public void UpdateMatchSubgame(int matchID, string subgameName, double value)
        {
            var match = Matches.FirstOrDefault(x => x.MatchId == matchID);

            if (match == null) return;

            match.AddSubGame(subgameName, value);
        }

    }

    //MatchesData() -> Matches -> foodbal
    //MatchesData.parse(mozzartGetter)



}
