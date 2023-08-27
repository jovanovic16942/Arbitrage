﻿using Arbitrage.EntityFramework;
using Arbitrage.EntityFramework.Models;
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
        public MatchResponse GetMatches(DateTime? date)
        {
            date ??= DateTime.Now;

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

            return matchResponse;
        }

        public List<Root> GetOdds(IEnumerable<int> matchIds)
        {

            // create a new RestSharp client
            var client = new RestClient("https://www.mozzartbet.com");

            int numOfSteps = matchIds.Count() / 50;

            List<Root> result = new List<Root>();

            for(int i = 0; i <= numOfSteps; i++)
            {
                // create a new RestSharp request
                var request = new RestRequest("/getBettingOdds", Method.Post);

                // add parameters to the request body
                request.AddParameter("origin", "https://www.mozzartbet.com", ParameterType.HttpHeader);
                request.AddParameter("referer", "https://www.mozzartbet.com/sr/kladjenje-2018", ParameterType.HttpHeader);

                // add request body
                var requestBody = new
                {
                    matchIds = matchIds.Select(x => x).Skip(i * 50).Take(50).ToArray(),
                    subgames = subgamesIds,
                };

                request.AddJsonBody(requestBody);

                // execute the request and get the response
                RestResponse response = client.Execute(request);

                var oddsResponse = JsonConvert.DeserializeObject<List<Root>>(response.Content);

                result.AddRange(oddsResponse);

                Thread.Sleep(2000);
            }


            return result;
        }

        //public void InsertTeams()
        //{
        //    List<Team> teams = new List<Team>();

        //    foreach (var match in mozzartData)
        //    {
        //        teams.Add(new Team { Name = match.Participant1.Name, ShortName = match.Participant1.ShortName });
        //        teams.Add(new Team { Name = match.Participant2.Name, ShortName = match.Participant2.ShortName });
        //    }

        //    teams = teams.DistinctBy(x => x.Name).ToList();

        //    ArbitrageDb.Instance().InsertTeams(teams);
        //}

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