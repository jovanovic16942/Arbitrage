using Arbitrage.General;
using Arbitrage.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.PinnBet
{
    public class PinnBetParser : Parser
    {
        private readonly PinnBetGetter _getter = new();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public PinnBetParser() : base(BettingHouse.PinnBet) { }

        private void ParseMatchEvent(JsonEvent matchEvent, List<JsonBet> bets)
        {
            if (matchEvent.isTopOffer) { return; } // Skip special offfers

            DateTime startTime = DateTime.Parse(matchEvent.dateTime).AddHours(1); // TODO SUSS

            var teams = matchEvent.name.Split('-').Select(x => x.Trim()).ToList();

            if (teams.Count < 2) { return; } // For now skip matches with < 2 teams

            HouseMatchData matchData = new(House, SportFromId[matchEvent.sportId], startTime, teams[0], teams[1]);

            // Combine bets from both responses
            var allBets = bets.Concat(matchEvent.bets);

            // Add odds
            foreach (var betGame in allBets)
            {
                foreach (var outcome in betGame.betOutcomes)
                {
                    int betGameId = outcome.betTypeOutcomeId;

                    if (!betGameFromInt.Keys.Contains(betGameId)) continue;

                    BetGame game = betGameFromInt[betGameId].Clone();

                    if (double.TryParse(outcome.sBV, out double thr))
                    {
                        game.SetThreshold(thr);
                    }

                    game.Value = outcome.odd;
                    matchData.AddBetGame(game);
                }
            }

            _parsedData.Add(matchData);
        }

        private void ParseMatchResponse(JsonMatchResponse response)
        {
            foreach (var competition in response.competitions)
            {
                if (competition.events == null || competition.events.Count == 0) { continue; }

                foreach (var matchEvent in competition.events)
                {
                    if (matchEvent == null) { continue; }
                    try
                    {
                        var oddsEvent = _getter.GetBets(competition.competitionId, matchEvent.id, competition.regionId, competition.events[0].sportId);
                        ParseMatchEvent(matchEvent, oddsEvent.bets);
                    }
                    catch (Exception ex)
                    {
                        // Get some odds but not all
                        log.Error("Source: " + ex.Source + " Message: " + ex.Message);
                        ParseMatchEvent(matchEvent, matchEvent.bets);
                    }
                }
            }
        }
        protected override void ParseFootball()
        {
            var matchResponses = _getter.GetMatches(1);

            foreach (var matchResponse in matchResponses)
            {
                ParseMatchResponse(matchResponse);
            }
        }

        protected override void ParseBasketball()
        {
            var matchResponses = _getter.GetMatches(2);

            foreach (var matchResponse in matchResponses)
            {
                ParseMatchResponse(matchResponse);
            }
        }

        static readonly Dictionary<int, Sport> SportFromId = new()
        {
            {1, Sport.Football },
            {2, Sport.Basketball },
        };

        /// <summary>
        /// Map betGames from json response bet type outcome id_str to BetGameConfig
        /// over-under (total goals) odds have special rules
        /// </summary>
        static readonly Dictionary<int, BetGame> betGameFromInt = new()
        {
            /// BASKETBALL
            {1000, new(BetGameType.W1_X_0, GamePeriod.H1) },
            {1001, new(BetGameType.W2_X_0, GamePeriod.H1) },
            {1029, new(BetGameType.W1_X_0, GamePeriod.H2) },
            {1030, new(BetGameType.W2_X_0, GamePeriod.H2) },

            {966, new(BetGameType.W1_X_0, GamePeriod.Q1) },
            {967, new(BetGameType.W2_X_0, GamePeriod.Q1) },
            {984, new(BetGameType.W1_X_0, GamePeriod.Q2) },
            {985, new(BetGameType.W2_X_0, GamePeriod.Q2) },
            {1015, new(BetGameType.W1_X_0, GamePeriod.Q3) },
            {1016, new(BetGameType.W2_X_0, GamePeriod.Q3) },
            {1044, new(BetGameType.W1_X_0, GamePeriod.Q4) },
            {1045, new(BetGameType.W2_X_0, GamePeriod.Q4) },

            {954, new(BetGameType.W1) },
            {955, new(BetGameType.W2) },

            {956, new(BetGameType.D1X) },
            {957, new(BetGameType.DX2) },

            {1004, new(BetGameType.D1X, GamePeriod.H1) },
            {1005, new(BetGameType.DX2, GamePeriod.H1) },

            {1039, new(BetGameType.D1X, GamePeriod.H2) },
            {1040, new(BetGameType.DX2, GamePeriod.H2) },

            {972, new(BetGameType.WX1, GamePeriod.Q1) },
            {973, new(BetGameType.WXX, GamePeriod.Q1) },
            {974, new(BetGameType.WX2, GamePeriod.Q1) },
            {975, new(BetGameType.D1X, GamePeriod.Q1) },
            {976, new(BetGameType.DX2, GamePeriod.Q1) },

            {981, new(BetGameType.WX1, GamePeriod.Q2) },
            {982, new(BetGameType.WXX, GamePeriod.Q2) },
            {983, new(BetGameType.WX2, GamePeriod.Q2) },
            {988, new(BetGameType.D1X, GamePeriod.Q2) },
            {989, new(BetGameType.DX2, GamePeriod.Q2) },
            {1012, new(BetGameType.WX1, GamePeriod.Q3) },
            {1013, new(BetGameType.WXX, GamePeriod.Q3) },
            {1014, new(BetGameType.WX2, GamePeriod.Q3) },
            {1027, new(BetGameType.D1X, GamePeriod.Q3) },
            {1028, new(BetGameType.DX2, GamePeriod.Q3) },
            {1041, new(BetGameType.WX1, GamePeriod.Q4) },
            {1042, new(BetGameType.WXX, GamePeriod.Q4) },
            {1043, new(BetGameType.WX2, GamePeriod.Q4) },
            {1056, new(BetGameType.D1X, GamePeriod.Q4) },
            {1057, new(BetGameType.DX2, GamePeriod.Q4) },


            {960, new(BetGameType.UNDER)}, // + OT
            {961, new(BetGameType.OVER)},

            // TODO over-under without overtime
            //{1548, new(BetGameType.UNDER) }
            //{1549, new(BetGameType.OVER) }
            //These ids are correct



            {962, new(BetGameType.UNDER, GamePeriod.M, Team.T1)},
            {963, new(BetGameType.OVER, GamePeriod.M, Team.T1)},
            {964, new(BetGameType.UNDER, GamePeriod.M, Team.T2)},
            {965, new(BetGameType.OVER, GamePeriod.M, Team.T2)},

            {1008, new(BetGameType.UNDER, GamePeriod.H1, Team.T1)},
            {1009, new(BetGameType.OVER, GamePeriod.H1, Team.T1)},
            {1010, new(BetGameType.UNDER, GamePeriod.H1, Team.T2)},
            {1011, new(BetGameType.OVER, GamePeriod.H1, Team.T2)},

            {1035, new(BetGameType.UNDER, GamePeriod.H2, Team.T1)},
            {1036, new(BetGameType.OVER, GamePeriod.H2, Team.T1)},
            {1037, new(BetGameType.UNDER, GamePeriod.H2, Team.T2)},
            {1038, new(BetGameType.OVER, GamePeriod.H2, Team.T2)},


            {1006, new(BetGameType.UNDER, GamePeriod.H1)},
            {1007, new(BetGameType.OVER, GamePeriod.H1)},
            {1033, new(BetGameType.UNDER, GamePeriod.H2)},
            {1034, new(BetGameType.OVER, GamePeriod.H2)},

            {970, new(BetGameType.UNDER, GamePeriod.Q1)},
            {971, new(BetGameType.OVER, GamePeriod.Q1)},
            {990, new(BetGameType.UNDER, GamePeriod.Q2)},
            {991, new(BetGameType.OVER, GamePeriod.Q2)},
            {1019, new(BetGameType.UNDER, GamePeriod.Q3)},
            {1020, new(BetGameType.OVER, GamePeriod.Q3)},
            {1048, new(BetGameType.UNDER, GamePeriod.Q4)},
            {1049, new(BetGameType.OVER, GamePeriod.Q4)},
            {1097, new(BetGameType.UNDER, GamePeriod.QB)},
            {1098, new(BetGameType.OVER, GamePeriod.QB)},


            {977, new(BetGameType.UNDER, GamePeriod.Q1, Team.T1)},
            {978, new(BetGameType.OVER, GamePeriod.Q1, Team.T1)},
            {979, new(BetGameType.UNDER, GamePeriod.Q1, Team.T2)},
            {980, new(BetGameType.OVER, GamePeriod.Q1, Team.T2)},

            {992, new(BetGameType.UNDER, GamePeriod.Q2, Team.T1)},
            {993, new(BetGameType.OVER, GamePeriod.Q2, Team.T1)},
            {994, new(BetGameType.UNDER, GamePeriod.Q2, Team.T2)},
            {995, new(BetGameType.OVER, GamePeriod.Q2, Team.T2)},

            {1023, new(BetGameType.UNDER, GamePeriod.Q3, Team.T1)},
            {1024, new(BetGameType.OVER, GamePeriod.Q3, Team.T1)},
            {1025, new(BetGameType.UNDER, GamePeriod.Q3, Team.T2)},
            {1026, new(BetGameType.OVER, GamePeriod.Q3, Team.T2)},

            {1052, new(BetGameType.UNDER, GamePeriod.Q4, Team.T1)},
            {1053, new(BetGameType.OVER, GamePeriod.Q4, Team.T1)},
            {1054, new(BetGameType.UNDER, GamePeriod.Q4, Team.T2)},
            {1055, new(BetGameType.OVER, GamePeriod.Q4, Team.T2)},


            /// FOOTBALL

            {34, new(BetGameType.W1_X_0) },
            {35, new(BetGameType.W2_X_0) },

            //// 1 X 2 - Dupla sansa
            {1, new(BetGameType.WX1) },
            {2, new(BetGameType.WXX) },
            {3, new(BetGameType.WX2) },
            {6, new(BetGameType.D1X) },
            {7, new(BetGameType.D12) },
            {8, new(BetGameType.DX2) },

            //// 1 X 2 - Dupla sansa - H1
            {15, new(BetGameType.WX1, GamePeriod.H1) },
            {16, new(BetGameType.WXX, GamePeriod.H1) },
            {17, new(BetGameType.WX2, GamePeriod.H1) },
            {9, new(BetGameType.D1X, GamePeriod.H1) },
            {10, new(BetGameType.D12, GamePeriod.H1) },
            {11, new(BetGameType.DX2, GamePeriod.H1) },

            {30, new(BetGameType.W1_X_0, GamePeriod.H1) },
            {31, new(BetGameType.W2_X_0, GamePeriod.H1) },

            //// 1 X 2 - Dupla sansa - H2
            {18, new(BetGameType.WX1, GamePeriod.H2) },
            {19, new(BetGameType.WXX, GamePeriod.H2) },
            {20, new(BetGameType.WX2, GamePeriod.H2) },
            {12, new(BetGameType.D1X, GamePeriod.H2) },
            {13, new(BetGameType.D12, GamePeriod.H2) },
            {14, new(BetGameType.DX2, GamePeriod.H2) },

            {32, new(BetGameType.W1_X_0, GamePeriod.H2) },
            {33, new(BetGameType.W2_X_0, GamePeriod.H2) },

            {36, new(BetGameType.GG) },
            {37, new(BetGameType.NG) },
            {467, new(BetGameType.GG, GamePeriod.H1) },
            {468, new(BetGameType.NG, GamePeriod.H1) },
            {469, new(BetGameType.GG, GamePeriod.H2) },
            {470, new(BetGameType.NG, GamePeriod.H2) },

            //// over - under 
            {4, new(BetGameType.UNDER)},
            {5, new(BetGameType.OVER)},
            {76, new(BetGameType.UNDER, GamePeriod.H1)},
            {77, new(BetGameType.OVER, GamePeriod.H1)},
            {96, new(BetGameType.UNDER, GamePeriod.H2)},
            {97, new(BetGameType.OVER, GamePeriod.H2)},

            //// over - under per team
            {98, new(BetGameType.UNDER, GamePeriod.M, Team.T1)},
            {99, new(BetGameType.OVER, GamePeriod.M, Team.T1)},
            {100, new(BetGameType.UNDER, GamePeriod.M, Team.T2)},
            {101, new(BetGameType.OVER, GamePeriod.M, Team.T2)},

            {126, new(BetGameType.UNDER, GamePeriod.H1, Team.T1)},
            {127, new(BetGameType.OVER, GamePeriod.H1, Team.T1)},
            {134, new(BetGameType.UNDER, GamePeriod.H1, Team.T2)},
            {135, new(BetGameType.OVER, GamePeriod.H1, Team.T2)},

            {139, new(BetGameType.UNDER, GamePeriod.H2, Team.T1)},
            {140, new(BetGameType.OVER, GamePeriod.H2, Team.T1)},
            {141, new(BetGameType.UNDER, GamePeriod.H2, Team.T2)},
            {142, new(BetGameType.OVER, GamePeriod.H2, Team.T2)},
        };
    }
}
