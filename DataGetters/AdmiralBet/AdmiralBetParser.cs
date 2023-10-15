using Arbitrage.DataGetters.Admiralbet;
using Arbitrage.DataGetters.MaxBet;
using Arbitrage.General;
using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Arbitrage.DataGetters.AdmiralBet
{
    public class AdmiralBetParser : Parser
    {
        private AdmiralBetGetter _getter = new AdmiralBetGetter();

        public AdmiralBetParser()
        {
            _data = new MatchesData(BettingHouses.AdmiralBet);
        }

        protected override void UpdateData(DateTime dateTime)
        {
            var matchResponses = _getter.GetMatches();

            foreach (var matchResponse in matchResponses)
            {
                foreach (var competition in matchResponse.competitions)
                {
                    if (competition.events == null || competition.events.Count == 0) { continue; }

                    foreach (var matchEvent in competition.events)
                    {

                        if (matchEvent.isTopOffer) { continue; }

                        DateTime dt = DateTime.Parse(matchEvent.dateTime).AddHours(2);

                        var participants = matchEvent.name.Split('-').Select(x => x.Trim()).ToList();

                        if(participants.Count < 2) { continue; }

                        Participant p1 = new Participant(participants[0]);
                        Participant p2 = new Participant(participants[1]);

                        Match match = new Match(dt, p1, p2);

                        // Add odds
                        foreach (var betGame in matchEvent.bets)
                        {
                            foreach (var outcome in betGame.betOutcomes)
                            {
                                if (betGameFromInt.Keys.Contains(outcome.betTypeOutcomeId))
                                {
                                    match.AddBetGame(betGameFromInt[outcome.betTypeOutcomeId], outcome.odd);
                                }
                            }
                        }

                        _data.Insert(match);

                    }
                }
            }
        }

        static Dictionary<int, BettingGames> betGameFromInt = new Dictionary<int, BettingGames> {
            {424, BettingGames._1 },
            {425, BettingGames._X },
            {426, BettingGames._2 },
            //{0, BettingGames._0_TO_2 },
            //{0, BettingGames._2_OR_MORE },
            //{0, BettingGames._3_OR_MORE },
            {501, BettingGames._12 },
            {500, BettingGames._1X },
            {502, BettingGames._X2 } // TODO kvote
        };
    }
}
