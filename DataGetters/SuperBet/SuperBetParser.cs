using Arbitrage.General;
using Arbitrage.Utils;

namespace Arbitrage.DataGetters.SuperBet
{
    public class SuperBetParser : Parser
    {
        private readonly SuperBetGetter _getter = new();

        JsonMatchResponse? _matchResponse = null;

        public SuperBetParser() : base(BettingHouse.SuperBet) { }

        protected override void ParseFootball()
        {
            _matchResponse = _getter.GetMatches();

            var footballMatches = _matchResponse.data.Where(x => x.sportId == 5).ToList();

            ParseMatches(footballMatches);
        }

        protected override void ParseBasketball()
        {
            _matchResponse ??= _getter.GetMatches();

            var basketballMatches = _matchResponse.data.Where(x => x.sportId == 4).ToList();

            ParseMatches(basketballMatches);
        }

        private void ParseMatches(List<JsonMatchData> matches)
        {
            List<int> matchIds = matches.Select(x => x.eventId).ToList();

            foreach (var matchId in matchIds)
            {
                var matchResp = _getter.GetMatchResponse(matchId);
                try
                {
                    ParseJsonMatchResp(matchResp);
                } catch (Exception ex)
                {

                }
            }
        }

        private void ParseJsonMatchResp(JsonMatchResponse matchResp)
        {
            if (matchResp == null || matchResp.data == null || matchResp.data.Count != 1) return;

            var jsonMatch = matchResp.data[0];

            if (jsonMatch.odds == null) return;

            DateTime startTime = DateTime.Parse(jsonMatch.matchDate).AddHours(1);
            List<string> teams = jsonMatch.matchName.Split("·").Select(x => x.Trim()).ToList();

            var sport = sportFromId[jsonMatch.sportId];
            var bgMap = sportMaps[sport];
            HouseMatchData matchData = new(House, sport, startTime, teams[0], teams[1]);

            foreach(var jsonOdd in jsonMatch.odds)
            {
                try
                {
                    int betGameId = jsonOdd.outcomeId;
                    if (!bgMap.ContainsKey(betGameId)) continue;
                    BetGame game = bgMap[betGameId].Clone();

                    var spc = jsonOdd.specialBetValue;

                    if (spc != null && spc.Contains("-"))
                    {
                        spc = spc.Substring(spc.IndexOf("-") + 1);
                    }

                    if (game.period == GamePeriod.NONE)
                    {
                        game.SetPeriod(quarterFromSpecifier[jsonOdd.specifiers.quarternr]);
                    }

                    if (spc != null && double.TryParse(spc, out double thr))
                    {
                        game.SetThreshold(thr);
                    }

                    game.Value = jsonOdd.price;


                    matchData.AddBetGame(game);
                } catch (Exception e)
                {

                }
            }

            _parsedData.Add(matchData);
        }

        /// <summary>
        /// Map quarternr from json to GamePeriod
        /// </summary>
        static readonly Dictionary<string, GamePeriod> quarterFromSpecifier = new()
        {
            {"1", GamePeriod.Q1 },
            {"2", GamePeriod.Q2 },
            {"3", GamePeriod.Q3 },
            {"4", GamePeriod.Q4 },
        };

        /// <summary>
        /// Map sportId from json response si to sport enum
        /// </summary>
        static readonly Dictionary<int, Sport> sportFromId = new()
        {
            {5, Sport.Football},
            {4, Sport.Basketball},
        };

        
        /// <summary>
        /// Map betGames from json response oi (odd id_str) to BetGameConfig
        /// </summary>
        static readonly Dictionary<int, BetGame> bgFromOutcomeIdFootball = new()
        {
            {1470, new(BetGameType.WX1) },
            {1471, new(BetGameType.WXX) },
            {1472, new(BetGameType.WX2) },
            {1364, new(BetGameType.D12) },
            {1363, new(BetGameType.D1X) },
            {1365, new(BetGameType.DX2) },
            {1440, new(BetGameType.GG) },
            {1441, new(BetGameType.NG) },
            //
            {1520, new(BetGameType.WX1, GamePeriod.H1) },
            {1521, new(BetGameType.WXX, GamePeriod.H1) },
            {1522, new(BetGameType.WX2, GamePeriod.H1) },
            {1492, new(BetGameType.D12, GamePeriod.H1) },
            {1491, new(BetGameType.D1X, GamePeriod.H1) },
            {1493, new(BetGameType.DX2, GamePeriod.H1) },
            {1527, new(BetGameType.GG, GamePeriod.H1) },
            {1528, new(BetGameType.NG, GamePeriod.H1) },
            //
            {1511, new(BetGameType.WX1, GamePeriod.H2) },
            {1512, new(BetGameType.WXX, GamePeriod.H2) },
            {1513, new(BetGameType.WX2, GamePeriod.H2) },
            {1570, new(BetGameType.D12, GamePeriod.H2) },
            {1569, new(BetGameType.D1X, GamePeriod.H2) },
            {1571, new(BetGameType.DX2, GamePeriod.H2) },
            {1475, new(BetGameType.GG, GamePeriod.H2) },
            {1476, new(BetGameType.NG, GamePeriod.H2) },
            //

            {151888, new(BetGameType.UNDER)},
            {151889, new(BetGameType.OVER)},
            {151890, new(BetGameType.UNDER, GamePeriod.H1)},
            {151891, new(BetGameType.OVER, GamePeriod.H1)},
            {151896, new(BetGameType.UNDER, GamePeriod.H2)},
            {151897, new(BetGameType.OVER, GamePeriod.H2)},
            {1461, new(BetGameType.UNDER, GamePeriod.M, Team.T1)},
            {1462, new(BetGameType.OVER, GamePeriod.M, Team.T1)},
            {1406, new(BetGameType.UNDER, GamePeriod.M, Team.T2)},
            {1407, new(BetGameType.OVER, GamePeriod.M, Team.T2)},
            {6550, new(BetGameType.UNDER, GamePeriod.H1, Team.T1)},
            {6551, new(BetGameType.OVER, GamePeriod.H1, Team.T1)},
            {6554, new(BetGameType.UNDER, GamePeriod.H1, Team.T2)},
            {6555, new(BetGameType.OVER, GamePeriod.H1, Team.T2)},
            {4201740, new(BetGameType.UNDER, GamePeriod.H2, Team.T1)},
            {4201741, new(BetGameType.OVER, GamePeriod.H2, Team.T1)},
            {4201742, new(BetGameType.UNDER, GamePeriod.H2, Team.T2)},
            {4201743, new(BetGameType.OVER, GamePeriod.H2, Team.T2)},
        };


        static readonly Dictionary<int, BetGame> bgFromOutcomeIdBasket = new()
        {
            {2182, new(BetGameType.W1)},
            {2183, new(BetGameType.W2)},

            {2221, new(BetGameType.WX1)},
            {2222, new(BetGameType.WXX)},
            {2223, new(BetGameType.WX2)},
            {162139, new(BetGameType.D1X) },
            {162140, new(BetGameType.DX2) },

            {2190, new(BetGameType.WX1, GamePeriod.H1)},
            {2191, new(BetGameType.WXX, GamePeriod.H1)},
            {2192, new(BetGameType.WX2, GamePeriod.H1)},

            {162141, new(BetGameType.D1X, GamePeriod.H1) },
            {162142, new(BetGameType.DX2, GamePeriod.H1) },


            {2226, new(BetGameType.WX1, GamePeriod.NONE)},
            {2227, new(BetGameType.WXX, GamePeriod.NONE)},
            {2228, new(BetGameType.WX2, GamePeriod.NONE)},
            {4203505, new(BetGameType.D1X, GamePeriod.Q1) },
            {4203506, new(BetGameType.DX2, GamePeriod.Q1) },


            {2204, new(BetGameType.WX1, GamePeriod.H2O)},
            {2205, new(BetGameType.WXX, GamePeriod.H2O)},
            {2206, new(BetGameType.WX2, GamePeriod.H2O)},

            {2169, new(BetGameType.UNDER)},
            {2170, new(BetGameType.OVER)},
            {2159, new(BetGameType.UNDER, GamePeriod.H1)},
            {2160, new(BetGameType.OVER, GamePeriod.H1)},
            {2178, new(BetGameType.UNDER, GamePeriod.H2)},
            {2179, new(BetGameType.OVER, GamePeriod.H2)},
            {152243, new(BetGameType.UNDER, GamePeriod.H1, Team.T1)},
            {152244, new(BetGameType.OVER, GamePeriod.H1, Team.T1)},
            {152226, new(BetGameType.UNDER, GamePeriod.H1, Team.T2)},
            {152227, new(BetGameType.OVER, GamePeriod.H1, Team.T2)},

            {2260, new(BetGameType.UNDER, GamePeriod.NONE)},
            {2261, new(BetGameType.OVER, GamePeriod.NONE)},

            {152238, new(BetGameType.UNDER, GamePeriod.Q1, Team.T1)},
            {152239, new(BetGameType.OVER, GamePeriod.Q1, Team.T1)},
            {152222, new(BetGameType.UNDER, GamePeriod.Q1, Team.T2)},
            {152223, new(BetGameType.OVER, GamePeriod.Q1, Team.T2)},
        };

        static readonly Dictionary<Sport, Dictionary<int, BetGame>> sportMaps = new()
        {
            { Sport.Football, bgFromOutcomeIdFootball },
            { Sport.Basketball, bgFromOutcomeIdBasket },
        };
    }
}
