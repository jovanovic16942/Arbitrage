using Arbitrage.General;
using Arbitrage.Utils;

namespace Arbitrage.DataGetters.SuperBet
{
    public class SuperBetParser : Parser
    {
        private readonly SuperBetGetter _getter = new();

        JsonMatchResponse? _matchResponse = null;

        public SuperBetParser() : base(BettingHouse.SuperBet) 
        {
            _matchResponse = _getter.GetMatches();
        }

        protected override void ParseFootball()
        {
            _matchResponse ??= _getter.GetMatches();

            var footballMatches = _matchResponse.data.Where(x => x.si == 5).ToList();

            ParseMatches(footballMatches);
        }

        protected override void ParseBasketball()
        {
            _matchResponse ??= _getter.GetMatches();

            var basketballMatches = _matchResponse.data.Where(x => x.si == 4).ToList();

            ParseMatches(basketballMatches);
        }

        private void ParseMatches(List<JsonMatch> matches)
        {
            List<int> matchIds = matches.Select(x => x._id).ToList();

            int step = 20;

            for (int i = 0; (i * step) < matchIds.Count; i++)
            {
                int total = step * i;

                var fullResponse = _getter.GetMatchData(matchIds.Skip(total).Take(step));

                foreach (var jsonMatch in fullResponse.data)
                {
                    try
                    {
                        ParseJsonMatch(jsonMatch);
                    }
                    catch (Exception e)
                    {
                        // Loggovanje
                    }
                }
            }
        }

        void ParseJsonMatch(JsonMatch jsonMatch)
        {
            if (jsonMatch == null) return;

            DateTime startTime = DateTime.Parse(jsonMatch.mld);
            List<string> teams = jsonMatch.mn.Split("·").Select(x => x.Trim()).ToList();

            HouseMatchData matchData = new(House, sportFromId[jsonMatch.si], startTime, teams[0], teams[1]);

            foreach(var jsonOdd in jsonMatch.odds)
            {
                int betGameId = jsonOdd.oi;
                if (!betGameFromOI.ContainsKey(betGameId)) continue;

                double? thr = null;

                // Special parsing for UNDER - OVER
                if (betGameId == 151888 || betGameId == 151889) 
                {
                    thr = (int)Math.Truncate(jsonOdd.spc.total);
                }

                BetGameConfig cfg = betGameFromOI[betGameId];
                //cfg.threshold = thr;

                BetGame bg = new(cfg)
                {
                    Value = jsonOdd.ov
                };
                matchData.AddBetGame(bg);
            }

            _parsedData.Add(matchData);
        }

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
        static readonly Dictionary<int, BetGameConfig> betGameFromOI = new()
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
            // TODO Over - under per teams and halftimes-quarters
            //{, new(BetGameType.UNDER, GamePeriod.H1)},
            //{, new(BetGameType.OVER, GamePeriod.H1)},
            //{, new(BetGameType.UNDER, GamePeriod.H2)},
            //{, new(BetGameType.OVER, GamePeriod.H2)},
            //{, new(BetGameType.UNDER, GamePeriod.M, Team.T1)},
            //{, new(BetGameType.OVER, GamePeriod.M, Team.T1)},
            //{, new(BetGameType.UNDER, GamePeriod.M, Team.T2)},
            //{, new(BetGameType.OVER, GamePeriod.M, Team.T2)},
            //{, new(BetGameType.UNDER, GamePeriod.H1, Team.T1)},
            //{, new(BetGameType.OVER, GamePeriod.H1, Team.T1)},
            //{, new(BetGameType.UNDER, GamePeriod.H1, Team.T2)},
            //{, new(BetGameType.OVER, GamePeriod.H1, Team.T2)},
            //{, new(BetGameType.UNDER, GamePeriod.H2, Team.T1)},
            //{, new(BetGameType.OVER, GamePeriod.H2, Team.T1)},
            //{, new(BetGameType.UNDER, GamePeriod.H2, Team.T2)},
            //{, new(BetGameType.OVER, GamePeriod.H2, Team.T2)},
        };

        /// <summary>
        /// Map betGames from json response oi (odd id_str) to BettingGames enum
        /// </summary>
        static readonly Dictionary<int, BettingGames> betGameFromOIOld = new()
        {
            {1470, BettingGames._1 },
            {1471, BettingGames._X },
            {1472, BettingGames._2 },
            {1364, BettingGames._12 },
            {1363, BettingGames._1X },
            {1365, BettingGames._X2 },
            {1440, BettingGames._GG },
            {1441, BettingGames._NG },
            //
            {1520, BettingGames._1_I },
            {1521, BettingGames._X_I },
            {1522, BettingGames._2_I },
            {1492, BettingGames._12_I},
            {1491, BettingGames._1X_I},
            {1493, BettingGames._X2_I},
            {1527, BettingGames._GG_I},
            {1528, BettingGames._NG_I},
            //
            {1511, BettingGames._1_II },
            {1512, BettingGames._X_II },
            {1513, BettingGames._2_II },
            {1570, BettingGames._12_II},
            {1569, BettingGames._1X_II},
            {1571, BettingGames._X2_II},
            {1475, BettingGames._GG_II},
            {1476, BettingGames._NG_II},
            //

            {151888, BettingGames._UG_0 },
            {151889, BettingGames._UG_1_PLUS },
            {1151888, BettingGames._UG_0_1 },
            {1151889, BettingGames._UG_2_PLUS },
            {2151888, BettingGames._UG_0_2 },
            {2151889, BettingGames._UG_3_PLUS },
            {3151888, BettingGames._UG_0_3 },
            {3151889, BettingGames._UG_4_PLUS },
            {4151888, BettingGames._UG_0_4 },
            {4151889, BettingGames._UG_5_PLUS },
            {5151888, BettingGames._UG_0_5 },
            {5151889, BettingGames._UG_6_PLUS },
            {6151888, BettingGames._UG_0_6 },
            {6151889, BettingGames._UG_7_PLUS }
        };
    }
}
