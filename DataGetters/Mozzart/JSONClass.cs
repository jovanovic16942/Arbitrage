using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.General
{
    public class JsonGameCounts
    {
        public int Total { get; set; }
        public int Count1 { get; set; }
        public int Count2 { get; set; }
        // Add more properties as necessary
    }

    public class JsonParticipant
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }

    public class JsonGroupationItem
    {
        public int GroupationId { get; set; }
        public int Rank { get; set; }
    }

    public class JsonPriorityByGroupation
    {
        public List<JsonGroupationItem> OrderItems { get; set; }
    }

    public class JsonSport
    {
        public int Id { get; set; }
        public JsonPriorityByGroupation PriorityByGroupation { get; set; }
    }

    public class JsonCompetition
    {
        public int Id { get; set; }
        public int Priority { get; set; }
        public JsonPriorityByGroupation PriorityByGroupation { get; set; }
        public string Name { get; set; }
        public JsonSport Sport { get; set; }
    }

    public class JsonMatch
    {
        public string CompetitionComment_sr { get; set; }
        public int Id { get; set; }
        public long StartTime { get; set; }
        public int SpecialType { get; set; }
        public string Competition_name_sr { get; set; }
        public int MatchNumber { get; set; }
        public JsonGameCounts GameCounts { get; set; }
        public int CountKodds { get; set; }
        public int OddsCount { get; set; }
        public List<JsonParticipant> Participants { get; set; }
        public string MainMatch { get; set; }
        public JsonCompetition Competition { get; set; }
    }

    public class JsonMatchResponse
    {
        public List<JsonMatch> Matches { get; set; }
        public int Total { get; set; }
    }

    public class JsonSubGame
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

    public class JsonKodds
    {
        public int id { get; set; }
        public string specialOddValue { get; set; }
        public int matchId { get; set; }
        public string value { get; set; }
        public string winStatus { get; set; }
        public JsonSubGame subGame { get; set; }
    }

    public class JsonRoot
    {
        public int id { get; set; }
        public Dictionary<string, JsonKodds> kodds { get; set; }
    }

 

}
