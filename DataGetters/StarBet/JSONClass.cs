using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.StarBet
{
    internal class JSONClass
    {
    }

    public class JsonLeagueResponse
    {
        public List<JsonLeague> L;
        public string SN; //sportName
    }

    public class JsonLeague
    {
        public int LID;
        public string LN;
    }

    public class JsonLeagueDataResponse
    {
        public List<JsonPair> P;
    }

    public class JsonPair
    {
        public long PID;
        public string PN;
        public string DI;
    }

    public class JsonOddsResponse
    {
        public string IgraNaziv;
        public int ID;
        public int SID;
        public List<JsonOdd> T;
    }

    public class JsonOdd
    {
        public long ID;
        public double Kvota;
        public string TipOpisMK;
        public string TipPecatiWeb;
    }
}
