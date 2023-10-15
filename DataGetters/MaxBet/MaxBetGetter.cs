using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.MaxBet
{
    public class MaxBetGetter
    {
        private List<JsonCategory> leagues = new List<JsonCategory>();

        public MaxBetGetter() { }

        public JsonMatchResponse GetMatches(string leagueId)
        {
            Thread.Sleep(Constants.SleepTime);

            string url = "https://www.maxbet.rs/restapi/offer/sr/sport/S/league/" + leagueId + "/mob?annex=3&desktopVersion=2.24.43&locale=sr";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }

        public List<string> GetLeagues()
        {
            string url = "https://www.maxbet.rs/restapi/offer/sr/categories/sport/S/l?annex=3&desktopVersion=2.24.43&locale=sr";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            //TODO PROVERITI STA JE Max Bonus Tip Fudbal U LIGAMA I IZBACITI ako treba

            JsonLeaguesResponse leaguesResponse = JsonConvert.DeserializeObject<JsonLeaguesResponse>(response.Content);

            leagues = leaguesResponse.categories;

            return leagues.Select(x => x.id).ToList();
        }
    }
}
