using Arbitrage.DataGetters.MMOB;

namespace Arbitrage.DataGetters.OktagonBet
{
    internal class OktagonBetGetter : MMOBGetter
    {
        public override string GetMatchUrl(long matchId)
        {
            string url = "https://www.oktagonbet.com/restapi/offer/sr/match/" + matchId + "?annex=1&mobileVersion=2.21.50&locale=sr";
            return url;
        }

        public override string GetSportUrl(string sport)
        {
            string url = "https://www.oktagonbet.com/restapi/offer/sr/sport/" + sport + "/mob?annex=1&mobileVersion=2.21.50&locale=sr";
            return url;
        }
    }
}
