using Arbitrage.DataGetters.MMOB;

namespace Arbitrage.DataGetters.BetOle
{
    internal class BetOleGetter : MMOBGetter
    {
        public override string GetMatchUrl(long matchId)
        {
            string url = "https://ibet2.betole.com/restapi/offer/sr/match/" + matchId + "?annex=0&mobileVersion=2.21.51&locale=sr";
            return url;
        }

        public override string GetSportUrl(string sport)
        {
            string url = "https://ibet2.betole.com/restapi/offer/sr/sport/" + sport + "/mob?annex=0&mobileVersion=2.21.51&locale=sr";
            return url;
        }
    }
}
