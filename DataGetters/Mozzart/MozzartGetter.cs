using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;

namespace Arbitrage.DataGetters.Mozzart
{

    public class MozzartGetter
    {
        public JsonMatchResponse GetMatches()
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
                sportIds = new object[] { 1 },
                subgames = new object[] { },
                type = "betting"
            };

            request.AddJsonBody(requestBody);

            // execute the request and get the response
            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }

        public List<JsonRoot> GetOdds(IEnumerable<int> matchIds)
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

                var oddsResponse = JsonConvert.DeserializeObject<List<JsonRoot>>(response.Content);

                result.AddRange(oddsResponse);

                Thread.Sleep(Constants.SleepTime);
            }


            return result;
        }

        //public void InsertTeams()
        //{
        //    List<Team> teams = new List<Team>();

        //    foreach (var match in mozzartData)
        //    {
        //        teams.Add(new Team { Name = match.Participant1.Name, ShortName = match.Participant1.ShortName });
        //        teams.Add(new Team { Name = match.Participant2.Name, ShortName = match.Participant2.ShortName });
        //    }

        //    teams = teams.DistinctBy(x => x.Name).ToList();

        //    ArbitrageDb.Instance().InsertTeams(teams);
        //}

        static List<string> subgamesIds = new List<string>
        {
            "1001001001",
            "1001001002",
            "1001001003",
            "1001002001",
            "1001002002",
            "1001002003",
            "1001003002",
            "1001003012",
            "1001003004",
            "1001130001",
            "1001130002",
            "1001130003",
            "1001141017",
            "1001141015",
            "1001772001",
            "1001772002",
            "1001773001",
            "1001773002",
            "1001774001",
            "1001774002",
            "1001775001",
            "1001775002",
            "1001776001",
            "1001776002",
            "1001772001",
            "1001772002",
            "1001773001",
            "1001773002",
            "1001774001",
            "1001774002",
            "1001775001",
            "1001775002",
            "1001776001",
            "1001776002"
        };

    }
}
//7087955