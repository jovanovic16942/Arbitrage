using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;

namespace Arbitrage.DataGetters.Mozzart
{

    public class MozzartGetter
    {
        public JsonMatchResponse GetMatches(int sportId)
        {
            // create a new RestSharp client
            var client = new RestClient("https://www.mozzartbet.com");

            // create a new RestSharp request
            var request = new RestRequest("/betOffer2", Method.Post);

            // add parameters to the request body
            request.AddParameter("origin", "https://www.mozzartbet.com", ParameterType.HttpHeader);
            request.AddParameter("referer", "https://www.mozzartbet.com/sr/kladjenje-2018", ParameterType.HttpHeader);

            // add request body
            var requestBody = new
            {
                activeCompleteOffer = true,
                competitionIds = new object[] { },
                date = "all",
                lang = "sr",
                mostPlayed = false,
                numberOfGames = 0,
                offset = 0,
                size = 2000,
                sort = "bycompetition",
                //specials = null,
                sportIds = new object[] { sportId },
                subgames = new object[] { },
                type = "betting"
            };

            request.AddJsonBody(requestBody);

            // execute the request and get the response
            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }

        public List<JsonRoot> GetOdds(IEnumerable<int> matchIds, List<string> subgamesIds)
        {
            // create a new RestSharp client
            var client = new RestClient("https://www.mozzartbet.com");

            int numOfSteps = matchIds.Count() / 50;

            List<JsonRoot> result = new List<JsonRoot>();

            for (int i = 0; i <= numOfSteps; i++)
            {
                // create a new RestSharp request
                var request = new RestRequest("/getBettingOdds", Method.Post);

                // add parameters to the request body
                request.AddParameter("origin", "https://www.mozzartbet.com", ParameterType.HttpHeader);
                request.AddParameter("referer", "https://www.mozzartbet.com/sr/kladjenje-2018", ParameterType.HttpHeader);

                // add request body
                var requestBody = new
                {
                    matchIds = matchIds.Select(x => x).Skip(i * 50).Take(50).ToArray(),
                    subgames = subgamesIds,
                };

                request.AddJsonBody(requestBody);

                // execute the request and get the response
                RestResponse response = client.Execute(request);

                try
                {
                    var oddsResponse = JsonConvert.DeserializeObject<List<JsonRoot>>(response.Content);
                    result.AddRange(oddsResponse);
                } catch (Exception ex)
                {
                    // log
                }

                //Console.WriteLine("MozzartGetter GetOdds iteration: " + i);
                Thread.Sleep(Constants.SleepTime);
            }

            return result;
        }
    }
}