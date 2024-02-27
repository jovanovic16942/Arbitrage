using Arbitrage.DataGetters.SoccerBet;
using Newtonsoft.Json;

namespace Arbitrage.DataGetters.MMOB
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

        //for soccerbet
        public Dictionary<int, Dictionary<string, JsonBetData>> betMap { get; set; }

        [JsonProperty("params")]
        public JsonBetParams betParams;
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

    public class JsonBetParams
    {
        public string awayOverUnderFirstHalf { get; set; }
        public string awayOverUnderFirstPeriod { get; set; }
        public string awayOverUnderOvertime { get; set; }
        public string handicapFirstPeriod { get; set; }
        public string handicapFourthPeriod { get; set; }
        public string handicapOvertime { get; set; }
        public string handicapOvertime3 { get; set; }
        public string handicapOvertime4 { get; set; }
        public string handicapOvertime5 { get; set; }
        public string handicapOvertime6 { get; set; }
        public string handicapSecondPeriod { get; set; }
        public string handicapThirdPeriod { get; set; }
        public string hd2wp1 { get; set; }
        public string hd2wp2 { get; set; }
        public string homeOverUnderFirstHalf { get; set; }
        public string homeOverUnderFirstPeriod { get; set; }
        public string homeOverUnderOvertime { get; set; }
        public string homeOverUnderOvertime2 { get; set; }
        public string overUnderFirstPeriod { get; set; }
        public string overUnderFourthPeriod { get; set; }
        public string overUnderHandicapOvertime { get; set; }
        public string overUnderMBTOT1 { get; set; }
        public string overUnderOvertime { get; set; }
        public string overUnderOvertime2 { get; set; }
        public string overUnderOvertime3 { get; set; }
        public string overUnderOvertime4 { get; set; }
        public string overUnderOvertime5 { get; set; }
        public string overUnderOvertime6 { get; set; }
        public string overUnderOvertime7 { get; set; }
        public string overUnderP { get; set; }
        public string overUnderSecondHalf { get; set; }
        public string overUnderSecondPeriod { get; set; }
        public string overUnderThirdPeriod { get; set; }
    }
}
