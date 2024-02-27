using Arbitrage.General;
using Arbitrage.Utils;
using NLog;

namespace Arbitrage.DataGetters.MMOB
{
    /// <summary>
    /// Merkur, Maxbet, Betole and Oktagon all use same backend for bet games
    /// </summary>
    public abstract class MMOBParser : Parser
    {
        protected MMOBGetter _getter;

        protected static readonly Logger log = LogManager.GetCurrentClassLogger();
        public MMOBParser(BettingHouse house, MMOBGetter getter) : base(house) 
        {
            _getter = getter;
        }

        protected override void ParseFootball()
        {
            var jsonResp = _getter.GetSportResponse("S");

            foreach (var jsonMatch in jsonResp.esMatches)
            {
                ParseJsonMatch(jsonMatch, Sport.Football);
            }
        }

        protected override void ParseBasketball()
        {
            var jsonResp = _getter.GetSportResponse("B");

            foreach (var jsonMatch in jsonResp.esMatches)
            {
                ParseJsonMatch(jsonMatch, Sport.Basketball);
            }
        }

        private void ParseJsonMatch(JsonMatch match, Sport sport)
        {
            try
            {
                var jsonMatch = _getter.GetMatchResponse(match.id);

                DateTime startTime = DateTimeConverter.DateTimeFromLong(jsonMatch.kickOffTime, 1);

                HouseMatchData hmd = new(House, sport, startTime, jsonMatch.home.Trim(), jsonMatch.away.Trim());

                // Add odds
                ParseJsonBetGames(jsonMatch, ref hmd, sport);

                _parsedData.Add(hmd);
            } catch (Exception e)
            {
                log.Error("Exception while parsing match:");
                log.Error(e);
            }
        }

        protected virtual void ParseJsonBetGames(JsonMatch jsonMatch, ref Utils.HouseMatchData hmd, Sport sport)
        {
            foreach (var (oddID, oddValue) in jsonMatch.odds)
            {
                try
                {
                    if (betGameFromID.Keys.Contains(oddID))
                    {
                        BetGame game = betGameFromID[oddID].Clone();
                        game.Value = oddValue;

                        // Special parsing for basketball odds
                        if (sport is Sport.Basketball && game.type is BetGameType.OVER or BetGameType.UNDER)
                        {
                            var thr = GetThreshold(oddID, jsonMatch.betParams);
                            game.SetThreshold(thr);
                        }

                        hmd.AddBetGame(game);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Exception while parsing bet game:");
                    log.Error(ex);
                }
            }
        }

        public static double GetThreshold(int bgId, JsonBetParams jsonParams)
        {
            var thr = bgId switch
            {
                50444 or 50445 => jsonParams.overUnderOvertime,
                50446 or 50447 => jsonParams.overUnderOvertime2,
                50448 or 50449 => jsonParams.overUnderOvertime3,
                50450 or 50451 => jsonParams.overUnderOvertime4,
                50452 or 50453 => jsonParams.overUnderOvertime5,
                50454 or 50455 => jsonParams.overUnderOvertime6,
                50456 or 50457 => jsonParams.overUnderOvertime7,

                50466 or 50467 => jsonParams.homeOverUnderOvertime,
                51540 or 51541 => jsonParams.homeOverUnderOvertime2,
                50478 or 50479 => jsonParams.awayOverUnderOvertime,

                50980 or 50979 => jsonParams.overUnderP,
                50986 or 50988 => jsonParams.overUnderSecondHalf,

                50982 or 50983 => jsonParams.homeOverUnderFirstHalf,
                50984 or 50985 => jsonParams.awayOverUnderFirstHalf,

                599 or 598 => jsonParams.overUnderFirstPeriod,
                50027 or 50028 => jsonParams.homeOverUnderFirstPeriod,
                50029 or 50030 => jsonParams.awayOverUnderFirstPeriod,
                50165 or 50166 => jsonParams.overUnderSecondPeriod,
                50167 or 50168 => jsonParams.overUnderThirdPeriod,
                50169 or 50170 => jsonParams.overUnderFourthPeriod,

                _ => "0.0",
            };

            return double.Parse(thr.Trim());
        }


        /// <summary>
        /// Map betGames from json response id to BetGame
        /// </summary>
        public static readonly Dictionary<int, BetGame> betGameFromID = new()
        {
            {272, new(BetGameType.GG) },
            {273, new(BetGameType.NG) },
            {291, new(BetGameType.GG, GamePeriod.H1) },
            {292, new(BetGameType.NG, GamePeriod.H1) },
            {299, new(BetGameType.GG, GamePeriod.H2) },
            {300, new(BetGameType.NG, GamePeriod.H2) },

            {1, new(BetGameType.WX1) },
            {2, new(BetGameType.WXX) },
            {3, new(BetGameType.WX2) },
            {7, new(BetGameType.D1X) },
            {8, new(BetGameType.D12) },
            {9, new(BetGameType.DX2) },

            {4, new(BetGameType.WX1, GamePeriod.H1) },
            {5, new(BetGameType.WXX, GamePeriod.H1) },
            {6, new(BetGameType.WX2, GamePeriod.H1) },
            {397, new(BetGameType.D1X, GamePeriod.H1) },
            {398, new(BetGameType.D12, GamePeriod.H1) },
            {399, new(BetGameType.DX2, GamePeriod.H1) },

            {235, new(BetGameType.WX1, GamePeriod.H2) },
            {236, new(BetGameType.WXX, GamePeriod.H2) },
            {237, new(BetGameType.WX2, GamePeriod.H2) },
            {608, new(BetGameType.D1X, GamePeriod.H2) },
            {609, new(BetGameType.D12, GamePeriod.H2) },
            {610, new(BetGameType.DX2, GamePeriod.H2) },

            {21, new(BetGameType.UNDER, thr: 1.5) },
            {22, new(BetGameType.UNDER, thr: 2.5) },
            {219, new(BetGameType.UNDER, thr: 3.5) },
            {453, new(BetGameType.UNDER, thr: 4.5) },
            {242, new(BetGameType.OVER, thr: 1.5) },
            {24, new(BetGameType.OVER, thr: 2.5) },
            {25, new(BetGameType.OVER, thr: 3.5) },
            {27, new(BetGameType.OVER, thr: 4.5) },

            {267, new(BetGameType.UNDER, GamePeriod.H1, thr: 0.5) },
            {211, new(BetGameType.UNDER, GamePeriod.H1, thr: 1.5) },
            {472, new(BetGameType.UNDER, GamePeriod.H1, thr: 2.5) },
            {207, new(BetGameType.OVER, GamePeriod.H1, thr: 0.5) },
            {208, new(BetGameType.OVER, GamePeriod.H1, thr: 1.5) },
            {209, new(BetGameType.OVER, GamePeriod.H1, thr: 2.5) },
            {210, new(BetGameType.OVER, GamePeriod.H1, thr: 3.5) },

            {269, new(BetGameType.UNDER, GamePeriod.H2, thr: 0.5) },
            {217, new(BetGameType.UNDER, GamePeriod.H2, thr: 1.5) },
            {474, new(BetGameType.UNDER, GamePeriod.H2, thr: 2.5) },
            {213, new(BetGameType.OVER, GamePeriod.H2, thr: 0.5) },
            {214, new(BetGameType.OVER, GamePeriod.H2, thr: 1.5) },
            {215, new(BetGameType.OVER, GamePeriod.H2, thr: 2.5) },
            {216, new(BetGameType.OVER, GamePeriod.H2, thr: 3.5) }, // check

            {239, new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 0.5) },
            {247, new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 1.5) },
            {551, new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 2.5) }, // check
            {238, new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 0.5) },
            {248, new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 1.5) },
            {276, new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 2.5) },

            {337, new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 0.5) },
            {817, new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 1.5) }, // check
            //{"", new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 2.5) },
            {307, new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 0.5) },
            {274, new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 1.5) },
            {349, new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 2.5) },

            {339, new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 0.5) },
            {825, new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 1.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 2.5) },
            {312, new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 0.5) },
            {297, new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 1.5) },
            {351, new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 2.5) },

            {241, new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 0.5) },
            {249, new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 1.5) },
            {552, new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 2.5) },
            {554, new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 3.5) },
            {240, new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 0.5) },
            {250, new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 1.5) },
            {277, new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 2.5) },
            {556, new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 3.5) },

            {338, new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 0.5) },
            {821, new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 1.5) },
            {308, new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 0.5) },
            {275, new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 1.5) },
            {350, new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 2.5) },

            {340, new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 0.5) },
            {829, new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 1.5) },
            {313, new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 0.5) },
            {298, new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 1.5) },
            {352, new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 2.5) },

            // BASKETBALL ODDS
            {50291, new(BetGameType.W1)},
            {50293, new(BetGameType.W2)},

            {611, new(BetGameType.W1_X_0, GamePeriod.H1)},
            {612, new(BetGameType.W2_X_0, GamePeriod.H1)},
            {613, new(BetGameType.W1_X_0, GamePeriod.H2)},
            {614, new(BetGameType.W2_X_0, GamePeriod.H2)},
            {50025, new(BetGameType.W1_X_0, GamePeriod.Q1)},
            {50026, new(BetGameType.W2_X_0, GamePeriod.Q1)},
            {50173, new(BetGameType.W1_X_0, GamePeriod.Q2)},
            {50174, new(BetGameType.W2_X_0, GamePeriod.Q2)},
            {50175, new(BetGameType.W1_X_0, GamePeriod.Q3)},
            {50176, new(BetGameType.W2_X_0, GamePeriod.Q3)},
            {50177, new(BetGameType.W1_X_0, GamePeriod.Q4)},
            {50178, new(BetGameType.W2_X_0, GamePeriod.Q4)},

            {595, new(BetGameType.WX1, GamePeriod.Q1) },
            {596, new(BetGameType.WXX, GamePeriod.Q1) },
            {597, new(BetGameType.WX2, GamePeriod.Q1) },

            {50147, new(BetGameType.WX1, GamePeriod.Q2) },
            {50148, new(BetGameType.WXX, GamePeriod.Q2) },
            {50149, new(BetGameType.WX2, GamePeriod.Q2) },

            {50150, new(BetGameType.WX1, GamePeriod.Q3) },
            {50151, new(BetGameType.WXX, GamePeriod.Q3) },
            {50152, new(BetGameType.WX2, GamePeriod.Q3) },

            {50153, new(BetGameType.WX1, GamePeriod.Q4) },
            {50154, new(BetGameType.WXX, GamePeriod.Q4) },
            {50155, new(BetGameType.WX2, GamePeriod.Q4) },

            {50444, new(BetGameType.UNDER)},
            {50445, new(BetGameType.OVER)},
            {50446, new(BetGameType.UNDER)},
            {50447, new(BetGameType.OVER)},
            {50448, new(BetGameType.UNDER)},
            {50449, new(BetGameType.OVER)},
            {50450, new(BetGameType.UNDER)},
            {50451, new(BetGameType.OVER)},
            {50452, new(BetGameType.UNDER)},
            {50453, new(BetGameType.OVER)},
            {50454, new(BetGameType.UNDER)},
            {50455, new(BetGameType.OVER)},
            {50456, new(BetGameType.UNDER)},

            {50466, new(BetGameType.UNDER, tm: Team.T1)},
            {50467, new(BetGameType.OVER, tm: Team.T1)},
            {51540, new(BetGameType.UNDER, tm: Team.T1)},
            {51541, new(BetGameType.OVER, tm: Team.T1)},
            {50478, new(BetGameType.UNDER, tm: Team.T2)},
            {50479, new(BetGameType.OVER, tm: Team.T2)},

            {50980, new(BetGameType.UNDER, GamePeriod.H1)},
            {50979, new(BetGameType.OVER, GamePeriod.H1)},
            {50986, new(BetGameType.UNDER, GamePeriod.H2)},
            {50988, new(BetGameType.OVER, GamePeriod.H2)},

            {50982, new(BetGameType.UNDER, GamePeriod.H1, Team.T1)},
            {50983, new(BetGameType.OVER, GamePeriod.H1, Team.T1)},
            {50984, new(BetGameType.UNDER, GamePeriod.H1, Team.T2)},
            {50985, new(BetGameType.OVER, GamePeriod.H1, Team.T2)},

            {599, new(BetGameType.UNDER, GamePeriod.Q1)},
            {598, new(BetGameType.OVER, GamePeriod.Q1)},
            {50027, new(BetGameType.UNDER, GamePeriod.Q1, Team.T1)},
            {50028, new(BetGameType.OVER, GamePeriod.Q1, Team.T1)},
            {50029, new(BetGameType.UNDER, GamePeriod.Q1, Team.T2)},
            {50030, new(BetGameType.OVER, GamePeriod.Q1, Team.T2)},

            {50165, new(BetGameType.UNDER, GamePeriod.Q2)},
            {50166, new(BetGameType.OVER, GamePeriod.Q2)},
            {50167, new(BetGameType.UNDER, GamePeriod.Q3)},
            {50168, new(BetGameType.OVER, GamePeriod.Q3)},
            {50169, new(BetGameType.UNDER, GamePeriod.Q4)},
            {50170, new(BetGameType.OVER, GamePeriod.Q4)},
        };
    }
}
