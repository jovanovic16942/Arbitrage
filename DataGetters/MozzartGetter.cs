using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters
{
    public class MozzartGetter
    {
        List<MozzartData> mozzartData = new List<MozzartData>();

        //TODO FIX 1 PARTICIPANT ODDS

        public void GetMatches(DateTime? date)
        {
            if (date == null)
            {
                date = DateTime.Now;
            }

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
                activeCompleteOffer = false,
                competitionIds = new object[] { },
                date = date.Value.ToString("yyyy-MM-dd"),
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

            MatchResponse matchResponse = JsonConvert.DeserializeObject<MatchResponse>(response.Content);

            foreach (var match in matchResponse.Matches)
            {
                try
                {
                    var participants = match.Participants.ToList();
                    var p1 = participants[0];
                    var p2 = participants[1];

                    var participant1 = new Participant(p1.Id, p1.Name, p1.ShortName, p1.Description);
                    var participant2 = new Participant(p2.Id, p2.Name, p2.ShortName, p2.Description);

                    mozzartData.Add(new MozzartData(match.Id, match.StartTime, participant1, participant2));
                }
                catch {

                }
            }
        }

        public void GetOdds()
        {


            // create a new RestSharp client
            var client = new RestClient("https://www.mozzartbet.com");

            // create a new RestSharp request
            var request = new RestRequest("/getBettingOdds", Method.Post);

            // add parameters to the request body
            request.AddParameter("origin", "https://www.mozzartbet.com", ParameterType.HttpHeader);
            request.AddParameter("referer", "https://www.mozzartbet.com/sr/kladjenje-2018", ParameterType.HttpHeader);


            int numOfSteps = mozzartData.Count / 50 + 1;

            for(int i = 0; i <= numOfSteps; i++)
            {
                // add request body
                var requestBody = new
                {
                    matchIds = mozzartData.Select(x => x.MatchId).Skip(i * 50).Take(50).ToArray(),
                    subgames = subgamesIds,
                };


                request.AddJsonBody(requestBody);

                // execute the request and get the response
                RestResponse response = client.Execute(request);


            }  

        }

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