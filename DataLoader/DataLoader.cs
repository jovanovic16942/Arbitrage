using Arbitrage.DataGetters;
using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataLoader
{
    internal class DataLoader
    {
        MatchesData? _data;

        IParser _parser;

        public DataLoader(IParser parser) {
            _parser = parser;    
        }

        public List<Match> GetMatches(DateTime? dateTime) {

            dateTime ??= DateTime.Now;

            UpdateData(dateTime.Value);

            return _data!.GetMatches().Where(x => x.StartTime.Date == dateTime.Value.Date).ToList();
        }

        private void UpdateData(DateTime dateTime)
        {
            _data = _parser.GetMatches(dateTime);

            // TODO update database
        }
    }
}
