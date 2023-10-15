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
        private MaxBetGetter _getter = new MaxBetGetter();

        public MaxBetParser()
        {
            _data = new MatchesData(BettingHouses.MaxBet);
        }

        protected override void UpdateData(DateTime dateTime)
        {
            var leagueIDs = _getter.GetLeagues();

            foreach (var leagueID in leagueIDs)
            {
                //specials
                if(leagueID == "138547")
                {
                    continue;
                }
                JsonMatchResponse resp = _getter.GetMatches(leagueID);
                if (resp == null)
                {
                    continue;
                }

                foreach (var match in resp.esMatches)
                {
                    //DateTime startTime = new DateTime(match.kickOffTime);

                    Participant participant1 = new Participant(match.home);
                    Participant participant2 = new Participant(match.away);

                    Match newMatch = new Match(match.kickOffTime, participant1, participant2);

                    foreach (var (id, betGame) in betGameFromString)
                    { 
                        if (match.odds.ContainsKey(id))
                        { 
                            newMatch.AddBetGame(betGame, match.odds[id]);
                        }
                    }

                    _data.Insert(newMatch);
                }
            }

        }

        static Dictionary<int, BettingGames> betGameFromString = new Dictionary<int, BettingGames> {
            {1, BettingGames._1 },
            {2, BettingGames._X },
            {3, BettingGames._2 },
            {22, BettingGames._0_TO_2 },
            {242, BettingGames._2_OR_MORE },
            {24, BettingGames._3_OR_MORE },
            {8, BettingGames._12 },
            {7, BettingGames._1X },
            {9, BettingGames._X2 } // TODO kvote
        };
    }
}
