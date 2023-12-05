using Arbitrage.DataGetters.AdmiralBet;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Admiralbet
{
    public class AdmiralBetGetter
    {
        public AdmiralBetGetter() { }

        public List<JsonMatchResponse> GetMatches()
        {
            List<JsonMatchResponse> matches = new();

            int offset = 0;

            while (true)
            {
                //Console.WriteLine("AdmiralBetGetter page offset: " + offset);
                var respose = GetPage(offset);

                if (respose == null || respose.competitions == null || respose.competitions.Count == 0) 
                {
                    break;
                }

                matches.Add(respose);

                offset += 25;

                Thread.Sleep(General.Constants.SleepTime);
            }

            return matches;
        }

        private JsonMatchResponse GetPage(int offset, int perPage = 25)
        {
            var dateTime = DateTime.Now.AddDays(1000).AddHours(-2);
            var dateTimeString = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            string footballUrl = "https://sport-webapi.admiralbet.rs/SportBookCacheWeb/api/offer/competitionsWithEventsStartingSoonForSportV2/"
                + perPage + "/" + offset +
                "/false/" + dateTimeString + "/1";

            var client = new RestClient(footballUrl);

            var request = new RestRequest("", Method.Get);

            request.AddHeader("Accept", "application/json, text/plain, */*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", "en-AU,en;q=0.9");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Host", "sport-webapi.admiralbet.rs");
            request.AddHeader("Language", "sr-Latn");
            request.AddHeader("Officeid", "138");
            request.AddHeader("Origin", "https://admiralbet.rs");
            request.AddHeader("Referer", "https://admiralbet.rs/");
            //request.AddHeader("Sec-Ch-Ua", "application");
            //request.AddHeader("Accept", "application");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36");

            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }

    }
}
