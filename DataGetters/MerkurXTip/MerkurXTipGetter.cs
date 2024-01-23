using Arbitrage.EntityFramework.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.MerkurXTip
{
    public class MerkurXTipGetter
    {
        public MerkurXTipGetter() { }

        public JsonMatchResponse GetMatches()
        {
            string url = "https://www.merkurxtip.rs/restapi/offer/sr/sport/S/mob?annex=0&desktopVersion=2.24.46&locale=sr";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }

        public JsonMatch GetMatch(long matchId)
        {
            Thread.Sleep(General.Constants.SleepTimeShort);

            string url = "https://www.merkurxtip.rs/restapi/offer/sr/match/" + matchId + "?annex=0&desktopVersion=1.31.5&locale=sr";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatch matchResponse = JsonConvert.DeserializeObject<JsonMatch>(response.Content);

            return matchResponse;
        }
    }
}
