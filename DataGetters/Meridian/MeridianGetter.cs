using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;

namespace Arbitrage.DataGetters.Meridian
{
    public class MeridianGetter
    {
        public MeridianGetter() { }

        public List<JsonMatchResponse> GetMatches(int sportId)
        {
            var responses = new List<JsonMatchResponse>();

            string formattedDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");

            string baseUrl = "https://meridianbet.rs/sails/sport/" + sportId + "/date/";  //basket 55 - 58 f
            string urlFilter = "/filter/oneDay/offset/";
            const string urlPositon = "?filterPositions=0,0,0";

            for(int offset = 0; ; offset += 20)
            {
                string requestUrl = string.Join("", baseUrl, formattedDateTime, urlFilter, offset, urlPositon);
                var response =  GetMatchResponse(requestUrl);               

                if (!response.events.Any())
                {
                    break;
                }

                responses.Add(response);
            }

            return responses;
        }

        private JsonMatchResponse GetMatchResponse(string url)
        {
            Thread.Sleep(Constants.SleepTimeShort);

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }

        public JsonEvent GetOddsResponse(string matchId)
        {
            var client = new RestClient("https://meridianbet.rs/sails/events/");

            var request = new RestRequest(matchId, Method.Get);

            RestResponse response = client.Execute(request);

            JsonEvent matchResponse = JsonConvert.DeserializeObject<JsonEvent>(response.Content);

            return matchResponse;
        }
    }
}
