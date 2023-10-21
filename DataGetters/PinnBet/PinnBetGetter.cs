using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Pinnbet
{
    public class PinnBetGetter
    {
        List<int> matchEventIds = new List<int>();

        public List<JsonMatch> GetResponse()
        {
            string url = "https://www.pinnbet.rs/apiprematch/events/to/F/2026-8-26%206:55";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            var matchResponse = JsonConvert.DeserializeObject<List<JsonMatch>>(response.Content);

            return matchResponse;    
        }


        public List<JsonSelection> GetSingleMatchResponse(int eventId, int roundId)
        {
            Thread.Sleep(Constants.SleepTimeShort); // TODO maybe randomize wait time
            string url = "https://www.pinnbet.rs/apiprematch/selections/" + roundId + "/" + eventId;

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            var matchResponse = JsonConvert.DeserializeObject<List<JsonSelection>>(response.Content);

            return(matchResponse);
        }
    }
}
