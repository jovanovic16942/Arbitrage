using Arbitrage.General;
using Arbitrage.Utils;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace Arbitrage.DataGetters.Mozzart
{
    public class MozzartParser : Parser
    {
        private MozzartGetter _getter = new MozzartGetter();

        public MozzartParser()
        {
            _data = new MatchesData(BettingHouses.Mozzart);
        }

        protected override void UpdateData(DateTime dateTime)
        {
            var respMatches = _getter.GetMatches(dateTime);

            //removed matches that expect n+ games in one ticket
            respMatches.Matches = respMatches.Matches.Where(x => x.SpecialType == 0).ToList();

            UpdateMatches(respMatches);

            var matchIDs = _data.GetMatches().Select(x => x.MatchId).ToList();
            var respOdds = _getter.GetOdds(matchIDs);
            UpdateOdds(respOdds);
        }

        private void UpdateMatches(JsonMatchResponse resp)
        {
            foreach (var match in resp.Matches)
            {
                try
                {
                    var participants = match.Participants.ToList();
                    var p1 = participants[0];
                    var p2 = participants[1];

                    var participant1 = new Utils.Participant(p1.Id, p1.Name, p1.ShortName, p1.Description);
                    var participant2 = new Utils.Participant(p2.Id, p2.Name, p2.ShortName, p2.Description);

                    _data.Insert(new Utils.Match(match.Id, match.StartTime, participant1, participant2));
                }
                catch
                {
                    // TODO Exception is thrown for Matches with 1 participant
                }
            }

        }

        private void UpdateOdds(List<JsonRoot> resp)
        {
            foreach (var matchOdds in resp)
            {
                var matchId = matchOdds.id;
                var kodds = matchOdds.kodds;

                foreach (var kvp in kodds)
                {
                    string subGameID = kvp.Key; // Mozzart.com subGameID - trenutno subgamesIds u MozzartGetter
                    JsonKodds kodd = kvp.Value;

                    if (kodd == null) continue;

                    double betValue = double.Parse(kodd.value); // Kvota

                    // Game/SubGame Mozzart.com data
                    JsonSubGame sg = kodd.subGame;

                    string subGameName = sg.subGameName.Trim();

                    BettingGames game;
                    if (betGameFromString.TryGetValue(subGameName, out game))
                    {
                        _data.UpdateMatchSubgame(matchId, game, betValue);
                    }

                }
            }
        }

        static Dictionary<string, BettingGames> betGameFromString = new Dictionary<string, BettingGames> { 
            {"1", BettingGames._1 }, 
            {"X", BettingGames._X }, 
            {"2", BettingGames._2 }, 
            {"0-2", BettingGames._0_TO_2 }, 
            {"2+", BettingGames._2_OR_MORE },
            {"3+", BettingGames._3_OR_MORE }, 
            {"12", BettingGames._12 }, 
            {"1X", BettingGames._1X }, 
            {"X2", BettingGames._X2 }
        };
    }
}
