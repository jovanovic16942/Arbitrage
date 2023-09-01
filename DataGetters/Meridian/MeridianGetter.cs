using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Meridian
{
    public class MeridianGetter
    {
        public MeridianGetter() { }

        public List<MatchResponse> GetMatches(DateTime? date)
        {
            var responses = new List<MatchResponse>();

            date ??= DateTime.Now;

            string formattedDateTime = date.Value.ToString("yyyy-MM-ddTHH:mm:sszzz");

            string baseUrl = "https://meridianbet.rs/sails/sport/58/date/";
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

                break;//obrisi

                Thread.Sleep(Constants.SleepTime);
            }

            return responses;
        }

        private MatchResponse GetMatchResponse(string url)
        {
            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            MatchResponse matchResponse = JsonConvert.DeserializeObject<MatchResponse>(response.Content);

            return matchResponse;
        }
    }
}
