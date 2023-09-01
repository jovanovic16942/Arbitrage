using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.General
{
    public class GameCounts
    {
        public int Total { get; set; }
        public int Count1 { get; set; }
        public int Count2 { get; set; }
        // Add more properties as necessary
    }

    public class Participant
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }

    public class GroupationItem
    {
        public int GroupationId { get; set; }
        public int Rank { get; set; }
    }

    public class PriorityByGroupation
    {
        public List<GroupationItem> OrderItems { get; set; }
    }

    public class Sport
    {
        public int Id { get; set; }
        public PriorityByGroupation PriorityByGroupation { get; set; }
    }

    public class Competition
    {
        public int Id { get; set; }
        public int Priority { get; set; }
        public PriorityByGroupation PriorityByGroupation { get; set; }
        public string Name { get; set; }
        public Sport Sport { get; set; }
    }

    public class Match
    {
        public string CompetitionComment_sr { get; set; }
        public int Id { get; set; }
        public long StartTime { get; set; }
        public int SpecialType { get; set; }
        public string Competition_name_sr { get; set; }
        public int MatchNumber { get; set; }
        public GameCounts GameCounts { get; set; }
        public int CountKodds { get; set; }
        public int OddsCount { get; set; }
        public List<Participant> Participants { get; set; }
        public string MainMatch { get; set; }
        public Competition Competition { get; set; }
    }

    public class MatchResponse
    {
        public List<Match> Matches { get; set; }
        public int Total { get; set; }
    }

    public class SubGame
    {
        public int id { get; set; }
        public int subGameId { get; set; }
        public int gameId { get; set; }
        public string gameName { get; set; }
        public string subGameName { get; set; }
        public string gameShortName { get; set; }
        public string subGameDescription { get; set; }
        public string specialOddValueType { get; set; }
        public bool priority { get; set; }
    }

    public class Kodds
    {
        public int id { get; set; }
        public string specialOddValue { get; set; }
        public int matchId { get; set; }
        public string value { get; set; }
        public string winStatus { get; set; }
        public SubGame subGame { get; set; }
    }

    public class Root
    {
        public int id { get; set; }
        public Dictionary<string, Kodds> kodds { get; set; }
    }

 

}
