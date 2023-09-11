using Arbitrage.DataGetters;
using Arbitrage.Utils;
using Microsoft.Extensions.Logging;
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

        public MatchesData GetMatches(DateTime? dateTime) {

            dateTime ??= DateTime.Now;

            UpdateData(dateTime.Value);

            return _data!;
        }

        private void UpdateData(DateTime dateTime)
        {
            _data = _parser.GetMatches(dateTime);

            // TODO update database
        }
    }
}
