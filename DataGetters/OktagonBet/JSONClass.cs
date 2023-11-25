using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.OktagonBet
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
        public long id;
        public string home;
        public string away;
        public long kickOffTime;
        public Dictionary<int, double> odds { get; set; }
    }
}
