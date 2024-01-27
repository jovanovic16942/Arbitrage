using Arbitrage.DataGetters.MMOB;
using Arbitrage.General;

namespace Arbitrage.DataGetters.OktagonBet
{
    internal class OktagonBetParser : MMOBParser
    {
        public OktagonBetParser() : base(BettingHouse.OktagonBet, new OktagonBetGetter()) { }
    }
}
