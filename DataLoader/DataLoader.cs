using Arbitrage.General;
using Arbitrage.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog;
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

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public DataLoader(BettingHouse house)
        {
            _parser = ParserFactory.GetParser(house);
            logger.Info("Created DataLoader for {@house}", house);
        }

        public List<HouseMatchData> GetData()
        {
            if (!_data.IsNullOrEmpty())
            {
                return _data!;
            }

            logger.Error("Data is null or empty!");
            return new();
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
