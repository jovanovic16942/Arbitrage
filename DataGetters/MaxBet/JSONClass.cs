using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.MaxBet
{
    public class JSONClass
    {

    }
    public class JsonLeaguesResponse
    {
        public List<JsonCategory> categories;
    }

    public class JsonCategory
    {
        public string id;
        public string name;
    }
}
