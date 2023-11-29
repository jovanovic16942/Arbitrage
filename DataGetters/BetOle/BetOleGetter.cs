using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.BetOle
{
    internal class BetOleGetter
    {
        public JsonMatchResponse GetFootballResponse()
        {
            string url = "https://ibet2.betole.com/restapi/offer/sr/sport/S/mob?annex=0&mobileVersion=2.21.51&locale=sr";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            var matchesResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchesResponse;
        }

        public JsonMatch GetMatchResponse(long matchId)
        {
            Thread.Sleep(Constants.SleepTimeShort);
            string url = "https://ibet2.betole.com/restapi/offer/sr/match/" + matchId + "?annex=0&mobileVersion=2.21.51&locale=sr";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            var matchResponse = JsonConvert.DeserializeObject<JsonMatch>(response.Content);

            return matchResponse;
        }
    }
}
