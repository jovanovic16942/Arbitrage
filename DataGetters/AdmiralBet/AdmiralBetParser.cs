using Arbitrage.DataGetters.Admiralbet;
using Arbitrage.General;
using Arbitrage.Utils;
using NLog;

namespace Arbitrage.DataGetters.AdmiralBet
{
    public class AdmiralBetParser : Parser
    {
        private readonly AdmiralBetGetter _getter = new();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public AdmiralBetParser() : base(BettingHouse.AdmiralBet) { }

        private void ParseMatchEvent(JsonEvent matchEvent, List<JsonBet> bets)
        {
            try
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
                    try
                    {
                        foreach (var outcome in betGame.betOutcomes)
                        {
                            try
                            {
                                int betGameId = outcome.betTypeOutcomeId;

                                if (!betGameFromInt.Keys.Contains(betGameId)) continue;

                                BetGame game = betGameFromInt[betGameId].Clone();

                                if (double.TryParse(outcome.sBV, out double thr))
                                {
                                    game.SetThreshold(thr);
                                }

                                game.Value = outcome.odd;

                                var oldVal = matchData.GetOddValue(game);

                                if (oldVal == 0)
                                {
                                    matchData.AddBetGame(game);
                                } else if (oldVal != game.Value)
                                {
                                    log.Warn(string.Format("Different values detected for: {0} Values: {1} (old) - {2} (new)",
                                            game, oldVal, game.Value));
                                    matchData.UpdateBetGame(game);
                                }

                            }
                            catch (Exception e)
                            {
                                log.Error("Exception while parsing bet outcome: ");
                                log.Error(e);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Exception while parsing bet game: ");
                        log.Error(e);
                    }
                }

                _parsedData.Add(matchData);
            } catch (Exception ex)
            {
                log.Error("Exception while parsing match event: ");
                log.Error(ex);
            }
        }

        private void ParseMatchResponse(JsonMatchResponse response)
        {
            foreach (var competition in response.competitions)
            {
                if (competition.events == null || competition.events.Count == 0) { continue; }

                try
                {
                    var eventIDs = competition.events.Select(x => x.id).ToList();
                    var oddsResponse = _getter.GetBets(competition.competitionId, eventIDs, competition.regionId, competition.events[0].sportId);

                    foreach (var matchEvent in competition.events)
                    {
                        if (matchEvent == null) { continue; }
                        try
                        {
                            var oddsEvent = oddsResponse.First(x => x.eventId == matchEvent.id);
                            ParseMatchEvent(matchEvent, oddsEvent.bets);
                        }
                        catch (Exception ex)
                        {
                            // Get some odds but not all
                            log.Error("Source: " + ex.Source + " Message: " + ex.Message);
                            ParseMatchEvent(matchEvent, matchEvent.bets);
                        }
                    }
                } catch (Exception e)
                {
                    log.Error("Exception while parsing response: " + e);
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
            {2291, new(BetGameType.W1_X_0, GamePeriod.H1) },
            {2292, new(BetGameType.W2_X_0, GamePeriod.H1) },
            {2293, new(BetGameType.W1_X_0, GamePeriod.H2) },
            {2294, new(BetGameType.W2_X_0, GamePeriod.H2) },

            {2355, new(BetGameType.W1_X_0, GamePeriod.Q1) },
            {2356, new(BetGameType.W2_X_0, GamePeriod.Q1) },
            {2357, new(BetGameType.W1_X_0, GamePeriod.Q2) },
            {2358, new(BetGameType.W2_X_0, GamePeriod.Q2) },
            {2359, new(BetGameType.W1_X_0, GamePeriod.Q3) },
            {2360, new(BetGameType.W2_X_0, GamePeriod.Q3) },
            {2361, new(BetGameType.W1_X_0, GamePeriod.Q4) },
            {2362, new(BetGameType.W2_X_0, GamePeriod.Q4) },

            {2371, new(BetGameType.W1_X_0, GamePeriod.Q1, Team.T1) },
            {2372, new(BetGameType.W2_X_0, GamePeriod.Q1, Team.T1) },

            {746, new(BetGameType.W1) },
            {747, new(BetGameType.W2) },

            {754, new(BetGameType.WX1) },
            {755, new(BetGameType.WXX) },
            {756, new(BetGameType.WX2) },

            {772, new(BetGameType.WX1, GamePeriod.Q1) },
            {773, new(BetGameType.WXX, GamePeriod.Q1) },
            {774, new(BetGameType.WX2, GamePeriod.Q1) },
            {784, new(BetGameType.WX1, GamePeriod.Q2) },
            {785, new(BetGameType.WXX, GamePeriod.Q2) },
            {786, new(BetGameType.WX2, GamePeriod.Q2) },
            {792, new(BetGameType.WX1, GamePeriod.Q3) },
            {793, new(BetGameType.WXX, GamePeriod.Q3) },
            {794, new(BetGameType.WX2, GamePeriod.Q3) },
            {795, new(BetGameType.WX1, GamePeriod.Q4) },
            {796, new(BetGameType.WXX, GamePeriod.Q4) },
            {797, new(BetGameType.WX2, GamePeriod.Q4) },

            {821, new(BetGameType.UNDER)}, // + OT
            {822, new(BetGameType.OVER)},
            {2299, new(BetGameType.UNDER, GamePeriod.M, Team.T1)},
            {2300, new(BetGameType.OVER, GamePeriod.M, Team.T1)},
            {2301, new(BetGameType.UNDER, GamePeriod.M, Team.T2)},
            {2302, new(BetGameType.OVER, GamePeriod.M, Team.T2)},

            {779, new(BetGameType.UNDER, GamePeriod.H1)},
            {780, new(BetGameType.OVER, GamePeriod.H1)},
            {891, new(BetGameType.UNDER, GamePeriod.H2)},
            {892, new(BetGameType.OVER, GamePeriod.H2)},

            {775, new(BetGameType.UNDER, GamePeriod.Q1)},
            {776, new(BetGameType.OVER, GamePeriod.Q1)},
            {787, new(BetGameType.UNDER, GamePeriod.Q2)},
            {788, new(BetGameType.OVER, GamePeriod.Q2)},
            {800, new(BetGameType.UNDER, GamePeriod.Q3)},
            {801, new(BetGameType.OVER, GamePeriod.Q3)},
            {825, new(BetGameType.UNDER, GamePeriod.Q4)},
            {826, new(BetGameType.OVER, GamePeriod.Q4)},

            {2373, new(BetGameType.UNDER, GamePeriod.Q1, Team.T2)},
            {2374, new(BetGameType.OVER, GamePeriod.Q1, Team.T2)},

            /// FOOTBALL
            {505, new(BetGameType.W1_X_0) },
            {506, new(BetGameType.W2_X_0) },

            // 1 X 2 - Dupla sansa
            {424, new(BetGameType.WX1) },
            {425, new(BetGameType.WXX) },
            {426, new(BetGameType.WX2) },
            {500, new(BetGameType.D1X) },
            {501, new(BetGameType.D12) },
            {502, new(BetGameType.DX2) },

            // 1 X 2 - Dupla sansa - H1
            {489, new(BetGameType.WX1, GamePeriod.H1) },
            {490, new(BetGameType.WXX, GamePeriod.H1) },
            {491, new(BetGameType.WX2, GamePeriod.H1) },
            {486, new(BetGameType.D1X, GamePeriod.H1) },
            {487, new(BetGameType.D12, GamePeriod.H1) },
            {488, new(BetGameType.DX2, GamePeriod.H1) },

            // 1 X 2 - Dupla sansa - H2
            {492, new(BetGameType.WX1, GamePeriod.H2) },
            {493, new(BetGameType.WXX, GamePeriod.H2) },
            {494, new(BetGameType.WX2, GamePeriod.H2) },
            {495, new(BetGameType.D1X, GamePeriod.H2) },
            {496, new(BetGameType.D12, GamePeriod.H2) },
            {497, new(BetGameType.DX2, GamePeriod.H2) },

            {498, new(BetGameType.GG) },
            {499, new(BetGameType.NG) },
            //{, new(BetGameType.GG, GamePeriod.H1) },
            //{, new(BetGameType.NG, GamePeriod.H1) },
            //{, new(BetGameType.GG, GamePeriod.H2) },
            //{, new(BetGameType.NG, GamePeriod.H2) },

            // over - under 
            {429, new(BetGameType.UNDER)},
            {430, new(BetGameType.OVER)},
            {472, new(BetGameType.UNDER, GamePeriod.H1)},
            {473, new(BetGameType.OVER, GamePeriod.H1)},
            {474, new(BetGameType.UNDER, GamePeriod.H2)},
            {475, new(BetGameType.OVER, GamePeriod.H2)},

            // over - under per team
            {520, new(BetGameType.UNDER, GamePeriod.M, Team.T1)},
            {521, new(BetGameType.OVER, GamePeriod.M, Team.T1)},
            {522, new(BetGameType.UNDER, GamePeriod.M, Team.T2)},
            {523, new(BetGameType.OVER, GamePeriod.M, Team.T2)},

            {468, new(BetGameType.UNDER, GamePeriod.H1, Team.T1)},
            {469, new(BetGameType.OVER, GamePeriod.H1, Team.T1)},
            {470, new(BetGameType.UNDER, GamePeriod.H1, Team.T2)},
            {471, new(BetGameType.OVER, GamePeriod.H1, Team.T2)},

            {464, new(BetGameType.UNDER, GamePeriod.H2, Team.T1)},
            {465, new(BetGameType.OVER, GamePeriod.H2, Team.T1)},
            {466, new(BetGameType.UNDER, GamePeriod.H2, Team.T2)},
            {467, new(BetGameType.OVER, GamePeriod.H2, Team.T2)},
        };
    }
}
