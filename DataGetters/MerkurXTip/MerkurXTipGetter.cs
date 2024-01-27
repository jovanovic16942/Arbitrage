using Arbitrage.DataGetters.MMOB;

namespace Arbitrage.DataGetters.MerkurXTip
{
    public class MerkurXTipGetter : MMOBGetter
    {
        public MerkurXTipGetter() { }

        public override string GetSportUrl(string sport)
        {
            string url = "https://www.merkurxtip.rs/restapi/offer/sr/sport/" + sport + "/mob?annex=0&desktopVersion=2.24.46&locale=sr";
            return url;
        }

        public override string GetMatchUrl(long matchId)
        {
            string url = "https://www.merkurxtip.rs/restapi/offer/sr/match/" + matchId + "?annex=0&desktopVersion=1.31.5&locale=sr";
            return url;
        }
    }
}
