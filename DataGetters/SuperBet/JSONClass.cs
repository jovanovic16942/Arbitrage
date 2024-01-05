using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.SuperBet
{
    internal class JSONClass
    {
    }

    public class JsonMatchesResponse
    {
        public List<JsonMatch> data;
    }

    public class JsonMatch
    {
        public int _id;
        public string md; //datetime
        public string mld; //datetime2 prob local datetime
        public string mn; //Participant1 and Participant2 names separated by ·
        public string msn; //Participants with short names
        public string uuid;
        public int si; //id_str sporta - f(5) , k(4)
        public object cnts;
        public object mtd;
        public List<JsonOdd> odds;
    }

    public class JsonOdd
    {
        public string uuid;
        public int bgi;
        public int bgdi;
        public int oi;
        public double ov;
        public int oof;
        public string oc;
        public string tags;
        public JsonSpc spc;
    }

    public class JsonSpc
    {
        public double total;
    }

    public class JsonMatchResponse
    {
        public List<JsonMatchData> data;
    }

    public class JsonMatchData
    {
        public string matchName;
        public string matchDate;
        public List<JsonOddData> odds;
        public int sportId;
        public Dictionary<int, string> offerStateStatus;
    }

    public  class JsonOddData
    {
        public string code;
        public string info;
        public int marketId;
        public string name;
        public string marketName;
        public double price;
        public string specialBetValue;
        public int outcomeId;
        public JsonSpec specifiers;
    }

    public class JsonSpec
    {
        public string quarternr;
        public string total;
    }
}
