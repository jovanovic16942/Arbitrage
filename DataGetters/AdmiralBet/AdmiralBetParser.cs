using Arbitrage.DataGetters.Admiralbet;
using Arbitrage.General;
using Arbitrage.Utils;

namespace Arbitrage.DataGetters.AdmiralBet
{
    public class AdmiralBetParser : Parser
    {
        private readonly AdmiralBetGetter _getter = new();

        public AdmiralBetParser() : base(BettingHouse.AdmiralBet) { }

        private void ParseMatchEvent(JsonEvent matchEvent)
        {
            if (matchEvent.isTopOffer) { return; } // Skip special offfers

            DateTime startTime = DateTime.Parse(matchEvent.dateTime).AddHours(1); // TODO SUSS

            var teams = matchEvent.name.Split('-').Select(x => x.Trim()).ToList();

            if (teams.Count < 2) { return; } // For now skip matches with < 2 teams

            HouseMatchData matchData = new(House, Sport.Football, startTime, teams[0], teams[1]);

            // Add odds
            foreach (var betGame in matchEvent.bets)
            {
                foreach (var outcome in betGame.betOutcomes)
                {
                    int betGameId = outcome.betTypeOutcomeId;

                    if (!betGameFromInt.Keys.Contains(betGameId)) continue;

                    double? thr = null;

                    // Special parsing for over-under with sbv
                    if (betGameId == 429 || betGameId == 430)
                    {
                        thr = double.Parse(outcome.sBV.Trim());
                    }

                    BetGame bg = new(betGameFromInt[betGameId], thr)
                    {
                        Value = outcome.odd
                    };

                    matchData.AddBetGame(bg);
                }
            }

            _parsedData.Add(matchData);
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
        protected override void ParseFootball()
        {
            var matchResponses = _getter.GetMatches();

            foreach (var matchResponse in matchResponses)
            {
                ParseMatchResponse(matchResponse);
            }
        }

        /// <summary>
        /// Map betGames from json response bet type outcome id_str to BetGameConfig
        /// over-under (total goals) odds have special rules
        /// </summary>
        static readonly Dictionary<int, BetGameConfig> betGameFromInt = new()
        {
            {424, new(BetGameType.WX1) },
            {425, new(BetGameType.WXX) },
            {426, new(BetGameType.WX2) },
            {501, new(BetGameType.D12) },
            {500, new(BetGameType.D1X) },
            {502, new(BetGameType.DX2) },
            {498, new(BetGameType.GG) },
            {499, new(BetGameType.NG) },

            {429, new(BetGameType.UNDER)},
            {430, new(BetGameType.OVER)},
        };

        /// <summary>
        /// Map betGames from json response bet type outcome id_str to BettingGames enum
        /// over-under (total goals) odds have special rules
        /// </summary>
        static readonly Dictionary<int, BettingGames> betGameFromIntOld = new()
        {
            {424, BettingGames._1 },
            {425, BettingGames._X },
            {426, BettingGames._2 },
            {498, BettingGames._GG },
            {499, BettingGames._NG },
            {501, BettingGames._12 },
            {500, BettingGames._1X },
            {502, BettingGames._X2 },
            // 429 is outcome id_str for subgame UNDER
            {1429, BettingGames._UG_0_1 },
            {2429, BettingGames._UG_0_2 },
            {3429, BettingGames._UG_0_3 },
            {4429, BettingGames._UG_0_4 },
            {5429, BettingGames._UG_0_5 },
            {6429, BettingGames._UG_0_6 },
            // 430 is outcome id_str for subgame OVER
            {1430, BettingGames._UG_2_PLUS },
            {2430, BettingGames._UG_3_PLUS },
            {3430, BettingGames._UG_4_PLUS },
            {4430, BettingGames._UG_5_PLUS },
            {5430, BettingGames._UG_6_PLUS },
        };
    }
}
