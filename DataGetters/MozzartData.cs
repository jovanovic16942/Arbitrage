using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters
{
    internal class Participant
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

    internal class SubGame
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public string Descritpion { get; set; }
    }

    internal class Game
    {
        public int Id { get; set; }
        public List<SubGame> SubGames { get; set; }
    }

    internal class MozzartData
    {
         
        public int MatchId { get; set; }

        public DateTime StartTime { get; set; }

        public Participant Participant1 { get; set; }
        public Participant Participant2 { get; set; }

        public MozzartData(int matchId, long startTimeMilis, Participant participant1, Participant participant2)
        {
            MatchId = matchId;

            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            StartTime = unixEpoch.AddMilliseconds(startTimeMilis).AddHours(1);

            Participant1 = participant1;
            Participant2 = participant2;

        }

        public double participant1WinOdds { get; set; }
        public double XOdds { get; set; }
        public double participant2WinOdds { get; set; }

    }
}
