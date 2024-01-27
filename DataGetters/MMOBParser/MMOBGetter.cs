using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.MMOB
{
    public abstract class MMOBGetter
    {
        public abstract string GetSportUrl(string sport);
        public abstract string GetMatchUrl(long matchId);

        public JsonMatchResponse GetSportResponse(string sport)
        {
            string url = GetSportUrl(sport);

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            var matchesResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchesResponse;
        }

        public JsonMatch GetMatchResponse(long matchId)
        {
            Thread.Sleep(Constants.SleepTimeShort);
            string url = GetMatchUrl(matchId);

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            var matchResponse = JsonConvert.DeserializeObject<JsonMatch>(response.Content);

            return matchResponse;
        }
    }
}
