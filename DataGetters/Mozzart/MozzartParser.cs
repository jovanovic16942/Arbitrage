using Arbitrage.General;
using Arbitrage.Utils;
using NLog;

namespace Arbitrage.DataGetters.Mozzart
{
    public class MozzartParser : Parser
    {
        private readonly MozzartGetter _getter = new();

        /// <summary>
        /// This maps json response match ID to our classes, this is a special case for Mozzart
        /// </summary>
        private readonly Dictionary<int, HouseMatchData> _mozzartIdToMatchData = new();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        public MozzartParser() : base(BettingHouse.Mozzart) {}

        protected override void ParseFootball()
        {
            ParseSportMatches(1, betGameFromIDFootball);
        }

        protected override void ParseBasketball()
        {
            ParseSportMatches(2, betGameFromIDBasketball);
        }

        protected void ParseSportMatches(int sportId, Dictionary<string, BetGame> betGameFromID)
        {
            // Call GetMatches and check responses
            JsonMatchResponse matchResponse;
            try
            {
                matchResponse = _getter.GetMatches(sportId);
            } catch (Exception ex)
            {
                log.Error("Exception thrown in Getter: " + ex.Message);
                return;
            }

            // removed matches that expect n+ betGames in one ticket
            matchResponse.Matches = matchResponse.Matches.Where(x => x.SpecialType == 0).ToList();

            if (matchResponse == null || matchResponse.Matches == null)
            {
                log.Error("Failed to get match response!");
                return;
            }

            else if (matchResponse.Total == 0 || matchResponse.Matches.Count == 0)
            {
                log.Warn("Match response contains no matches!");
                return;
            }

            ParseMatches(matchResponse, sportId);

            var matchIDs = _mozzartIdToMatchData.Keys;
            var oddsResponse = _getter.GetOdds(matchIDs, betGameFromID.Keys.ToList());
            ParseOdds(oddsResponse, betGameFromID);

            foreach (var matchData in _mozzartIdToMatchData.Values)
            {
                _parsedData.Add(matchData);
            }
        }

        private void ParseMatches(JsonMatchResponse resp, int sportId)
        {
            foreach (var match in resp.Matches)
            {
                try
                {
                    var participants = match.Participants.ToList();
                    if (participants.Count != 2) continue;

                    var p1 = participants[0];
                    var p2 = participants[1];

                    DateTime startTime = DateTimeConverter.DateTimeFromLong(match.StartTime, 1); // TODO SUUUSSSSSS  Promena sata daylight savings bblabla

                    // TODO get sport from match.Competition.Sport.Id
                    HouseMatchData matchData = new(BettingHouse.Mozzart, sportFromId[sportId], startTime, p1.Name, p2.Name);

                    _mozzartIdToMatchData.Add(match.Id, matchData);
                }
                catch (Exception e)
                {
                    log.Error("Exception while parsing match response: " + e);
                }
            }
        }
        private void ParseOdds(List<JsonRoot> resp, Dictionary<string, BetGame> betGameFromID)
        {
            foreach (var matchOdds in resp)
            {
                try
                {
                    var matchId = matchOdds.id;
                    var kodds = matchOdds.kodds;

                    foreach (var kvp in kodds)
                    {
                        try
                        {
                            string subGameID = kvp.Key.Trim(); // Mozzart.com subGameID - trenutno subgamesIds u MozzartGetter
                            JsonKodds kodd = kvp.Value;

                            if (kodd == null) continue;

                            double betValue = double.Parse(kodd.value); // Kvota

                            if (!betGameFromID.ContainsKey(subGameID))
                            {
                                log.Trace("Unknown betting game: " + kvp.ToString());
                                continue;
                            }

                            BetGame game = betGameFromID[subGameID].Clone();
                            game.Value = betValue;

                            _mozzartIdToMatchData[matchId].AddBetGame(game);
                        }
                        catch (Exception e)
                        {
                            log.Error("Exception while parsing match bet game: " + e);
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("Exception while parsing match odds response: " + e);
                }
            }
        }

        /// <summary>
        /// Map sportId from json response si to sport enum
        /// </summary>
        static readonly Dictionary<int, Sport> sportFromId = new()
        {
            {1, Sport.Football},
            {2, Sport.Basketball},
        };

        static readonly Dictionary<string, BetGame> betGameFromIDFootball = new()
        {
            {"1001130001", new(BetGameType.GG) },
            {"1001130002", new(BetGameType.NG) },
            {"1001130004", new(BetGameType.GG, GamePeriod.H1) },
            {"1001130010", new(BetGameType.NG, GamePeriod.H1) },
            {"1001130005", new(BetGameType.GG, GamePeriod.H2) },
            {"1001130011", new(BetGameType.NG, GamePeriod.H2) },

            {"1001001001", new(BetGameType.WX1) },
            {"1001001002", new(BetGameType.WXX) },
            {"1001001003", new(BetGameType.WX2) },
            {"1001002002", new(BetGameType.D12) },
            {"1001002001", new(BetGameType.D1X) },
            {"1001002003", new(BetGameType.DX2) },

            {"1001004001", new(BetGameType.WX1, GamePeriod.H1) },
            {"1001004002", new(BetGameType.WXX, GamePeriod.H1) },
            {"1001004003", new(BetGameType.WX2, GamePeriod.H1) },
            {"1001297002", new(BetGameType.D12, GamePeriod.H1) },
            {"1001297001", new(BetGameType.D1X, GamePeriod.H1) },
            {"1001297003", new(BetGameType.DX2, GamePeriod.H1) },

            {"1001019001", new(BetGameType.WX1, GamePeriod.H2) },
            {"1001019002", new(BetGameType.WXX, GamePeriod.H2) },
            {"1001019003", new(BetGameType.WX2, GamePeriod.H2) },
            //{"", new(BetGameType.D12, GamePeriod.H2) },
            //{"", new(BetGameType.D1X, GamePeriod.H2) },
            //{"", new(BetGameType.DX2, GamePeriod.H2) },

            {"1001003001", new(BetGameType.UNDER, thr: 1.5) },
            {"1001003002", new(BetGameType.UNDER, thr: 2.5) },
            {"1001003013", new(BetGameType.UNDER, thr: 3.5) },
            {"1001003026", new(BetGameType.UNDER, thr: 4.5) },
            {"1001003012", new(BetGameType.OVER, thr: 1.5) },
            {"1001003004", new(BetGameType.OVER, thr: 2.5) },
            {"1001003005", new(BetGameType.OVER, thr: 3.5) },
            {"1001003007", new(BetGameType.OVER, thr: 4.5) },

            {"1001008008", new(BetGameType.UNDER, GamePeriod.H1, thr: 0.5) },
            {"1001008005", new(BetGameType.UNDER, GamePeriod.H1, thr: 1.5) },
            {"1001008009", new(BetGameType.UNDER, GamePeriod.H1, thr: 2.5) },
            {"1001008001", new(BetGameType.OVER, GamePeriod.H1, thr: 0.5) },
            {"1001008002", new(BetGameType.OVER, GamePeriod.H1, thr: 1.5) },
            {"1001008003", new(BetGameType.OVER, GamePeriod.H1, thr: 2.5) },

            {"1001009008", new(BetGameType.UNDER, GamePeriod.H2, thr: 0.5) },
            {"1001009005", new(BetGameType.UNDER, GamePeriod.H2, thr: 1.5) },
            {"1001009009", new(BetGameType.UNDER, GamePeriod.H2, thr: 2.5) },
            {"1001009001", new(BetGameType.OVER, GamePeriod.H2, thr: 0.5) },
            {"1001009002", new(BetGameType.OVER, GamePeriod.H2, thr: 1.5) },
            {"1001009003", new(BetGameType.OVER, GamePeriod.H2, thr: 2.5) },


            {"1001131003", new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 0.5) },
            {"1001131009", new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 1.5) },
            {"1001131010", new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 2.5) },
            {"1001131001", new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 0.5) },
            {"1001131002", new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 1.5) },
            {"1001131008", new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 2.5) },

            {"1001128003", new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 0.5) },
            {"1001128004", new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 1.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 2.5) },
            {"1001128001", new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 0.5) },
            {"1001128002", new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 1.5) },
            {"1001128007", new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 2.5) },

            {"1001142003", new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 0.5) },
            {"1001142004", new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 1.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 2.5) },
            {"1001142001", new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 0.5) },
            {"1001142002", new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 1.5) },
            {"1001142007", new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 2.5) },

            {"1001132003", new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 0.5) },
            {"1001132009", new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 1.5) },
            {"1001132010", new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 2.5) },
            {"1001132001", new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 0.5) },
            {"1001132002", new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 1.5) },
            {"1001132008", new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 2.5) },

            {"1001129003", new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 0.5) },
            {"1001129004", new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 1.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 2.5) },
            {"1001129001", new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 0.5) },
            {"1001129002", new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 1.5) },
            {"1001129007", new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 2.5) },

            {"1001143003", new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 0.5) },
            {"1001143004", new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 1.5) },
            {"1001143005", new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 2.5) },
            {"1001143001", new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 0.5) },
            {"1001143002", new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 1.5) },
            {"1001143007", new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 2.5) },
        };


        static readonly Dictionary<string, BetGame> betGameFromIDBasketball = new()
        {
            //{2291, new(BetGameType.W1_X_0, GamePeriod.H1) },
            //{2292, new(BetGameType.W2_X_0, GamePeriod.H1) },
            //{2293, new(BetGameType.W1_X_0, GamePeriod.H2) },
            //{2294, new(BetGameType.W2_X_0, GamePeriod.H2) },

            //{2355, new(BetGameType.W1_X_0, GamePeriod.Q1) },
            //{2356, new(BetGameType.W2_X_0, GamePeriod.Q1) },
            //{2357, new(BetGameType.W1_X_0, GamePeriod.Q2) },
            //{2358, new(BetGameType.W2_X_0, GamePeriod.Q2) },
            //{2359, new(BetGameType.W1_X_0, GamePeriod.Q3) },
            //{2360, new(BetGameType.W2_X_0, GamePeriod.Q3) },
            //{2361, new(BetGameType.W1_X_0, GamePeriod.Q4) },
            //{2362, new(BetGameType.W2_X_0, GamePeriod.Q4) },

            //{2371, new(BetGameType.W1_X_0, GamePeriod.Q1, Team.T1) },
            //{2372, new(BetGameType.W2_X_0, GamePeriod.Q1, Team.T1) },

            {"1002196001", new(BetGameType.W1) },
            {"1002196003", new(BetGameType.W2) },

            {"1002017001", new(BetGameType.WX1) },
            {"1002017002", new(BetGameType.WXX) },
            {"1002017003", new(BetGameType.WX2) },
            {"1002002001", new(BetGameType.D1X) },
            //{"", new(BetGameType.D12) },
            {"1002002003", new(BetGameType.DX2) },


            {"1002025001", new(BetGameType.WX1, GamePeriod.H1) },
            {"1002025002", new(BetGameType.WXX, GamePeriod.H1) },
            {"1002025003", new(BetGameType.WX2, GamePeriod.H1) },
            {"1002560001", new(BetGameType.D1X, GamePeriod.H1) },
            //{"", new(BetGameType.D12, GamePeriod.H1) },
            {"1002560002", new(BetGameType.DX2, GamePeriod.H1) },
            //{, new(BetGameType.WX1, GamePeriod.H2) },
            //{, new(BetGameType.WXX, GamePeriod.H2) },
           // {, new(BetGameType.WX2, GamePeriod.H2) },

            //{772, new(BetGameType.WX1, GamePeriod.Q1) },
            //{773, new(BetGameType.WXX, GamePeriod.Q1) },
            //{774, new(BetGameType.WX2, GamePeriod.Q1) },
            //{784, new(BetGameType.WX1, GamePeriod.Q2) },
            //{785, new(BetGameType.WXX, GamePeriod.Q2) },
            //{786, new(BetGameType.WX2, GamePeriod.Q2) },
            //{792, new(BetGameType.WX1, GamePeriod.Q3) },
            //{793, new(BetGameType.WXX, GamePeriod.Q3) },
            //{794, new(BetGameType.WX2, GamePeriod.Q3) },
            //{795, new(BetGameType.WX1, GamePeriod.Q4) },
            //{796, new(BetGameType.WXX, GamePeriod.Q4) },
            //{797, new(BetGameType.WX2, GamePeriod.Q4) },
            //{"", new(BetGameType.WX1, GamePeriod.QB) },
            //{796, new(BetGameType.WXX, GamePeriod.QB) },
            //{797, new(BetGameType.WX2, GamePeriod.QB) },

            {"1002027001", new(BetGameType.UNDER)}, // + OT
            {"1002027003", new(BetGameType.OVER)},
            {"1002744001", new(BetGameType.UNDER)},
            {"1002744002", new(BetGameType.OVER)},
            {"1002745001", new(BetGameType.UNDER)},
            {"1002745002", new(BetGameType.OVER)},
            {"1002746001", new(BetGameType.UNDER)},
            {"1002746002", new(BetGameType.OVER)},
            {"1002747001", new(BetGameType.UNDER)},
            {"1002747002", new(BetGameType.OVER)},
            {"1002748001", new(BetGameType.UNDER)},
            {"1002748002", new(BetGameType.OVER)},
            {"1002749001", new(BetGameType.UNDER)},
            {"1002749002", new(BetGameType.OVER)},
            {"1002750001", new(BetGameType.UNDER)},
            {"1002750002", new(BetGameType.OVER)},
            {"1002751001", new(BetGameType.UNDER)},
            {"1002751002", new(BetGameType.OVER)},
            {"1002752001", new(BetGameType.UNDER)},
            {"1002752002", new(BetGameType.OVER)},
            {"1002753001", new(BetGameType.UNDER)},
            {"1002753002", new(BetGameType.OVER)},
            {"1002754001", new(BetGameType.UNDER)},
            {"1002754002", new(BetGameType.OVER)},
            {"1002755001", new(BetGameType.UNDER)},
            {"1002755002", new(BetGameType.OVER)},
            {"1002756001", new(BetGameType.UNDER)},
            {"1002756002", new(BetGameType.OVER)},
            {"1002757001", new(BetGameType.UNDER)},
            {"1002757002", new(BetGameType.OVER)},
            {"1002762001", new(BetGameType.UNDER)},
            {"1002762002", new(BetGameType.OVER)},
            {"1002763001", new(BetGameType.UNDER)},
            {"1002763002", new(BetGameType.OVER)},
            //{"", new(BetGameType.UNDER)},
            //{"", new(BetGameType.OVER)},
            //{"", new(BetGameType.UNDER)},
            //{"", new(BetGameType.OVER)},


            {"1002181001", new(BetGameType.UNDER, GamePeriod.M, Team.T1)},
            {"1002181002", new(BetGameType.OVER, GamePeriod.M, Team.T1)},
            {"1002183001", new(BetGameType.UNDER, GamePeriod.M, Team.T2)},
            {"1002183002", new(BetGameType.OVER, GamePeriod.M, Team.T2)},

            {"1002028001", new(BetGameType.UNDER, GamePeriod.H1)},
            {"1002028003", new(BetGameType.OVER, GamePeriod.H1)},
            {"1002150001", new(BetGameType.UNDER, GamePeriod.H2)},
            {"1002150003", new(BetGameType.OVER, GamePeriod.H2)},
            {"1002182001", new(BetGameType.UNDER, GamePeriod.H1, Team.T1)},
            {"1002182002", new(BetGameType.OVER, GamePeriod.H1, Team.T1)},
            //{"", new(BetGameType.UNDER, GamePeriod.H2, Team.T1)},
            //{"", new(BetGameType.OVER, GamePeriod.H2, Team.T1)},
            {"1002184001", new(BetGameType.UNDER, GamePeriod.H1, Team.T2)},
            {"1002184002", new(BetGameType.OVER, GamePeriod.H1, Team.T2)},
            //{"", new(BetGameType.UNDER, GamePeriod.H2, Team.T2)},
            //{"", new(BetGameType.OVER, GamePeriod.H2, Team.T2)},

            {"1002187001", new(BetGameType.UNDER, GamePeriod.Q1)},
            {"1002187002", new(BetGameType.OVER, GamePeriod.Q1)},
            //{787, new(BetGameType.UNDER, GamePeriod.Q2)},
            //{788, new(BetGameType.OVER, GamePeriod.Q2)},
            //{800, new(BetGameType.UNDER, GamePeriod.Q3)},
            //{801, new(BetGameType.OVER, GamePeriod.Q3)},
            //{825, new(BetGameType.UNDER, GamePeriod.Q4)},
            //{826, new(BetGameType.OVER, GamePeriod.Q4)},
            {"1002562001", new(BetGameType.UNDER, GamePeriod.QB)},
            {"1002562002", new(BetGameType.OVER, GamePeriod.QB)},

            //{2373, new(BetGameType.UNDER, GamePeriod.Q1, Team.T2)},
            //{2374, new(BetGameType.OVER, GamePeriod.Q1, Team.T2)},
        };
    }
}
