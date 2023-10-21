using Arbitrage.DataGetters.MaxBet;
using Arbitrage.General;
using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.SoccerBet
{
    public class SoccerBetParser : Parser
    {
        private readonly SoccerBetGetter _getter = new();

        public SoccerBetParser() : base(BettingHouses.SoccerBet) { }

        private void ParseMatch(JsonMatch jsonMatch)
        {
            Participant participant1 = new(jsonMatch.home.Trim());

            Participant participant2 = new(jsonMatch.away.Trim());

            DateTime startTime = DateTimeConverter.DateTimeFromLong(jsonMatch.kickOffTime, 2);

            var match = new Match(startTime, participant1, participant2);

            // Add odds
            foreach (var x in jsonMatch.betMap.Values)
            {
                var oddJson = x.Values.First();
                var bpc = oddJson.bpc;

                if (betGameFromBPC.Keys.Contains(bpc))
                {
                    match.AddBetGame(betGameFromBPC[bpc], oddJson.ov);
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
        static readonly Dictionary<int, BettingGames> betGameFromBPC = new()
        {
            {92212, BettingGames._1 },
            {92213, BettingGames._X },
            {92214, BettingGames._2 },
            {92277, BettingGames._UG_0_2 }, // TODO CHECK
            {92289, BettingGames._UG_2_PLUS },
            {92279, BettingGames._UG_3_PLUS },
            {92216, BettingGames._12 },
            {92215, BettingGames._1X },
            {92217, BettingGames._X2 } // TODO kvote
        };
    }
}
