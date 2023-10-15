using Arbitrage.Utils;
using System.Xml.Linq;

namespace Arbitrage.DataGetters
{
    public interface IParser
    {
        /// <summary>
        /// This method uses DataGetter internally and parses resulting JSON data
        /// </summary>
        /// <returns>MatchesData structure that contains list of matches</returns>
        MatchesData Parse();

        /// <summary>
        /// Get betting house name
        /// </summary>
        /// <returns>string name of current betting house</returns>
        string GetName();
    }
}