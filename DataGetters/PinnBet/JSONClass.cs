using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Pinnbet
{
    internal class JSONClass
    {
    }

    public class JsonMatchResponse
    {
        public List<JsonMatch> jsonMatches;
    }

    public class JsonMatch
    {
        public string homeTeamName;
        public string awayTeamName;
        public string matchStartTime;
        public string sportCode;
        public int eventId;
        public int roundId;
        public List<JsonSelection> selections;

    }

    public class JsonSelection
    {
        public string status;
        public double odds;
        public string marketCode;
        public string selectionCode;
        public string result;
    }
}
