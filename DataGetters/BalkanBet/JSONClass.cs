using Arbitrage.DataGetters.SoccerBet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.BalkanBet
{
    internal class JSONClass
    {
    }
    
    public class JsonMatchIdsResponse
    {
        public JsonData data;
    }

    public class JsonData
    {
        public List<JsonEvent> events;
    }

    public class JsonEvent
    {
        public long a;
    }

    public class JsonMatchResponse
    {
        public JsonMatchData data;
    }

    public class JsonMatchData
    {
        public List<JsonCompetitor> competitors;
        public int sportId;
        public string name;
        public string startsAt;
        public List<JsonMarket> markets; 
    }

    public class JsonCompetitor
    {
        public long id;
        public string name;
        public string shortName;
        public int type;
        public int teamId;
    }

    public class JsonMarket
    {
        public string name;
        public int marketId;
        public List<JsonOutcome> outcomes;
    }

    public class JsonOutcome
    {
        public long id;
        public string description;
        public string name;
        public double odd;
        public int outcomeId;
    }
}
