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
        private readonly AdmiralBetGetter _getter = new();

        public AdmiralBetParser() : base(BettingHouses.AdmiralBet) { }

        private void ParseMatchEvent(JsonEvent matchEvent)
        {
            if (matchEvent.isTopOffer) { return; } // Skip special offfers

            DateTime dt = DateTime.Parse(matchEvent.dateTime).AddHours(2);

            var participants = matchEvent.name.Split('-').Select(x => x.Trim()).ToList();

            if (participants.Count < 2) { return; } // For now skip matches with < 2 teams, since football only has 2 teams

            Participant p1 = new(participants[0]);
            Participant p2 = new(participants[1]);

            Match match = new(dt, p1, p2);

            // Add odds
            foreach (var betGame in matchEvent.bets)
            {
                foreach (var outcome in betGame.betOutcomes)
                {
                    if (betGameFromInt.Keys.Contains(outcome.betTypeOutcomeId))
                    {
                        // Special parsing for over-under with sbv
                        // TODO refactor to take all odds
                        if (outcome.betTypeOutcomeId == 429) // under
                        {
                            if (outcome.sBV.Trim() != "1.5") { continue; }
                        }
                        if (outcome.betTypeOutcomeId == 430) // over
                        {
                            if (outcome.sBV.Trim() != "1.5") { continue; }
                        }

                        match.AddBetGame(betGameFromInt[outcome.betTypeOutcomeId], outcome.odd);
                    }
                }
            }

            _data.Insert(match);
        }

        private void ParseMatchResponse(JsonMatchResponse response)
        {
            foreach (var competition in response.competitions)
            {
                if (competition.events == null || competition.events.Count == 0) { continue; }

                foreach (var matchEvent in competition.events)
                {
                    if (matchEvent == null) { continue; }

                    ParseMatchEvent(matchEvent);
                }
            }
        }
        protected override void UpdateData()
        {
            var matchResponses = _getter.GetMatches();

            foreach (var matchResponse in matchResponses)
            {
                ParseMatchResponse(matchResponse);
            }
        }

        static readonly Dictionary<int, BettingGames> betGameFromInt = new()
        {
            {424, BettingGames._1 },
            {425, BettingGames._X },
            {426, BettingGames._2 },
            {429, BettingGames._0_TO_2 },
            {430, BettingGames._2_OR_MORE },
            //{0, BettingGames._3_OR_MORE },
            {501, BettingGames._12 },
            {500, BettingGames._1X },
            {502, BettingGames._X2 } // TODO kvote
        };
    }
}
