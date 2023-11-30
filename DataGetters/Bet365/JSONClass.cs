using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Bet365
{
    internal class JSONClass
    {
    }

    public class JsonMatchesResponse
    {
        public List<JsonMatch> matches;
    }

    public class JsonSport
    {
        public string name;
        public string sportType;
        public List<JsonLeague> leagues;
    }

    public class JsonLeague
    {
        public long betLeagueId;
        public string name;
        public int numOfMatches;
    }

    public class JsonLeagueMatchesResponse
    {
        public List<JsonMatch> matchList;
    }

    public class JsonMatch
    {
        public long id;
        public long leagueId;
        public string home;
        public string away;
        public long kickOffTime;
        public string sport;
        public List<JsonBetGroup> odBetPickGroups;
    }

    public class JsonBetGroup
    {
        public long id;
        public string name;
        public List<JsonBet> tipTypes;
    }

    public class JsonBet
    {
        public string caption;
        public string name;
        public double value;
        public int tipTypeId;
        public string description;
    }
}
