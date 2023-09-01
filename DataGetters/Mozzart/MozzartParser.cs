using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Mozzart
{
    public class MozzartParser : Parser
    {
        private MozzartGetter _getter = new MozzartGetter();

        protected override void UpdateData(DateTime dateTime)
        {
            var respMatches = _getter.GetMatches(dateTime);
            UpdateMatches(respMatches);

            var matchIDs = _data.GetMatches().Select(x => x.MatchId).ToList();
            var respOdds = _getter.GetOdds(matchIDs);
            UpdateOdds(respOdds);
        }

        private void UpdateMatches(General.MatchResponse resp)
        {
            foreach (var match in resp.Matches)
            {
                try
                {
                    var participants = match.Participants.ToList();
                    var p1 = participants[0];
                    var p2 = participants[1];

                    var participant1 = new Participant(p1.Id, p1.Name, p1.ShortName, p1.Description);
                    var participant2 = new Participant(p2.Id, p2.Name, p2.ShortName, p2.Description);

                    _data.Insert(new Match(match.Id, match.StartTime, participant1, participant2));
                }
                catch
                {
                    // TODO Exception is thrown for Matches with 1 participant
                }
            }

        }

        private void UpdateOdds(List<General.Root> resp)
        {
            foreach (var matchOdds in resp)
            {
                var matchId = matchOdds.id;
                var kodds = matchOdds.kodds;

                foreach (var kvp in kodds)
                {
                    string subGameID = kvp.Key; // Mozzart.com subGameID - trenutno subgamesIds u MozzartGetter
                    General.Kodds kodd = kvp.Value;

                    if (kodd == null) continue;

                    double betValue = double.Parse(kodd.value); // Kvota

                    // Game/SubGame Mozzart.com data
                    General.SubGame sg = kodd.subGame;

                    string subGameName = sg.subGameName;

                    _data.UpdateMatchSubgame(matchId, subGameName, betValue);
                }
            }
        }
    }
}
