using Arbitrage.General;
using Arbitrage.Utils;
using NLog;

namespace Arbitrage.DataGetters.Meridian
{
    internal class MeridianParser : Parser
    {
        private readonly MeridianGetter _getter = new();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        public MeridianParser(): base(BettingHouse.Meridian) { }

        protected override void ParseFootball()
        {
            ParseSport(58);
        }

        protected override void ParseBasketball()
        {
            ParseSport(55);
        }

        private void ParseSport(int sportId)
        {
            var responses = _getter.GetMatches(sportId);

            foreach (var response in responses)
            {
                try
                {
                    foreach (var ev in response.events)
                    {
                        try
                        {
                            var jsonEvent = _getter.GetOddsResponse(ev.id);
                            ParseEvent(jsonEvent);
                        }
                        catch (Exception e)
                        {
                            log.Error("Exception while parsing event: " + e);
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("Exception while parsing json match response: " + e);
                }
            }
        }

        private void ParseEvent(JsonEvent ev)
        {
            var participants = ev.team;
            var p1 = participants[0].name.Trim();
            var p2 = participants[1].name.Trim();

            string format = "yyyy-MM-ddTHH:mm:ss.fffZ";
            DateTime startTime;

            if (DateTime.TryParseExact(ev.startTime, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out startTime))
            {
                Dictionary<int, Sport> sportFromId = new()
                {
                    { 58, Sport.Football },
                    { 55, Sport.Basketball },
                };

                var match = new HouseMatchData(BettingHouse.Meridian, sportFromId[ev.sportId], startTime, p1, p2);
                
                // Add odds
                foreach (var sel in ev.market)
                {
                    try
                    {
                        if (sel.state.Trim() != "ACTIVE") continue;

                        foreach (var item in sel.selection)
                        {
                            try
                            {
                                AddBetGame(match, sel, item);
                            }
                            catch (Exception e)
                            {
                                log.Error("Exception while parsing selection item: " + e);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                        log.Error("Exception while parsing selection: " + e);
                    }
                }

                _parsedData.Add(match);
            }
            else
            {
                // Parsiranje nije uspelo, možete obraditi grešku ovde
                Console.WriteLine("Nije moguće parsirati datum.");
            }
        }

        private void AddBetGame(HouseMatchData matchData, JsonSelection sel, JsonSelectionItem x)
        {
            Tuple<string, string> bgKey = new(sel.templateId, x.name);
            if (betGameFromString.ContainsKey(bgKey))
            {
                BetGame game = betGameFromString[bgKey].Clone();

                if (sel.overUnder != null)
                {
                    var thr = double.Parse(sel.overUnder);
                    game.SetThreshold(thr);
                }

                game.Value = double.Parse(x.price.Trim());
                matchData.AddBetGame(game);
            }
        }

        /// <summary>
        /// Maps (templateId, name) => BetGame
        /// </summary>
        static readonly Dictionary<Tuple<string, string>, BetGame> betGameFromString = new()
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

            ///// FOOTBALL
            {new("3999", "[[Rival1]]"), new(BetGameType.WX1) },
            {new("3999", "draw"), new(BetGameType.WXX) },
            {new("3999", "[[Rival2]]"), new(BetGameType.WX2) },
            {new("4008", "1X"), new(BetGameType.D1X) },
            {new("4008", "X2"), new(BetGameType.DX2) },
            {new("4008", "12"), new(BetGameType.D12) },

            {new("4117", "[[Rival1]]"), new(BetGameType.W1_X_0) },
            {new("4117", "[[Rival2]]"), new(BetGameType.W2_X_0) },
            {new("4040", "[[Rival1]]"), new(BetGameType.W1_X_0, GamePeriod.H1) },
            {new("4040", "[[Rival2]]"), new(BetGameType.W2_X_0, GamePeriod.H1) },
            {new("4059", "[[Rival1]]"), new(BetGameType.W1_X_0, GamePeriod.H2) },
            {new("4059", "[[Rival2]]"), new(BetGameType.W2_X_0, GamePeriod.H2) },

            {new("4017", "[[Rival1]]"), new(BetGameType.WX1, GamePeriod.H1) },
            {new("4017", "draw"), new(BetGameType.WXX, GamePeriod.H1) },
            {new("4017", "[[Rival2]]"), new(BetGameType.WX2, GamePeriod.H1) },
            {new("4022", "I 1X"), new(BetGameType.D1X, GamePeriod.H1) },
            {new("4022", "I X2"), new(BetGameType.DX2, GamePeriod.H1) },
            {new("4022", "I 12"), new(BetGameType.D12, GamePeriod.H1) },

            {new("4042", "[[Rival1]]"), new(BetGameType.WX1, GamePeriod.H2) },
            {new("4042", "draw"), new(BetGameType.WXX, GamePeriod.H2) },
            {new("4042", "[[Rival2]]"), new(BetGameType.WX2, GamePeriod.H2) },
            {new("4046", "II 1X"), new(BetGameType.D1X, GamePeriod.H2) },
            {new("4046", "II X2"), new(BetGameType.DX2, GamePeriod.H2) },
            {new("4046", "II 12"), new(BetGameType.D12, GamePeriod.H2) },

            {new("4007", "GG"), new(BetGameType.GG) },
            {new("4007", "NG"), new(BetGameType.NG) },
            {new("4021", "I GG"), new(BetGameType.GG, GamePeriod.H1) },
            {new("4021", "I NG"), new(BetGameType.NG, GamePeriod.H1) },
            {new("4045", "II GG"), new(BetGameType.GG, GamePeriod.H2) },
            {new("4045", "II NG"), new(BetGameType.NG, GamePeriod.H2) },

            {new("4004", "[[under]]"), new(BetGameType.UNDER) }, 
            {new("4004", "[[over]]"), new(BetGameType.OVER) },
            {new("4009", "[[under]]"), new(BetGameType.UNDER, tm: Team.T1) }, 
            {new("4009", "[[over]]"), new(BetGameType.OVER, tm: Team.T1) },
            {new("4011", "[[under]]"), new(BetGameType.UNDER, tm: Team.T2) }, 
            {new("4011", "[[over]]"), new(BetGameType.OVER, tm: Team.T2) },

            {new("4018", "[[under]]"), new(BetGameType.UNDER, GamePeriod.H1) }, 
            {new("4018", "[[over]]"), new(BetGameType.OVER, GamePeriod.H1) },
            {new("4023", "[[under]]"), new(BetGameType.UNDER, GamePeriod.H1, Team.T1) }, 
            {new("4023", "[[over]]"), new(BetGameType.OVER, GamePeriod.H1, Team.T1) },
            {new("4026", "[[under]]"), new(BetGameType.UNDER, GamePeriod.H1, Team.T2) }, 
            {new("4026", "[[over]]"), new(BetGameType.OVER, GamePeriod.H1, Team.T2) },

            {new("4043", "[[under]]"), new(BetGameType.UNDER, GamePeriod.H2) }, 
            {new("4043", "[[over]]"), new(BetGameType.OVER, GamePeriod.H2) },
            {new("4047", "[[under]]"), new(BetGameType.UNDER, GamePeriod.H2, Team.T1) }, 
            {new("4047", "[[over]]"), new(BetGameType.OVER, GamePeriod.H2, Team.T1) },
            {new("4049", "[[under]]"), new(BetGameType.UNDER, GamePeriod.H2, Team.T2) }, 
            {new("4049", "[[over]]"), new(BetGameType.OVER, GamePeriod.H2, Team.T2) },
        };
    }
}
