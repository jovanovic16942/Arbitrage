﻿using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;

namespace Arbitrage.DataGetters.Mozzart
{

    public class MozzartGetter
    {
        public JsonMatchResponse GetMatches()
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
                activeCompleteOffer = true,
                competitionIds = new object[] { },
                date = "all",
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

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }

        public List<JsonRoot> GetOdds(IEnumerable<int> matchIds)
        {

            // create a new RestSharp client
            var client = new RestClient("https://www.mozzartbet.com");

            int numOfSteps = matchIds.Count() / 50;

            List<JsonRoot> result = new List<JsonRoot>();

            for (int i = 0; i <= numOfSteps; i++)
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

                var oddsResponse = JsonConvert.DeserializeObject<List<JsonRoot>>(response.Content);

                result.AddRange(oddsResponse);

                //Console.WriteLine("MozzartGetter GetOdds iteration: " + i);
                Thread.Sleep(Constants.SleepTime);
            }


            return result;
        }

        static readonly List<string> subgamesIds = new()
        {
            "1001001001",
         "1001001002",
         "1001001003",
         "1001002001",
         "1001002002",
         "1001002003",
         "1001003001",
         "1001003002",
         "1001003003",
         "1001003004",
         "1001003005",
         "1001003006",
         "1001003007",
         "1001003008",
         "1001003012",
         "1001003013",
         "1001003014",
         "1001003015",
         "1001003016",
         "1001003017",
         "1001003018",
         "1001003019",
         "1001003020",
         "1001003021",
         "1001003022",
         "1001003024",
         "1001003025",
         "1001003026",
         "1001003027",
         "1001003028",
         "1001003029",
         "1001003030",
         "1001003031",
         "1001003032",
         "1001003033",
         "1001003034",
         "1001003035",
         "1001003036",
         "1001003037",
         "1001003038",
         "1001004001",
         "1001004002",
         "1001004003",
         "1001005001",
         "1001005002",
         "1001005003",
         "1001005004",
         "1001005005",
         "1001005006",
         "1001005007",
         "1001005008",
         "1001005009",
         "1001005010",
         "1001005011",
         "1001005012",
         "1001005013",
         "1001005014",
         "1001005015",
         "1001005016",
         "1001005017",
         "1001005018",
         "1001005019",
         "1001005020",
         "1001005021",
         "1001005022",
         "1001005023",
         "1001007000",
         "1001007001",
         "1001007002",
         "1001008001",
         "1001008002",
         "1001008003",
         "1001008005",
         "1001008006",
         "1001008008",
         "1001008010",
         "1001008011",
         "1001008012",
         "1001008013",
         "1001008014",
         "1001009001",
         "1001009002",
         "1001009003",
         "1001009005",
         "1001009006",
         "1001009008",
         "1001009009",
         "1001009010",
         "1001009011",
         "1001009012",
         "1001009013",
         "1001009014",
         "1001016001",
         "1001016002",
         "1001016003",
         "1001019001",
         "1001019002",
         "1001019003",
         "1001020000",
         "1001020001",
         "1001020002",
         "1001020003",
         "1001020004",
         "1001020010",
         "1001020011",
         "1001020012",
         "1001020013",
         "1001020014",
         "1001020020",
         "1001020021",
         "1001020022",
         "1001020023",
         "1001020024",
         "1001020030",
         "1001020031",
         "1001020032",
         "1001020033",
         "1001020040",
         "1001020041",
         "1001020042",
         "1001020043",
         "1001026001",
         "1001026002",
         "1001128001",
         "1001128002",
         "1001128003",
         "1001128004",
         "1001128006",
         "1001128007",
         "1001129001",
         "1001129002",
         "1001129003",
         "1001129006",
         "1001129007",
         "1001130001",
         "1001130002",
         "1001130003",
         "1001130004",
         "1001130005",
         "1001130006",
         "1001130007",
         "1001130008",
         "1001130009",
         "1001130010",
         "1001130011",
         "1001130013",
         "1001131001",
         "1001131002",
         "1001131003",
         "1001131004",
         "1001131005",
         "1001131006",
         "1001131007",
         "1001131008",
         "1001131009",
         "1001131010",
         "1001131011",
         "1001131012",
         "1001131013",
         "1001131014",
         "1001131015",
         "1001131016",
         "1001131017",
         "1001132001",
         "1001132002",
         "1001132003",
         "1001132004",
         "1001132005",
         "1001132006",
         "1001132007",
         "1001132008",
         "1001132009",
         "1001132011",
         "1001132012",
         "1001132013",
         "1001132014",
         "1001132015",
         "1001132016",
         "1001132017",
         "1001139001",
         "1001139002",
         "1001140001",
         "1001140002",
         "1001140003",
         "1001140004",
         "1001140005",
         "1001140006",
         "1001141001",
         "1001141002",
         "1001141003",
         "1001141004",
         "1001141005",
         "1001141006",
         "1001141007",
         "1001141008",
         "1001141010",
         "1001141011",
         "1001141012",
         "1001141013",
         "1001141014",
         "1001141015",
         "1001141016",
         "1001141017",
         "1001141018",
         "1001141019",
         "1001141021",
         "1001141030",
         "1001141031",
         "1001141032",
         "1001141033",
         "1001141034",
         "1001141035",
         "1001141036",
         "1001141037",
         "1001141038",
         "1001141039",
         "1001141040",
         "1001141041",
         "1001141042",
         "1001141043",
         "1001141044",
         "1001141045",
         "1001141046",
         "1001141047",
         "1001141048",
         "1001141049",
         "1001141050",
         "1001141051",
         "1001141052",
         "1001141053",
         "1001141054",
         "1001141055",
         "1001141056",
         "1001141057",
         "1001141058",
         "1001141059",
         "1001141060",
         "1001141061",
         "1001141062",
         "1001141063",
         "1001141064",
         "1001141065",
         "1001141066",
         "1001141067",
         "1001141068",
         "1001141069",
         "1001141070",
         "1001141071",
         "1001141072",
         "1001141073",
         "1001141074",
         "1001141075",
         "1001141076",
         "1001141077",
         "1001141078",
         "1001141079",
         "1001141080",
         "1001141081",
         "1001141082",
         "1001141083",
         "1001141084",
         "1001141085",
         "1001141086",
         "1001141087",
         "1001141088",
         "1001141089",
         "1001141090",
         "1001141091",
         "1001141092",
         "1001141093",
         "1001141094",
         "1001141095",
         "1001141096",
         "1001141097",
         "1001141098",
         "1001141099",
         "1001141100",
         "1001141101",
         "1001141102",
         "1001141103",
         "1001141104",
         "1001141105",
         "1001141106",
         "1001141107",
         "1001141108",
         "1001141109",
         "1001141110",
         "1001141111",
         "1001141112",
         "1001141113",
         "1001141114",
         "1001141115",
         "1001141116",
         "1001141117",
         "1001141118",
         "1001141119",
         "1001141120",
         "1001141121",
         "1001141122",
         "1001141123",
         "1001141124",
         "1001141125",
         "1001141126",
         "1001141127",
         "1001141128",
         "1001141129",
         "1001141130",
         "1001141131",
         "1001141132",
         "1001141133",
         "1001141134",
         "1001141135",
         "1001141136",
         "1001141137",
         "1001141138",
         "1001141139",
         "1001141140",
         "1001141141",
         "1001141142",
         "1001141143",
         "1001141144",
         "1001141145",
         "1001141146",
         "1001141148",
         "1001141149",
         "1001141150",
         "1001141151",
         "1001141152",
         "1001141153",
         "1001141154",
         "1001141155",
         "1001141156",
         "1001141157",
         "1001141158",
         "1001141159",
         "1001141160",
         "1001141161",
         "1001141162",
         "1001141163",
         "1001141164",
         "1001142001",
         "1001142002",
         "1001142003",
         "1001142004",
         "1001142006",
         "1001142007",
         "1001143001",
         "1001143002",
         "1001143003",
         "1001143004",
         "1001143006",
         "1001143007",
         "1001153001",
         "1001153002",
         "1001154001",
         "1001154002",
         "1001154003",
         "1001154004",
         "1001154005",
         "1001154006",
         "1001154007",
         "1001179001",
         "1001179002",
         "1001180001",
         "1001180002",
         "1001297001",
         "1001297002",
         "1001297003",
         "1001404001",
         "1001404002",
         "1001404003"
        };

    }
}