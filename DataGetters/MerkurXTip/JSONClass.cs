using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.MerkurXTip
{
    internal class JSONClass
    {
    }

    public class JsonMatchResponse
    {
        public List<JsonMatch> esMatches;
    }

    public class JsonMatch
    {
        public string home;
        public string away;
        public string leagueName;
        public long kickOffTime;
        public Dictionary<int, double> odds;
        public long id;
        public long matchCode;
        public string sport;
        public long leagueId;
        public bool superMatch;
    }
}
