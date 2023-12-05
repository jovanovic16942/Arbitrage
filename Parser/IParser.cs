using Arbitrage.Utils;
using System.Xml.Linq;

namespace Arbitrage.General
{
    public interface IParser
    {
        /// <summary>
        /// This method uses DataGetter internally and parses resulting JSON data
        /// </summary>
        /// <returns>MatchesData structure that contains list of matches</returns>
        MatchesData Parse(Sport sport);

        /// <summary>
        /// Parse specified sport data.
        /// </summary>
        /// <param name="sport"></param>
        /// <returns>Parsed data in a list of HouseMatchData</returns>
        List<HouseMatchData> ParseSport(Sport sport);

        /// <summary>
        /// Get betting house name
        /// </summary>
        /// <returns>string name of current betting house</returns>
        string GetName();
    }
}