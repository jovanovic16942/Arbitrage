using Arbitrage.General;
using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.BetOle
{
    internal class BetOleParser : Parser
    {

        private readonly BetOleGetter _getter = new();

        public BetOleParser() : base(BettingHouses.BetOle) { }

        protected override void UpdateData()
        {
            var jsonResp = _getter.GetFootballResponse();

            if (jsonResp == null || jsonResp.esMatches == null) 
            {
                // TODO logger
                return;
            }

            foreach (var jsonMatch in jsonResp.esMatches)
            {
                if (jsonMatch.id == 0) continue;

                try
                {
                    var match = _getter.GetMatchResponse(jsonMatch.id);
                    ParseJsonMatch(match);
                } catch (Exception e)
                {
                    // TODO handle
                    continue;
                }
            }
        }

        private void ParseJsonMatch(JsonMatch jsonMatch)
        {
            DateTime startTime = DateTimeConverter.DateTimeFromLong(jsonMatch.kickOffTime, 1);

            Participant p1 = new(jsonMatch.home);
            Participant p2 = new(jsonMatch.away);

            Match match = new(startTime, p1, p2);

            if (jsonMatch.odds == null) return;

            foreach (var (jsonOddId, oddValue) in jsonMatch.odds)
            {
                if (betGameFromInt.ContainsKey(jsonOddId))
                {
                    match.AddBetGame(betGameFromInt[jsonOddId], oddValue);
                }
            }

            _data.Insert(match);

        }

        // SAME AS MAXBET !!!
        /// <summary>
        /// Map bet game id from json response to BettingGames enum
        /// </summary>
        static readonly Dictionary<int, BettingGames> betGameFromInt = new()
        {
            {1, BettingGames._1 },
            {2, BettingGames._X },
            {3, BettingGames._2 },
            {8, BettingGames._12 },
            {7, BettingGames._1X },
            {9, BettingGames._X2 },
            {4, BettingGames._1_I },
            {5, BettingGames._X_I },
            {6, BettingGames._2_I },
            {398, BettingGames._12_I },
            {397, BettingGames._1X_I },
            {399, BettingGames._X2_I },
            {235, BettingGames._1_II },
            {236, BettingGames._X_II },
            {237, BettingGames._2_II },
            {609, BettingGames._12_II },
            {608, BettingGames._1X_II },
            {610, BettingGames._X2_II },
            {272, BettingGames._GG },
            {273, BettingGames._NG },
            {291, BettingGames._GG_I },
            {292, BettingGames._NG_I },
            {299, BettingGames._GG_II },
            {300, BettingGames._NG_II },
            {21, BettingGames._UG_0_1 },
            {22, BettingGames._UG_0_2 },
            {242, BettingGames._UG_2_PLUS },
            {23, BettingGames._UG_2_3 },
            {24, BettingGames._UG_3_PLUS },
            {25, BettingGames._UG_4_PLUS },
            {27, BettingGames._UG_5_PLUS },
        };
    }
}
