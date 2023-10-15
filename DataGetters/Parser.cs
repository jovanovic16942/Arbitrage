using Arbitrage.General;
using Arbitrage.Utils;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Arbitrage.DataGetters
{
    /// <summary>
    /// Parses data from specific betting house
    /// </summary>
    public abstract class Parser : IParser
    {
        // TODO for all parsers - check if responses are null everywhere - handle exceptions and runtime errors
        protected MatchesData _data;

        public BettingHouses House { get; }

        public Parser(BettingHouses house)
        {
            _data = new MatchesData(house);
            House = house;
        }

        public MatchesData Parse()
        {
            UpdateData();

            return _data;
        }
        public string GetName()
        {
            return House.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        protected abstract void UpdateData();

    }
}
