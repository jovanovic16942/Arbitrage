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
    }


}
