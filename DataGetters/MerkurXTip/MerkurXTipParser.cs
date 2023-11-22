using Arbitrage.DataGetters.MaxBet;
using Arbitrage.General;
using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.MerkurXTip
{
    public class MerkurXTipParser : Parser
    {
        private readonly MerkurXTipGetter _getter = new();

        public MerkurXTipParser() : base(BettingHouses.MerkurXTip) { }

        private void ParseMatch(JsonMatch jsonMatch)
        {
            Participant participant1 = new(jsonMatch.home.Trim());

            Participant participant2 = new(jsonMatch.away.Trim());

            DateTime startTime = DateTimeConverter.DateTimeFromLong(jsonMatch.kickOffTime, 1); // SSSSSSUUUUUSSSSSSSSS

            var match = new Match(startTime, participant1, participant2);

            // Add odds
            foreach (var (oddID, oddValue) in jsonMatch.odds)
            {
                if (betGameFromID.Keys.Contains(oddID))
                {
                    match.AddBetGame(betGameFromID[oddID], oddValue);
                }
            }

            _data.Insert(match);
        }

        protected override void UpdateData()
        {
            var resp = _getter.GetMatches();

            foreach (var jsonMatch in resp.esMatches)
            {
                ParseMatch(jsonMatch);
            }
        }

        /// <summary>
        /// Map betGames from json response bpc to BettingGames enum
        /// </summary>
        static readonly Dictionary<int, BettingGames> betGameFromID = new()
        {
            {1, BettingGames._1 },
            {2, BettingGames._X },
            {3, BettingGames._2 },
            {8, BettingGames._12 },
            {7, BettingGames._1X },
            {9, BettingGames._X2 },
            {22, BettingGames._UG_0_2 },
            {24, BettingGames._UG_3_PLUS},
            {272, BettingGames._GG},
            // TODO kvote
        };
    }
}
