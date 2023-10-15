using Arbitrage.EntityFramework.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.SoccerBet
{
    public class SoccerBetGetter
    {
        public SoccerBetGetter() { }
        //TODO proveriti da li se ako se salje za svaku ligu posebno dobije vise rezultata
        public JsonMatchResponse GetMatches()
        {
            string url = "https://www.soccerbet.rs/restapi/offer/sr/sport/S/mob?annex=0&desktopVersion=2.24.46&locale=sr";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }

        public void GetMatchesInLeague(string leagueId)
        {
            string url = "https://www.soccerbet.rs/restapi/offer/sr/sport/S/league-group/" + leagueId + "/mob?annex=0&desktopVersion=2.24.46&locale=sr";
        }
    }
}
