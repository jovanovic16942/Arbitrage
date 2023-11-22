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

            DateTime dt = DateTime.Parse(matchEvent.dateTime).AddHours(1); // TODO SUSS

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
                    int betGameId = outcome.betTypeOutcomeId;

                    // Special parsing for over-under with sbv
                    if (betGameId == 429 || betGameId == 430)
                    {
                        // ex. 429 with sbv 2.5 -> 2429
                        betGameId += 1000 * int.Parse(outcome.sBV.Trim()[..1]); 
                    }

                    if (betGameFromInt.Keys.Contains(betGameId))
                    {
                        var bg = betGameFromInt[betGameId];
                        match.AddBetGame(bg, outcome.odd);
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

        /// <summary>
        /// Map betGames from json response bet type outcome id to BettingGames enum
        /// over-under (total goals) odds have special rules
        /// </summary>
        static readonly Dictionary<int, BettingGames> betGameFromInt = new()
        {
            {424, BettingGames._1 },
            {425, BettingGames._X },
            {426, BettingGames._2 },
            {498, BettingGames._GG },
            {499, BettingGames._NG },
            {501, BettingGames._12 },
            {500, BettingGames._1X },
            {502, BettingGames._X2 },
            // 429 is outcome id for subgame UNDER
            {1429, BettingGames._UG_0_1 },
            {2429, BettingGames._UG_0_2 },
            {3429, BettingGames._UG_0_3 },
            {4429, BettingGames._UG_0_4 },
            {5429, BettingGames._UG_0_5 },
            {6429, BettingGames._UG_0_6 },
            // 430 is outcome id for subgame OVER
            {1430, BettingGames._UG_2_PLUS },
            {2430, BettingGames._UG_3_PLUS },
            {3430, BettingGames._UG_4_PLUS },
            {4430, BettingGames._UG_5_PLUS },
            {5430, BettingGames._UG_6_PLUS },
        };
    }
}
