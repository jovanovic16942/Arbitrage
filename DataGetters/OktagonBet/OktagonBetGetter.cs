using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.OktagonBet
{
    internal class OktagonBetGetter
    {
        public JsonMatchResponse GetFootballResponse()
        {
            string url = "https://www.oktagonbet.com/restapi/offer/sr/sport/S/mob?annex=1&mobileVersion=2.21.50&locale=sr";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            var matchesResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchesResponse;
        }

        public JsonMatch GetMatchResponse(long matchId)
        {
            Thread.Sleep(Constants.SleepTimeShort);
            string url = "https://www.oktagonbet.com/restapi/offer/sr/match/" + matchId + "?annex=1&mobileVersion=2.21.50&locale=sr";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            var matchResponse = JsonConvert.DeserializeObject<JsonMatch>(response.Content);

            return matchResponse;
        }
    }
}
