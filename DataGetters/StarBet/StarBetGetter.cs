using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.StarBet
{
    public class StarBetGetter
    {
        private CookieCollection _cookies;

        public void GetCookies()
        {
            string url = "https://starbet.rs/Bet";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            request.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            //request.AddHeader("Origin", "https://starbet.rs");
            //request.AddHeader("Referer", "https://starbet.rs/Bet");

            RestResponse response = client.Execute(request);

            _cookies = response.Cookies!;
        }

        private void AddCokies(RestRequest request)
        {
            foreach (Cookie cookie in _cookies)
            {
                request.AddCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
            }
        }

        public List<JsonLeagueResponse> GetLeagues()
        {
            GetCookies();

            string url = "https://starbet.rs/Oblozuvanje.aspx/GetSportoviSoLigi";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Post);

            request.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Origin", "https://starbet.rs");
            request.AddHeader("Referer", "https://starbet.rs/Bet");

            //AddCokies(request);

            var requestBody = new
            {
                filter = "0",
                activeStyle = "img/sports"
            };

            request.AddJsonBody(requestBody);

            RestResponse response = client.Execute(request);

            var leagueResponse = JsonConvert.DeserializeObject<List<JsonLeagueResponse>>(response.Content);

            return leagueResponse;
        }

        public List<JsonLeagueDataResponse> GetMatchesResponse(List<int> leagueIDs)
        {
            //GetCookies();

            string url = "https://starbet.rs/Oblozuvanje.aspx/GetLiga";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Post);

            request.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Origin", "https://starbet.rs");
            request.AddHeader("Referer", "https://starbet.rs/Bet");

            //AddCokies(request);

            var requestBody = new
            {
                filter = "0",
                LigaID = leagueIDs.ToArray(),
                parId = 0
            };

            string jsonBodyString = JsonConvert.SerializeObject(requestBody);

            request.AddParameter("application/json", jsonBodyString, ParameterType.RequestBody);

            RestResponse response = client.Execute(request);

            var matchResponse = JsonConvert.DeserializeObject<List<JsonLeagueDataResponse>>(response.Content);

            return matchResponse;
        }


        public List<JsonOddsResponse> GetOdds(long PID)
        {
            Thread.Sleep(Constants.SleepTimeShort);

            string url = "https://starbet.rs/Oblozuvanje.aspx/GetTipoviV2";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Post);

            request.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Origin", "https://starbet.rs");
            request.AddHeader("Referer", "https://starbet.rs/Bet");

            //AddCokies(request);

            var requestBody = new
            {
                PairId = PID
            };

            string jsonBodyString = JsonConvert.SerializeObject(requestBody);

            request.AddParameter("application/json", jsonBodyString, ParameterType.RequestBody);

            RestResponse response = client.Execute(request);

            var oddsResponse = JsonConvert.DeserializeObject<List<JsonOddsResponse>>(response.Content);

            return oddsResponse;
        }

    }
}
