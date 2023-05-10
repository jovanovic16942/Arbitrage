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

        public void GetMatches()
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
                activeCompleteOffer = false,
                competitionIds = new object[] { },
                date = "2023-03-18",
                lang = "sr",
                mostPlayed = false,
                numberOfGames = 304,
                offset = 0,
                size = 50,
                sort = "bycompetition",
                specials = false,
                sportIds = new object[] { 1 },
                subgames = new object[] { },
                type = "betting"
            };

            request.AddJsonBody(requestBody);

            // execute the request and get the response
            RestResponse response = client.Execute(request);


            // print the response content
            //Console.WriteLine(response.Content);
            //File.WriteAllText("mozzart-fudbal-betOffer2", response.Content);


            //var responseText = File.ReadAllText("mozzart-fudbal-betOffer2");

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

            // add request body
            var requestBody = new
            {
                matchIds = mozzartData.Select(x => x.MatchId).ToArray(),
                // subgames = new object[] { },
            };



            request.AddJsonBody(requestBody);

            // execute the request and get the response
            RestResponse response = client.Execute(request);

        }
    }
}
//7087955