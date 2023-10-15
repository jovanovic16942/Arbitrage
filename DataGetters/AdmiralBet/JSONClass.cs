using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.AdmiralBet
{
    internal class JSONClass
    {
        
    }

    public class JsonMatchResponse
    {
        public List<JsonCompetition> competitions;
    }

    public class JsonCompetition
    {
        public int competitionId;
        public string competitonName;

        public List<JsonEvent> events;
    }

    public class JsonEvent
    {
        public int id;
        public string name;
        public List<JsonBet> bets;
        public string dateTime;
        public bool isTopOffer;
    }

    public class JsonBet
    {
        public List<JsonBetOutcome> betOutcomes;
    }

    public class JsonBetOutcome
    {
        public double odd;
        public string sBV;
        public string name;
        public int betTypeOutcomeId;
    }
}
