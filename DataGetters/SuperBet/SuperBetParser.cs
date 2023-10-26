using Arbitrage.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.SuperBet
{
    public class SuperBetParser : Parser
    {
        private readonly SuperBetGetter _getter = new();

        public SuperBetParser() : base(BettingHouses.SuperBet) { }

        protected override void UpdateData()
        {
            var resp = _getter.GetMatches();

            var footballMatches = resp.data.Where(x => x.si == 5).ToList();

            List<int> footbalIds = new();

            footbalIds = footballMatches.Select(x => x._id).ToList();

            int step = 50;

            for(int i = 0; (i * step) < footbalIds.Count; i++)
            {
                int total = step * i;

                var fullResponse = _getter.GetMatchData(footbalIds.Skip(total).Take(step));

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
