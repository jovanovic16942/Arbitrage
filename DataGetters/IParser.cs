using Arbitrage.Utils;
using System.Xml.Linq;

namespace Arbitrage.DataGetters
{
    public interface IParser
    {
        MatchesData GetMatches(DateTime dateTime);
    }
}