using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.SoccerBet
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
        public long kickOffTime;
        public Dictionary<int, Dictionary<string, JsonBetData>> betMap { get; set; }
    }

    public class JsonBetData
    {
        public int bpc { get; set; }
        public int tt { get; set; }
        public string s { get; set; }
        public double ov { get; set; }
        public int bc { get; set; }
        public string sv { get; set; }
    }
}
