﻿using Arbitrage.General;
using Arbitrage.Utils;

namespace Arbitrage.DataGetters.SuperBet
{
    public class SuperBetParser : Parser
    {
        private readonly SuperBetGetter _getter = new();

        public SuperBetParser() : base(BettingHouses.SuperBet) { }

        protected override void UpdateData()
        {
            var resp = _getter.GetMatches();

            var footballMatches = resp.data.Where(x => x.si == 5).ToList(); // sportId 5 - fudbal

            List<int> footbalIds = new();

            footbalIds = footballMatches.Select(x => x._id).ToList();

            int step = 50;

            for(int i = 0; (i * step) < footbalIds.Count; i++)
            {
                int total = step * i;

                var fullResponse = _getter.GetMatchData(footbalIds.Skip(total).Take(step));

                foreach(var jsonMatch in fullResponse.data) 
                {
                    try
                    {
                        ParseJsonMatch(jsonMatch);
                    } catch(Exception e)
                    {
                        // Loggovanje
                    }
                }
            }
        }

        void ParseJsonMatch(JsonMatch jsonMatch)
        {
            if (jsonMatch == null) return;

            DateTime starTime = DateTime.Parse(jsonMatch.mld);
            List<Participant> participants = jsonMatch.mn.Split("·").Select(x => new Participant(x)).ToList();

            Match match = new(starTime, participants[0], participants[1]);

            foreach(var jsonOdd in jsonMatch.odds)
            {
                int betGameId = jsonOdd.oi;
                
                // Special parsing for UNDER - OVER
                if (betGameId == 151888 || betGameId == 151889) 
                {
                    betGameId += 1000000 * (int)Math.Truncate(jsonOdd.spc.total);
                }

                if (!betGameFromOI.ContainsKey(betGameId)) continue;

                match.AddBetGame(betGameFromOI[betGameId], jsonOdd.ov);
            }

            _data.Insert(match);
        }


        /// <summary>
        /// Map betGames from json response oi (odd id) to BettingGames enum
        /// </summary>
        static readonly Dictionary<int, BettingGames> betGameFromOI = new()
        {
            {1470, BettingGames._1 },
            {1471, BettingGames._X },
            {1472, BettingGames._2 },
            {1364, BettingGames._12 },
            {1363, BettingGames._1X },
            {1365, BettingGames._X2 },
            {1440, BettingGames._GG },
            {1441, BettingGames._NG },
            //
            {1520, BettingGames._1_I },
            {1521, BettingGames._X_I },
            {1522, BettingGames._2_I },
            {1492, BettingGames._12_I},
            {1491, BettingGames._1X_I},
            {1493, BettingGames._X2_I},
            {1527, BettingGames._GG_I},
            {1528, BettingGames._NG_I},
            //
            {1511, BettingGames._1_II },
            {1512, BettingGames._X_II },
            {1513, BettingGames._2_II },
            {1570, BettingGames._12_II},
            {1569, BettingGames._1X_II},
            {1571, BettingGames._X2_II},
            {1475, BettingGames._GG_II},
            {1476, BettingGames._NG_II},
            //

            {151888, BettingGames._UG_0 },
            {151889, BettingGames._UG_1_PLUS },
            {1151888, BettingGames._UG_0_1 },
            {1151889, BettingGames._UG_2_PLUS },
            {2151888, BettingGames._UG_0_2 },
            {2151889, BettingGames._UG_3_PLUS },
            {3151888, BettingGames._UG_0_3 },
            {3151889, BettingGames._UG_4_PLUS },
            {4151888, BettingGames._UG_0_4 },
            {4151889, BettingGames._UG_5_PLUS },
            {5151888, BettingGames._UG_0_5 },
            {5151889, BettingGames._UG_6_PLUS },
            {6151888, BettingGames._UG_0_6 },
            {6151889, BettingGames._UG_7_PLUS }
        };
    }
}
