using Arbitrage.DataGetters.MMOB;
using Arbitrage.General;

namespace Arbitrage.DataGetters.MerkurXTip
{
    public class MerkurXTipParser : MMOBParser
    {
        public MerkurXTipParser() : base(BettingHouse.MerkurXTip, new MerkurXTipGetter()) { }
    }
}
