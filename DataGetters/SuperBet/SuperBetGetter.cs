using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Arbitrage.DataGetters.SuperBet
{
    public class SuperBetGetter
    {

        public JsonMatchesResponse GetMatches()
        {
            string startDate = DateTime.Now.ToString("yyyy-MM-dd+HH:mm:ss");
            string endDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd+HH:mm:ss");

            string url = "https://production-superbet-offer-basic.freetls.fastly.net/sb-rs/api/offer/getOfferByDate?offerState=prematch&startDate=" + startDate + "&endDate=" + endDate;

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchesResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchesResponse>(response.Content);

            return matchResponse;
        }

        public JsonMatchesResponse GetMatchData(IEnumerable<int> matchIds)
        {
            Thread.Sleep(Constants.SleepTime);

            var idsString = string.Join(",", matchIds);

            string url = "https://production-superbet-offer-basic.freetls.fastly.net/sb-rs/api/matches/byId?matchIds=" + idsString;

            //string url = "https://production-superbet-offer-basic.freetls.fastly.net/sb-rs/api/v2/sr-Latn-RS/events/" + idsString + "?matchIds=" + idsString;


            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchesResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchesResponse>(response.Content);

            return matchResponse;
        }

        public JsonMatchResponse GetMatchResponse(int matchId)
        {
            Thread.Sleep(Constants.SleepTimeShort);

            string url = "https://production-superbet-offer-basic.freetls.fastly.net/sb-rs/api/v2/sr-Latn-RS/events/" + matchId + "?matchIds=" + matchId;

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }
    }
}