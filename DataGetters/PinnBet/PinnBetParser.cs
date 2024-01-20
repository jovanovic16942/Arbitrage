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

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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

                    var id = matchEvent.id;

                    try
                    {
                        var oddsEvent = _getter.GetBets(competition.competitionId, matchEvent.id, competition.regionId, competition.events[0].sportId);
                        ParseMatchEvent(matchEvent, oddsEvent.bets);
                    }
                    catch (Exception ex)
                    {
                        // Get some odds but not all
                        logger.Error("Source: " + ex.Source + " Message: " + ex.Message);
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

            //{746, new(BetGameType.W1) },
            //{747, new(BetGameType.W2) },

            //{754, new(BetGameType.WX1) },
            //{755, new(BetGameType.WXX) },
            //{756, new(BetGameType.WX2) },

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


            //{821, new(BetGameType.UNDER)}, // + OT
            //{822, new(BetGameType.OVER)},
            //{2299, new(BetGameType.UNDER, GamePeriod.M, Team.T1)},
            //{2300, new(BetGameType.OVER, GamePeriod.M, Team.T1)},
            //{2301, new(BetGameType.UNDER, GamePeriod.M, Team.T2)},
            //{2302, new(BetGameType.OVER, GamePeriod.M, Team.T2)},


            //{779, new(BetGameType.UNDER, GamePeriod.H1)},
            //{780, new(BetGameType.OVER, GamePeriod.H1)},
            //{891, new(BetGameType.UNDER, GamePeriod.H2)},
            //{892, new(BetGameType.OVER, GamePeriod.H2)},

            //{775, new(BetGameType.UNDER, GamePeriod.Q1)},
            //{776, new(BetGameType.OVER, GamePeriod.Q1)},
            //{787, new(BetGameType.UNDER, GamePeriod.Q2)},
            //{788, new(BetGameType.OVER, GamePeriod.Q2)},
            //{800, new(BetGameType.UNDER, GamePeriod.Q3)},
            //{801, new(BetGameType.OVER, GamePeriod.Q3)},
            //{825, new(BetGameType.UNDER, GamePeriod.Q4)},
            //{826, new(BetGameType.OVER, GamePeriod.Q4)},


            //{2373, new(BetGameType.UNDER, GamePeriod.Q1, Team.T2)},
            //{2374, new(BetGameType.OVER, GamePeriod.Q1, Team.T2)},



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
