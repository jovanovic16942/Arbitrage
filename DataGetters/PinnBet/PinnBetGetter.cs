﻿using Newtonsoft.Json;
using RestSharp;

namespace Arbitrage.DataGetters.PinnBet
{
    public class PinnBetGetter
    {
        public List<JsonMatchResponse> GetMatches(int sportId)
        {
            List<JsonMatchResponse> matches = new();

            int offset = 0;

            while (true)
            {
                var respose = GetPage(offset, sportId);

                if (respose == null || respose.competitions == null || respose.competitions.Count == 0)
                {
                    break;
                }

                matches.Add(respose);

                offset += 25;
            }

            return matches;
        }

        private JsonMatchResponse GetPage(int offset, int sportId, int perPage = 25)
        {
            Thread.Sleep(General.Constants.SleepTimeShort);

            var dateTime = DateTime.Now.AddDays(1000).AddHours(-2);
            var dateTimeString = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            string footballUrl = "https://sportweb.pinnbet.rs/SportBookCacheWeb/api/offer/competitionsWithEventsStartingSoonForSportV2/"
                + perPage + "/" + offset +
                "/false/" + dateTimeString + "/" + sportId;

            var client = new RestClient(footballUrl);

            var request = new RestRequest("", Method.Get);

            request.AddHeader("Accept", "application/json, text/plain, */*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", "en-AU,en;q=0.9");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Language", "sr-Latn");
            request.AddHeader("Officeid", "6");
            request.AddHeader("Origin", "https://www.pinnbet.rs");
            request.AddHeader("Referer", "https://www.pinnbet.rs/");
            //request.AddHeader("Sec-Ch-Ua", "application");
            //request.AddHeader("Accept", "application");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36");

            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }

        public JsonEvent GetBets(int competitionId, long eventId, int regionId, int sportId)
        {
            Thread.Sleep(General.Constants.SleepTimeShort);

            string url = "https://sportweb.pinnbet.rs/SportBookCacheWeb/api/offer/betsAndGroups/"
                +  string.Format("{0}/{1}/{2}/{3}", sportId, regionId, competitionId, eventId);


            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            request.AddHeader("Accept", "application/utf8+json, application/json, text/plain, */*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", "en-AU,en-GB;q=0.9,en-US;q=0.8,en;q=0.7");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Language", "sr-Latn");
            request.AddHeader("Officeid", "6");
            request.AddHeader("Origin", "https://www.pinnbet.rs");
            request.AddHeader("Referer", "https://www.pinnbet.rs/");
            //request.AddHeader("Sec-Ch-Ua", "application");
            //request.AddHeader("Accept", "application");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

            RestResponse response = client.Execute(request);

            JsonEvent matchResponse = JsonConvert.DeserializeObject<JsonEvent>(response.Content);

            return matchResponse;
        }


    }
}
