using Arbitrage.Utils;
using NLog;

namespace Arbitrage.General
{
    /// <summary>
    /// Parses data from specific betting house
    /// </summary>
    public abstract class Parser : IParser
    {
        // TODO for all parsers - check if responses are null everywhere - handle exceptions and runtime errors

        /// <summary>
        /// DEPRECATED
        /// </summary>
        protected MatchesData _data;

        protected List<HouseMatchData> _parsedData = new();

        public BettingHouse House { get; }

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public Parser(BettingHouse house)
        {
            _data = new MatchesData(house);
            House = house;
            logger.Info(House.ToString() + "Parser created!");
        }

        public List<HouseMatchData> ParseSport(Sport sport)
        {
            switch (sport)
            {
                case Sport.Football:
                    ParseFootball();
                    break;
                case Sport.Basketball:
                    ParseBasketball();
                    break;
                default: break;
            }

            return _parsedData.Where(x => x.sport == sport).ToList();
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="sport"></param>
        /// <returns></returns>
        public MatchesData Parse(Sport sport)
        {

            switch (sport)
            {
                case Sport.Football:
                    ParseFootball();
                    break;
                case Sport.Basketball:
                    ParseBasketball();
                    break;
                default: break;
            }

            //try
            //{
            //    UpdateData();
            //} catch (Exception ex)
            //{
            //    // TODO serious logging
            //}

            return _data;
        }

        /// <summary>
        /// Override these methods in subclasses
        /// </summary>
        protected virtual void ParseBasketball()
        {
            throw new NotImplementedException();
        }

        protected virtual void ParseFootball()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return House.ToString();
        }
        /// <summary>
        /// Deprecated
        /// </summary>
        protected virtual void UpdateData()
        {
            throw new NotImplementedException();
        }

    }
}
