using Arbitrage.EntityFramework.Models;
using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Bet365
{
    public class Bet365Getter
    {
        // TODO Get all matches
        public JsonMatchesResponse GetResponse()
        {
            string url = "https://www.365.rs/ibet/async/offer/lastMinuteMatches/0.json?v=4.52.76&locale=sr&ttgIds=";
            //string url = "https://www.365.rs/ibet/offer/league/2222612/-1/0/false.json?v=4.52.76&locale=sr&ttgIds=";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchesResponse matchesResponse = JsonConvert.DeserializeObject<JsonMatchesResponse>(response.Content);

            return matchesResponse;
        }

        public JsonMatch GetMatchResponse(long matchId)
        {
            Thread.Sleep(Constants.SleepTimeShort);

            string url = "https://www.365.rs/ibet/offer/special/null/" + matchId + ".json?v=4.52.76&locale=sr";
            //string url = "https://www.365.rs/ibet/offer/league/2222612/-1/0/false.json?v=4.52.76&locale=sr&ttgIds=";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatch matchResponse = JsonConvert.DeserializeObject<JsonMatch>(response.Content);

            return matchResponse;
        }
    }
}
