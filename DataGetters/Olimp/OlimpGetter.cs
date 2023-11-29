using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Arbitrage.DataGetters.Olimp
{
    public class OlimpGetter
    {
        private CookieCollection _cookies;

        public OlimpGetter() 
        {
            //getting cookies
            string url = "https://online.kladioniceolimp.com/zns_pocetna.php?dsm=1";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            _cookies  = response.Cookies;
        }

        public string GetLeagues()
        {
            string url = "https://online.kladioniceolimp.com/zns_pocetna.php?dsm=1";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Post);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("Host", "online.kladioniceolimp.com");
            request.AddHeader("Origin", "https://online.kladioniceolimp.com");
            request.AddHeader("Referer", "https://online.kladioniceolimp.com/zns_pocetna.php?dsm=1");

            foreach (Cookie cookie in _cookies)
            {
                request.AddCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
            }

            RestResponse response = client.Execute(request);

            var cook = response.Cookies;

            return response.Content ?? "";
        }

        public string GetMatches(List<int> leagueIds)
        {
            string url = "https://online.kladioniceolimp.com/zns_ajax/showOffer.php";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Post);

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", "en-AU,en-GB;q=0.9,en-US;q=0.8,en;q=0.7");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("Host", "online.kladioniceolimp.com");
            request.AddHeader("Origin", "https://online.kladioniceolimp.com");
            request.AddHeader("Referer", "https://online.kladioniceolimp.com/zns_pocetna.php");
            request.AddHeader("Sec-Ch-Ua", "\"Google Chrome\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
            request.AddHeader("Sec-Ch-Ua-Mobile", "?0");
            request.AddHeader("Sec-Ch-Ua-Platform", "\"Windows\"");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");

            foreach (Cookie cookie in _cookies)
            {
                request.AddCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
            }

            List<string> tmpLeagueStrings = leagueIds.ConvertAll(i => i.ToString());

            string leagueIdsReqStr = string.Join(',', tmpLeagueStrings);


            string encodedSelOffer = HttpUtility.UrlEncode(leagueIdsReqStr);

            // Kreiranje kompletnog request body-a
            string requestBody = $"selOffer={encodedSelOffer}&opcPonudaX=0&idm=&prNaziv=&vremenska=1";


            //string jsonBody = JsonConvert.SerializeObject(requestBody);
            int contentLength = Encoding.UTF8.GetBytes(requestBody).Length;
            request.AddHeader("Content-Length", contentLength.ToString());

            request.AddParameter("application/json", requestBody, ParameterType.RequestBody);


            RestResponse response = client.Execute(request);

            return response.Content ?? "";
        }


        public string GetMatchOdds(string matchId)
        {
            Thread.Sleep(Constants.SleepTimeShort);

            string url = "https://online.kladioniceolimp.com/zns_ajax/showOffer.php";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Post);

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", "en-AU,en-GB;q=0.9,en-US;q=0.8,en;q=0.7");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("Host", "online.kladioniceolimp.com");
            request.AddHeader("Origin", "https://online.kladioniceolimp.com");
            request.AddHeader("Referer", "https://online.kladioniceolimp.com/zns_pocetna.php");
            request.AddHeader("Sec-Ch-Ua", "\"Google Chrome\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
            request.AddHeader("Sec-Ch-Ua-Mobile", "?0");
            request.AddHeader("Sec-Ch-Ua-Platform", "\"Windows\"");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");

            foreach (Cookie cookie in _cookies)
            {
                request.AddCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
            }

            string encodedIdm = HttpUtility.UrlEncode(matchId);

            // Kreiranje kompletnog request body-a
            string requestBody = $"idm={encodedIdm}";


            //string jsonBody = JsonConvert.SerializeObject(requestBody);
            int contentLength = Encoding.UTF8.GetBytes(requestBody).Length;
            request.AddHeader("Content-Length", contentLength.ToString());

            request.AddParameter("application/json", requestBody, ParameterType.RequestBody);


            RestResponse response = client.Execute(request);

            return response.Content ?? "";
        }
    }
}
