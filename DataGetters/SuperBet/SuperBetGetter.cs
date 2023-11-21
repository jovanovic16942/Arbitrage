using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.SuperBet
{
    public class SuperBetGetter
    {

        public JsonMatchResponse GetMatches()
        {

            string startDate = DateTime.Now.ToString("yyyy-MM-dd+HH:mm:ss");
            string endDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd+HH:mm:ss");

            string url = "https://production-superbet-offer-basic.freetls.fastly.net/sb-rs/api/offer/getOfferByDate?offerState=prematch&startDate=" + startDate + "&endDate=" + endDate;

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }

        public JsonMatchResponse GetMatchData(IEnumerable<int> matchIds)
        {
            Thread.Sleep(Constants.SleepTime);

            var idsString = string.Join(",", matchIds);

            string url = "https://production-superbet-offer-basic.freetls.fastly.net/sb-rs/api/matches/byId?matchIds=" + idsString;

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }
    }
}