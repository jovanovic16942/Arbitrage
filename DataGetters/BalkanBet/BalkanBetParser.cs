using Arbitrage.DataGetters.PinnBet;
using Arbitrage.General;
using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.BalkanBet
{
    internal class BalkanBetParser : Parser
    {
        private readonly BalkanBetGetter _getter = new();

        public BalkanBetParser() : base(BettingHouse.BalkanBet) { }

        protected override void UpdateData()
        {
            var idsResp = _getter.GetMatchIds();

            foreach(var ev in idsResp.data.events)
            {
                try
                {
                    var resp = _getter.GetMatchResponse(ev.a);
                    ParseMatchResponse(resp);
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void ParseMatchResponse(JsonMatchResponse resp)
        {
            DateTime startTime = DateTime.Parse(resp.data.startsAt);

            Participant participant1 = new(resp.data.competitors.First(x => x.type == 1).name);
            Participant participant2 = new(resp.data.competitors.First(x => x.type == 2).name);

            Match match = new(startTime, participant1, participant2);

            foreach (var jsonMarket in resp.data.markets)
            {
                foreach (var jsonOdd in jsonMarket.outcomes)
                {
                    var sgName = jsonOdd.name;
                    if (betGameFromString.ContainsKey(sgName))
                    {
                        match.TryAddBetGame(betGameFromString[sgName], jsonOdd.odd);
                    }
                }
            }

            _data.Insert(match);
        }

        /// <summary>
        /// Map bet game name from json response to BetGameConfig
        /// </summary>
        static readonly Dictionary<string, BettingGames> betGameFromString = new()
        {
            {"1", BettingGames._1 },
            {"X", BettingGames._X },
            {"2", BettingGames._2 },
            {"12", BettingGames._12 },
            {"1X", BettingGames._1X },
            {"X2",BettingGames._X2 },
            {"I 1", BettingGames._1_I },
            {"I X", BettingGames._X_I },
            {"I 2", BettingGames._2_I },
            {"I 12", BettingGames._12_I },
            {"I 1X", BettingGames._1X_I },
            {"I X2", BettingGames._X2_I },
            {"II 1", BettingGames._1_II },
            {"II X", BettingGames._X_II },
            {"II 2", BettingGames._2_II },
            {"II 12", BettingGames._12_II },
            {"II 1X", BettingGames._1X_II },
            {"II X2", BettingGames._X2_II },
            {"GG", BettingGames._GG },
            {"NG", BettingGames._NG },
            {"I GG", BettingGames._GG_I },
            {"NE I GG", BettingGames._NG_I },
            {"II GG", BettingGames._GG_II },
            {"NE 2 GG", BettingGames._NG_II },
            {"0-1", BettingGames._UG_0_1 },
            {"0-2", BettingGames._UG_0_2 },
            {"0-3", BettingGames._UG_0_3 },
            {"0-4", BettingGames._UG_0_4 },
            {"2+", BettingGames._UG_2_PLUS },
            {"23", BettingGames._UG_2_3 },
            {"3+", BettingGames._UG_3_PLUS },
            {"4+", BettingGames._UG_4_PLUS },
            {"5+", BettingGames._UG_5_PLUS },
        };
    }
}
