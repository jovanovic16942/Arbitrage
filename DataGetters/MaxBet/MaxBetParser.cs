using Arbitrage.DataGetters.Meridian;
using Arbitrage.General;
using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.MaxBet
{
    internal class MaxBetParser : Parser
    {
        private readonly MaxBetGetter _getter = new();

        public MaxBetParser() : base(BettingHouses.MaxBet) { }

        private void ParseMatch(JsonMatch jsonMatch)
        {
            DateTime startTime = DateTimeConverter.DateTimeFromLong(jsonMatch.kickOffTime, 2);

            Participant participant1 = new(jsonMatch.home);
            Participant participant2 = new(jsonMatch.away);

            Match match = new(startTime, participant1, participant2);

            // Add odds
            foreach (var (id, betGame) in betGameFromInt)
            {
                if (jsonMatch.odds.ContainsKey(id))
                {
                    match.AddBetGame(betGame, jsonMatch.odds[id]);
                }
            }

            _data.Insert(match);
        }

        private void ParseLeagues(List<string> leagueIDs)
        {
            foreach (var leagueID in leagueIDs)
            {
                //specials
                if (leagueID == "138547")
                {
                    continue;
                }

                //Console.WriteLine("MaxBetParser leagueID: " + leagueID);

                JsonMatchResponse resp = _getter.GetMatches(leagueID);
                if (resp == null)
                {
                    continue;
                }

                foreach (var match in resp.esMatches)
                {
                    ParseMatch(match);
                }
            }
        }

        protected override void UpdateData()
        {
            var leagueIDs = _getter.GetLeagues();
            ParseLeagues(leagueIDs);
        }

        /// <summary>
        /// Map bet game id from json response to BettingGames enum
        /// </summary>
        static readonly Dictionary<int, BettingGames> betGameFromInt = new()
        {
            {1, BettingGames._1 },
            {2, BettingGames._X },
            {3, BettingGames._2 },
            {22, BettingGames._0_TO_2 },
            {242, BettingGames._2_OR_MORE },
            {24, BettingGames._3_OR_MORE },
            {8, BettingGames._12 },
            {7, BettingGames._1X },
            {9, BettingGames._X2 }
        };
    }
}
