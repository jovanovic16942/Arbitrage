using Arbitrage.General;
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
        List<HouseMatchData>? _data;
        readonly IParser _parser;

        public DataLoader(BettingHouse house)
        {
            _parser = ParserFactory.GetParser(house);
        }

        public List<HouseMatchData> GetData()
        {
            if (_data == null) throw new InvalidOperationException("Invalid call to GetData() - _data is null. Call Load() first.");
            return _data;
        }

        public async Task Load(Sport sport) {
            Console.WriteLine(_parser.GetName() + " download started...");
            await UpdateData(sport);
            Console.WriteLine(_parser.GetName() + " download complete");
        }

        private async Task UpdateData(Sport sport)
        {
            _data = _parser.ParseSport(sport);
            // TODO update database
        }
    }
}
