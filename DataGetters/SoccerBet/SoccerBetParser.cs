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
        private SoccerBetGetter _getter = new SoccerBetGetter();

        public SoccerBetParser()
        {
            _data = new MatchesData(BettingHouses.SoccerBet);
        }
        protected override void UpdateData()
        {
            var resp = _getter.GetMatches();

            foreach (var jsonMatch in resp.esMatches)
            {
                Participant participant1 = new Participant(jsonMatch.home.Trim());
                
                Participant participant2 = new Participant(jsonMatch.away.Trim());

                DateTime dateTime = DateTimeConverter.DateTimeFromLong(jsonMatch.kickOffTime);

                var match = new Match(jsonMatch.kickOffTime, participant1, participant2);

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
        }

        // Map betGames from json response bpc to BettingGames enum
        static Dictionary<int, BettingGames> betGameFromBPC = new Dictionary<int, BettingGames> {
            {92212, BettingGames._1 },
            {92213, BettingGames._X },
            {92214, BettingGames._2 },
            {92277, BettingGames._0_TO_2 },
            {92289, BettingGames._2_OR_MORE },
            {92279, BettingGames._3_OR_MORE },
            {92216, BettingGames._12 },
            {92215, BettingGames._1X },
            {92217, BettingGames._X2 } // TODO kvote
        };
    }
}
