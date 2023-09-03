using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrage.General;
using Arbitrage.Utils;

namespace Arbitrage.DataGetters.Meridian
{
    internal class MeridianParser : Parser
    {
        private MeridianGetter _getter = new MeridianGetter();

        public MeridianParser() 
        {
            _data = new MatchesData(BettingHouses.Meridian);
        }

        protected override void UpdateData(DateTime dateTime)
        {
            var responses = _getter.GetMatches(dateTime);

            int id = 0;

            foreach (var response in responses )
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

                foreach (var sel in ev.standardShortMarkets) 
                {
                    if(sel == null) continue;

                    if(sel.selection != null)
                    {
                        sel.selection.ForEach(x => match.AddSubGame(x.name, double.Parse(x.price)));
                    }

                    if(sel.selections != null)
                    {
                        foreach(var sel2 in sel.selections)
                        {
                            if(sel2 != null)
                            {
                                sel2.selection.ForEach(x => match.AddSubGame(x.name, double.Parse(x.price)));
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
    }
}
