using Arbitrage.DataGetters.AdmiralBet;
using Arbitrage.DataGetters.SoccerBet;
using Arbitrage.General;
using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Pinnbet
{
    public class PinnBetParser : Parser
    {
        private readonly PinnBetGetter _getter = new();

        public PinnBetParser(): base(BettingHouses.PinnBet) { }

        protected override void UpdateData()
        {
            var jsonMatches = _getter.GetResponse();

            if (jsonMatches == null) { return; }

            foreach (var jsonMatch in jsonMatches)
            {
                if (jsonMatch.sportCode != "F") continue;

                DateTime startTime = DateTime.Parse(jsonMatch.matchStartTime);

                Participant p1 = new(jsonMatch.homeTeamName.Trim());
                Participant p2 = new(jsonMatch.awayTeamName.Trim());

                Match match = new(startTime, p1, p2);

                // Add odds
                var jsonSelections = _getter.GetSingleMatchResponse(jsonMatch.eventId, jsonMatch.roundId);

                foreach (var (selCode, betGame) in betGameFromSelCode)
                {
                    var sel = jsonSelections.FirstOrDefault(x => x.selectionCode.Trim() == selCode);
                    if (sel == null) continue;

                    match.AddBetGame(betGame, sel.odds);
                }

                _data.Insert(match);
            }
        }

        /// <summary>
        /// Map betGames from json response code to BettingGames enum
        /// </summary>
        static readonly Dictionary<string, BettingGames> betGameFromSelCode = new()
        {
            {"k1", BettingGames._1 },
            {"kx", BettingGames._X },
            {"k2", BettingGames._2 },
            {"ds12", BettingGames._12 },
            {"ds1x", BettingGames._1X },
            {"dsx2", BettingGames._X2 },
            {"pp1", BettingGames._1_I },
            {"ppx", BettingGames._X_I },
            {"pp2", BettingGames._2_I },
            {"dp1", BettingGames._1_II },
            {"dpx", BettingGames._X_II },
            {"dp2", BettingGames._2_II },
            {"tgg", BettingGames._GG },
            {"tng", BettingGames._NG },
            {"ug01", BettingGames._UG_0_1 },
            {"ug02", BettingGames._UG_0_2 },
            {"ug03", BettingGames._UG_0_3 },
            {"ug04", BettingGames._UG_0_4 },
            {"ug05", BettingGames._UG_0_5 },
            {"ug12", BettingGames._UG_1_2 },
            {"ug13", BettingGames._UG_1_3 },
            {"ug14", BettingGames._UG_1_4 },
            {"ug15", BettingGames._UG_1_5 },
            {"ug16", BettingGames._UG_1_6 },
            {"ug23", BettingGames._UG_2_3 },
            {"ug24", BettingGames._UG_2_4 },
            {"ug25", BettingGames._UG_2_5 },
            {"ug26", BettingGames._UG_2_6 },
            {"ug34", BettingGames._UG_3_4 },
            {"ug35", BettingGames._UG_3_5 },
            {"ug36", BettingGames._UG_3_6 },
            {"ug45", BettingGames._UG_4_5 },
            {"ug46", BettingGames._UG_4_6 },
            {"ug2v", BettingGames._UG_2_PLUS },
            {"ug3v", BettingGames._UG_3_PLUS },
            {"ug4v", BettingGames._UG_4_PLUS },
            {"ug5v", BettingGames._UG_5_PLUS },
            {"ug6v", BettingGames._UG_6_PLUS }
        };
    }
}
