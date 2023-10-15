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
        readonly IParser _parser;

        public DataLoader(IParser parser)
        {
            _parser = parser;
        }

        public MatchesData? GetMatches()
        {
            return _data;
        }

        public async Task Load() {
            Console.WriteLine(_parser.GetName() + " download started...");

            await UpdateData();
            Console.WriteLine(_parser.GetName() + " download complete");
        }

        private async Task UpdateData()
        {
            _data = _parser.Parse();

            // TODO update database
        }
    }
}
