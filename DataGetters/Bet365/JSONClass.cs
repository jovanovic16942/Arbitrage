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
