using Arbitrage.General;
using Arbitrage.Utils;
using Microsoft.IdentityModel.Tokens;
using NLog;

namespace Arbitrage.DataLoader
{
    internal class DataLoader
    {
        Dictionary<Sport, List<HouseMatchData>> _data;
        readonly IParser _parser;

        private static Logger log = LogManager.GetCurrentClassLogger();

        public DataLoader(BettingHouse house)
        {
            _parser = ParserFactory.GetParser(house);

            _data = new();

            foreach (Sport sport in Enum.GetValues(typeof(Sport)))
            {
                _data.Add(sport, new List<HouseMatchData>());
            }

            log.Info("Created DataLoader for {@house}", house);
        }

        public List<HouseMatchData> GetData()
        {
            if (_data.IsNullOrEmpty())
            {
                log.Error("Data is null or empty!");
                return new();
            }

            List<HouseMatchData> data = new();

            foreach (var d in _data.Values) 
            {
                data.AddRange(d);
            }

            return data;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sports">List of sports to load</param>
        /// <param name="p">Load all sports in parallel if true</param>
        /// <returns></returns>
        public async Task Load(List<Sport> sports = null, bool p = false) {

            if (sports is null)
            {
                var values = Enum.GetValues(typeof(Sport));
                sports = new List<Sport>((Sport[])values);
            }

            Console.WriteLine(_parser.GetName() + " download started...");
            await UpdateData(sports, p);
            Console.WriteLine(_parser.GetName() + " download complete");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sports">List of sports to load</param>
        /// <param name="p">Load all sports in parallel if true</param>
        /// <returns></returns>
        private async Task UpdateData(List<Sport> sports, bool p = false)
        {
            List<Sport> _s = new();

            if (p)
            {
                Parallel.ForEach(sports, sport =>
                {
                    _ = UpdateSport(sport);
                });
            } else
            {
                foreach (var sport in sports)
                {
                    _ = UpdateSport(sport);
                }
            }
        }

        private async Task UpdateSport(Sport sport)
        {
            Console.WriteLine(_parser.GetName() + " " + sport + " download started...");
            _data[sport] = _parser.ParseSport(sport);
            Console.WriteLine(_parser.GetName() + " " + sport + " download complete");
        }
    }
}
