using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Arbitrage.General;
using Arbitrage.Utils;
using Microsoft.Extensions.Logging;

namespace Arbitrage.DataGetters.Meridian
{
    //TODO dodati req za hvatanje ukupno golova (manje vise) tj 0-2 2+...

    internal class MeridianParser : Parser
    {
        private readonly MeridianGetter _getter = new();

        public MeridianParser(): base(BettingHouses.Meridian) { }

        protected override void UpdateData()
        {
            var responses = _getter.GetMatches();

            foreach (var response in responses)
            {
                foreach (var ev in response.events)
                {
                    ParseEvent(ev);
                }
            }
        }

        private void ParseEvent(JsonEvent ev)
        {
            var participants = ev.team;
            var p1 = participants[0];
            var p2 = participants[1];

            var participant1 = new Participant(int.Parse(p1.id), p1.name, "", "");
            var participant2 = new Participant(int.Parse(p2.id), p2.name, "", "");

            string format = "yyyy-MM-ddTHH:mm:ss.fffZ";
            DateTime startTime;

            if (DateTime.TryParseExact(ev.startTime, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out startTime))
            {
                var match = new Utils.Match(startTime, participant1, participant2);

                // Add odds
                foreach (var sel in ev.standardShortMarkets) 
                {
                    if(sel == null) continue;

                    if(sel.selection != null)
                    {
                        sel.selection.ForEach(x => TryAddBetGame(match, x));
                    }

                    if(sel.selections != null)
                    {
                        foreach(var sel2 in sel.selections)
                        {
                            if(sel2 != null)
                            {
                                sel2.selection.ForEach(x => TryAddBetGame(match, x));
                            }
                        }
                    }
                }

                _data.Insert(match);
            }
            else
            {
                // Parsiranje nije uspelo, možete obraditi grešku ovde
                // TODO Logging
                Console.WriteLine("Nije moguće parsirati datum.");
            }
        }

        private void TryAddBetGame(Arbitrage.Utils.Match match, JsonSelectionItem x)
        {
            BettingGames game;
            if (betGameFromString.TryGetValue(x.name.Trim(), out game))
            {
                match.AddBetGame(game, double.Parse(x.price));
            }
        }

        static Dictionary<string, BettingGames> betGameFromString = new Dictionary<string, BettingGames> {
            {"1", BettingGames._1 },
            {"[[Rival1]]", BettingGames._1 },
            {"draw", BettingGames._X },
            {"x", BettingGames._X },
            {"2", BettingGames._2 },
            {"[[Rival2]]", BettingGames._2 },
            {"0-2", BettingGames._0_TO_2 },  // TODO pokupiti ispravan string za meridijan kvote //under
            {"2+", BettingGames._2_OR_MORE },
            {"3+", BettingGames._3_OR_MORE },
            {"12", BettingGames._12 },
            {"1X", BettingGames._1X },
            {"X2", BettingGames._X2 } // TODO kvote
        };
    }
}
