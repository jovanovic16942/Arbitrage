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

    public class JsonMatchResponse
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
}
