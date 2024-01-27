using Arbitrage.DataGetters.MMOB;
using Arbitrage.General;

namespace Arbitrage.DataGetters.BetOle
{
    internal class BetOleParser : MMOBParser
    {
        public BetOleParser() : base(BettingHouse.BetOle, new BetOleGetter()) { }
    }
}
