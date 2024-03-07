using Arbitrage.DataGetters.MMOB;
using Arbitrage.General;

namespace Arbitrage.DataGetters.SoccerBet
{
    public class SoccerBetParser : MMOBParser
    {
        public SoccerBetParser() : base(BettingHouse.SoccerBet, new SoccerBetGetter()) { }

        protected override void ParseJsonBetGames(MMOB.JsonMatch jsonMatch, ref Utils.HouseMatchData hmd, Sport sport)
        {
            if (jsonMatch == null || jsonMatch.betMap == null || jsonMatch.betMap.Count == 0)
            {
                log.Warn("Invalid JsonMatch argument: " + jsonMatch);
                return;
            }

            foreach (var x in jsonMatch.betMap.Values)
            {
                try
                {
                    foreach (var oddJson in x.Values) {

                        if (oddJson == null || oddJson.s.Trim().Equals("L"))
                        {
                            continue;
                        }

                        var bpc = oddJson.bpc;

                        if (betGameFromBPC.Keys.Contains(bpc))
                        {
                            BetGame game = betGameFromBPC[bpc].Clone();
                            game.Value = oddJson.ov;

                            if (sport is Sport.Basketball && game.type is BetGameType.OVER or BetGameType.UNDER)
                            {
                                try
                                {
                                    double thr = double.Parse(oddJson.sv.Trim().Substring(6));
                                    game.SetThreshold(thr);

                                    // TODO
                                    //game.SetPeriod()
                                }
                                catch (Exception e)
                                {
                                    log.Error("Exception while parsing betgame: " + e);
                                }
                            }

                            hmd.AddBetGame(game);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Exception while parsing bet game:");
                    log.Error(ex);
                }
            }
        }

    /// <summary>
    /// Map betGames from json response bpc to BetGame
    /// </summary>
    static readonly Dictionary<int, BetGame> betGameFromBPC = new()
        {
            {92212, new(BetGameType.WX1)},
            {92213, new(BetGameType.WXX) },
            {92214, new(BetGameType.WX2) },
            {92216, new(BetGameType.D12) },
            {92215, new(BetGameType.D1X) },
            {92217, new(BetGameType.DX2) },
            {92218, new(BetGameType.WX1, GamePeriod.H1)},
            {92219, new(BetGameType.WXX, GamePeriod.H1) },
            {92220, new(BetGameType.WX2, GamePeriod.H1) },
            {92221, new(BetGameType.WX1, GamePeriod.H2)},
            {92222, new(BetGameType.WXX, GamePeriod.H2) },
            {92223, new(BetGameType.WX2, GamePeriod.H2) },
            //TODO poluvreme dupla sansa
            {92429, new(BetGameType.GG) },
            {92430, new(BetGameType.NG) },
            {92431, new(BetGameType.GG, GamePeriod.H1) },
            {92432, new(BetGameType.NG, GamePeriod.H1) },
            {92433, new(BetGameType.GG, GamePeriod.H2) },
            {92434, new(BetGameType.NG, GamePeriod.H2) },

            {92276, new(BetGameType.UNDER, thr: 1.5) },
            {92277, new(BetGameType.UNDER, thr: 2.5) },
            {92284, new(BetGameType.UNDER, thr: 3.5) },
            {92305, new(BetGameType.UNDER, thr: 4.5) },
            {92304, new(BetGameType.OVER, thr: 0.5) },
            {92289, new(BetGameType.OVER, thr: 1.5) },
            {92279, new(BetGameType.OVER, thr: 2.5) },
            {92280, new(BetGameType.OVER, thr: 3.5) },
            {92282, new(BetGameType.OVER, thr: 4.5) },
            {92288, new(BetGameType.OVER, thr: 5.5) },
            {92283, new(BetGameType.OVER, thr: 6.5) },

            {92312, new(BetGameType.UNDER, GamePeriod.H1, thr: 0.5) },
            {92310, new(BetGameType.UNDER, GamePeriod.H1, thr: 1.5) },
            {92314, new(BetGameType.UNDER, GamePeriod.H1, thr: 2.5) },
            {92306, new(BetGameType.OVER, GamePeriod.H1, thr: 0.5) },
            {92307, new(BetGameType.OVER, GamePeriod.H1, thr: 1.5) },
            {92308, new(BetGameType.OVER, GamePeriod.H1, thr: 2.5) },
            {92309, new(BetGameType.OVER, GamePeriod.H1, thr: 3.5) },

            {92325, new(BetGameType.UNDER, GamePeriod.H2, thr: 0.5) },
            {92323, new(BetGameType.UNDER, GamePeriod.H2, thr: 1.5) },
            {92327, new(BetGameType.UNDER, GamePeriod.H2, thr: 2.5) },
            {92319, new(BetGameType.OVER, GamePeriod.H2, thr: 0.5) },
            {92320, new(BetGameType.OVER, GamePeriod.H2, thr: 1.5) },
            {92321, new(BetGameType.OVER, GamePeriod.H2, thr: 2.5) },
            {92322, new(BetGameType.OVER, GamePeriod.H2, thr: 3.5) },

            {92356, new(BetGameType.UNDER, tm: Team.T1, thr: 0.5) },
            {92357, new(BetGameType.UNDER, tm: Team.T1, thr: 1.5) },
            {92363, new(BetGameType.UNDER, tm: Team.T1, thr: 2.5) },
            {92364, new(BetGameType.UNDER, tm: Team.T1, thr: 3.5) },
            {92355, new(BetGameType.OVER, tm: Team.T1, thr: 0.5) },
            {92358, new(BetGameType.OVER, tm: Team.T1, thr: 1.5) },
            {92359, new(BetGameType.OVER, tm: Team.T1, thr: 2.5) },
            {92365, new(BetGameType.OVER, tm: Team.T1, thr: 3.5) },

            {92370, new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 0.5) },
            {92371, new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 1.5) },
            {92369, new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 0.5) },
            {92372, new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 1.5) },
            {92368, new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 2.5) },

            {92378, new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 0.5) },
            {92379, new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 1.5) },
            {92375, new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 0.5) },
            {92374, new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 1.5) },
            {92377, new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 2.5) },

            {92393, new(BetGameType.UNDER, tm: Team.T2, thr: 0.5) },
            {92394, new(BetGameType.UNDER, tm: Team.T2, thr: 1.5) },
            {92400, new(BetGameType.UNDER, tm: Team.T2, thr: 2.5) },
            {92401, new(BetGameType.UNDER, tm: Team.T2, thr: 3.5) },
            {92392, new(BetGameType.OVER, tm: Team.T2, thr: 0.5) },
            {92395, new(BetGameType.OVER, tm: Team.T2, thr: 1.5) },
            {92396, new(BetGameType.OVER, tm: Team.T2, thr: 2.5) },
            {92402, new(BetGameType.OVER, tm: Team.T2, thr: 3.5) },

            {92407, new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 0.5) },
            {92408, new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 1.5) },
            {92406, new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 0.5) },
            {92409, new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 1.5) },
            {92405, new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 2.5) },

            {92414, new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 0.5) },
            {92415, new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 1.5) },
            {92413, new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 0.5) },
            {92416, new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 1.5) },
            {92412, new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 2.5) },

            ///
            /// BASKETBALL
            /// 
            {99995, new(BetGameType.W1)},
            {99996, new(BetGameType.W2)},

            {98827, new(BetGameType.WX1) },
            {98829, new(BetGameType.WXX) },
            {98828, new(BetGameType.WX2) },

            {98730, new(BetGameType.WX1, GamePeriod.H1) },
            {98732, new(BetGameType.WXX, GamePeriod.H1) },
            {98731, new(BetGameType.WX2, GamePeriod.H1) },

            {98902, new(BetGameType.WX1, GamePeriod.Q1) },
            {98904, new(BetGameType.WXX, GamePeriod.Q1) },
            {98903, new(BetGameType.WX2, GamePeriod.Q1) },

            {98908, new(BetGameType.WX1, GamePeriod.Q2) },
            {98910, new(BetGameType.WXX, GamePeriod.Q2) },
            {98909, new(BetGameType.WX2, GamePeriod.Q2) },

            {98911, new(BetGameType.WX1, GamePeriod.Q3) },
            {98913, new(BetGameType.WXX, GamePeriod.Q3) },
            {98912, new(BetGameType.WX2, GamePeriod.Q3) },

            {98917, new(BetGameType.WX1, GamePeriod.Q4) },
            {98919, new(BetGameType.WXX, GamePeriod.Q4) },
            {98918, new(BetGameType.WX2, GamePeriod.Q4) },

            {100525, new(BetGameType.OVER) },
            {100526, new(BetGameType.UNDER) },
            {100431, new(BetGameType.OVER, GamePeriod.H1) },
            {100432, new(BetGameType.UNDER, GamePeriod.H1) },

            {100438, new(BetGameType.UNDER, GamePeriod.Q1) },
            {100437, new(BetGameType.OVER, GamePeriod.Q1) },
            {100448, new(BetGameType.UNDER, GamePeriod.Q2) }, 
            {100447, new(BetGameType.OVER, GamePeriod.Q2) }, 
            {100450, new(BetGameType.UNDER, GamePeriod.Q3) }, 
            {100449, new(BetGameType.OVER, GamePeriod.Q3) }, 
            {100452, new(BetGameType.UNDER, GamePeriod.Q4) }, 
            {100451, new(BetGameType.OVER, GamePeriod.Q4) }, 

            {100517, new(BetGameType.OVER, tm: Team.T1) },
            {100518, new(BetGameType.UNDER, tm: Team.T1) },
            {100485, new(BetGameType.OVER, tm: Team.T2) },
            {100486, new(BetGameType.UNDER, tm: Team.T2) },
            
            {100498, new(BetGameType.UNDER, GamePeriod.H1, Team.T1) },
            {100497, new(BetGameType.OVER, GamePeriod.H1, Team.T1) },
            {100466, new(BetGameType.UNDER, GamePeriod.H1, Team.T2) },
            {100465, new(BetGameType.OVER, GamePeriod.H1, Team.T2) },
            
            {100504, new(BetGameType.UNDER, GamePeriod.Q1, Team.T1) },
            {100503, new(BetGameType.OVER, GamePeriod.Q1, Team.T1) },
            {100472, new(BetGameType.UNDER, GamePeriod.Q1, Team.T2) },
            {100471, new(BetGameType.OVER, GamePeriod.Q1, Team.T2) },
            
            {100508, new(BetGameType.UNDER, GamePeriod.Q2, Team.T1) },
            {100507, new(BetGameType.OVER, GamePeriod.Q2, Team.T1) },
            {100476, new(BetGameType.UNDER, GamePeriod.Q2, Team.T2) },
            {100475, new(BetGameType.OVER, GamePeriod.Q2, Team.T2) },

            {100510, new(BetGameType.UNDER, GamePeriod.Q3, Team.T1) },
            {100509, new(BetGameType.OVER, GamePeriod.Q3, Team.T1) },
            {100478, new(BetGameType.UNDER, GamePeriod.Q3, Team.T2) },
            {100477, new(BetGameType.OVER, GamePeriod.Q3, Team.T2) },
        };
    }
}
