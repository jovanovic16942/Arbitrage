using Arbitrage.DataGetters.MMOB;
using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;

namespace Arbitrage.DataGetters.SoccerBet
{
    internal class SoccerBetGetter : MMOBGetter
    {
        public override string GetMatchUrl(long matchId)
        {
            string url = "https://www.soccerbet.rs/restapi/offer/sr/match/" + matchId + "?annex=0&mobileVersion=2.21.51&locale=sr";
            return url;
        }

        public override string GetSportUrl(string sport)
        {
            string url = "https://www.soccerbet.rs/restapi/offer/sr/sport/" + sport + "/mob?annex=0&mobileVersion=2.21.51&locale=sr";
            return url;
        }
    }
    //public class SoccerBetGetter
    //{
    //    public SoccerBetGetter() { }
    //    //TODO proveriti da li se ako se salje za svaku ligu posebno dobije vise rezultata
    //    public JsonMatchResponse GetMatches()
    //    {
    //        //string url = "https://www.soccerbet.rs/restapi/offer/sr/sport/S/mob?annex=0&desktopVersion=2.24.46&locale=sr";
    //        string url = "https://www.soccerbet.rs/restapi/offer/sr/sport/" + "S" + "/mob?annex=0&mobileVersion=2.21.51&locale=sr";

    //        var client = new RestClient(url);

    //        var request = new RestRequest("", Method.Get);

    //        RestResponse response = client.Execute(request);

    //        JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

    //        return matchResponse;
    //    }

    //    //public void GetMatchesInLeague(string leagueId)
    //    //{
    //    //    string url = "https://www.soccerbet.rs/restapi/offer/sr/sport/S/league-group/" + leagueId + "/mob?annex=0&desktopVersion=2.24.46&locale=sr";
    //    //}
    //}
}
