using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;

namespace Arbitrage.DataGetters.Bet365
{
    public class Bet365Getter
    {
        // TODO Get all matches
        public JsonMatchesResponse GetResponse()
        {
            // https://www.365.rs/ibet/offer/sportsAndLeagues/-1.json?v=4.52.76&locale=sr

            string url = "https://www.365.rs/ibet/async/offer/lastMinuteMatches/0.json?v=4.52.76&locale=sr&ttgIds=";
            //string url = "https://www.365.rs/ibet/offer/league/2222612/-1/0/false.json?v=4.52.76&locale=sr&ttgIds=";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchesResponse matchesResponse = JsonConvert.DeserializeObject<JsonMatchesResponse>(response.Content);

            return matchesResponse;
        }

        public List<JsonSport> GetLeagues()
        {
            string url = "https://www.365.rs/ibet/offer/sportsAndLeagues/-1.json?v=4.52.76&locale=sr";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            List<JsonSport> matchesResponse = JsonConvert.DeserializeObject<List<JsonSport>>(response.Content);

            return matchesResponse;
        }

        public JsonLeagueMatchesResponse GetMatchesInLeague(long leagueId)
        {
            string url = "https://www.365.rs/ibet/offer/league/" +leagueId + "/-1/0/false.json?v=4.52.76&locale=sr&ttgIds=";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonLeagueMatchesResponse matchesResponse = JsonConvert.DeserializeObject<JsonLeagueMatchesResponse>(response.Content);

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
