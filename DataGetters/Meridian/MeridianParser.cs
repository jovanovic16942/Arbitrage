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

                // Determine which quarter from template name
                if (game.period == GamePeriod.Q1)
                {
                    try
                    {
                        game.SetPeriod(chToQuarter[sel.templateName.Trim()[..5]]);
                    } 
                    catch (Exception e)
                    {
                        log.Error("Neam pojma brate: " + e);
                    }
                }

                if (sel.overUnder != null)
                {
                    var thr = double.Parse(sel.overUnder);
                    game.SetThreshold(thr);
                }

                game.Value = double.Parse(x.price.Trim());
                matchData.AddBetGame(game);
            }
        }

        static readonly Dictionary<string, GamePeriod> chToQuarter = new()
        {
            {"[[1]]", GamePeriod.Q1 },
            {"[[2]]", GamePeriod.Q2 },
            {"[[3]]", GamePeriod.Q3 },
            {"[[4]]", GamePeriod.Q4 },
        };

        /// <summary>
        /// Maps (templateId, name) => BetGame
        /// </summary>
        static readonly Dictionary<Tuple<string, string>, BetGame> betGameFromString = new()
        {
            /// BASKETBALL
            {new("3741", "[[Rival1]]"), new(BetGameType.W1) },
            {new("3741", "[[Rival2]]"), new(BetGameType.W2) },
            {new("3747", "[[Rival1]]"), new(BetGameType.WX1) },
            {new("3747", "draw"), new(BetGameType.WXX) },
            {new("3747", "[[Rival2]]"), new(BetGameType.WX2) },

            {new("3763", "[[Rival1]]"), new(BetGameType.W1_X_0, GamePeriod.H1) },
            {new("3763", "[[Rival2]]"), new(BetGameType.W2_X_0, GamePeriod.H1) },
            {new("3760", "[[Rival1]]"), new(BetGameType.WX1, GamePeriod.H1) },
            {new("3760", "draw"), new(BetGameType.WXX, GamePeriod.H1) },
            {new("3760", "[[Rival2]]"), new(BetGameType.WX2, GamePeriod.H1) },

            {new("5259", "[[Rival1]]"), new(BetGameType.W1_X_0, GamePeriod.H2) },
            {new("5259", "[[Rival2]]"), new(BetGameType.W2_X_0, GamePeriod.H2) },
            {new("5258", "[[Rival1]]"), new(BetGameType.WX1, GamePeriod.H2) },
            {new("5258", "draw"), new(BetGameType.WXX, GamePeriod.H2) },
            {new("5258", "[[Rival2]]"), new(BetGameType.WX2, GamePeriod.H2) },

            {new("3742", "[[under]]"), new(BetGameType.UNDER) },
            {new("3742", "[[over]]"), new(BetGameType.OVER) },

            {new("3761", "[[under]]"), new(BetGameType.UNDER, GamePeriod.H1) },
            {new("3761", "[[over]]"), new(BetGameType.OVER, GamePeriod.H1) },
            {new("5257", "[[under]]"), new(BetGameType.UNDER, GamePeriod.H2) },
            {new("5257", "[[over]]"), new(BetGameType.OVER, GamePeriod.H2) },
            {new("4362", "[[under]]"), new(BetGameType.UNDER, GamePeriod.H1, Team.T1) },
            {new("4362", "[[over]]"), new(BetGameType.OVER, GamePeriod.H1, Team.T1) },
            {new("4363", "[[under]]"), new(BetGameType.UNDER, GamePeriod.H1, Team.T2) },
            {new("4363", "[[over]]"), new(BetGameType.OVER, GamePeriod.H1, Team.T2) },

            // TODO H2 T1-T2

            {new("3751", "[[under]]"), new(BetGameType.UNDER, tm: Team.T1) },
            {new("3751", "[[over]]"), new(BetGameType.OVER, tm: Team.T1) },
            {new("3753", "[[under]]"), new(BetGameType.UNDER, tm: Team.T2) },
            {new("3753", "[[over]]"), new(BetGameType.OVER, tm: Team.T2) },

            // Same for all quarters, handled in the parser
            {new("3758", "[[Rival1]]"), new(BetGameType.W1_X_0, GamePeriod.Q1) },
            {new("3758", "[[Rival2]]"), new(BetGameType.W2_X_0, GamePeriod.Q1) },
            {new("3754", "[[Rival1]]"), new(BetGameType.WX1, GamePeriod.Q1) },
            {new("3754", "draw"), new(BetGameType.WXX, GamePeriod.Q1) },
            {new("3754", "[[Rival2]]"), new(BetGameType.WX2, GamePeriod.Q1) },
            {new("3755", "[[under]]"), new(BetGameType.UNDER, GamePeriod.Q1) },
            {new("3755", "[[over]]"), new(BetGameType.OVER, GamePeriod.Q1) },
            
            // TODO Quarters per Team

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
