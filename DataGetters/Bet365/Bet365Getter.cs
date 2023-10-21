using Arbitrage.EntityFramework.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Bet365
{
    public class Bet365Getter
    {
        public void GetResponse()
        {
            string url = "https://www.365.rs/ibet-web-client/#/home/leaguesWithMatches";
            //string url = "https://www.365.rs/ibet/offer/league/2222612/-1/0/false.json?v=4.52.76&locale=sr&ttgIds=";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            //JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            //return matchResponse;
        }
    }
}
