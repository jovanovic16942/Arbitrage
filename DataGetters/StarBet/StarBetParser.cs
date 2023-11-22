using Arbitrage.General;
using Arbitrage.Utils;

namespace Arbitrage.DataGetters.StarBet
{
    public class StarBetParser : Parser
    {
        private readonly StarBetGetter _getter = new();

        public StarBetParser() : base(BettingHouses.StarBet) { }

        protected override void UpdateData()
        {
            var leagueResp = _getter.GetLeagues();

            foreach (var leagues in leagueResp)
            {
                ParseLeaguesResponse(leagues);
            }

        }

        private void ParseLeaguesResponse(JsonLeagueResponse leagues)
        {
            if (leagues.SN != "Fudbal") return;

            List<int> leagueIDs = leagues.L.Select(x => x.LID).ToList();

            var matchesResp = _getter.GetMatchesResponse(leagueIDs);

            foreach (var matchResp in matchesResp)
            {
                if (matchResp == null || matchResp.P == null) continue;

                foreach (var jsonPair in matchResp.P)
                {
                    try
                    {
                        ParseJsonPair(jsonPair);
                    } catch (Exception e)
                    {
                        //Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        private void ParseJsonPair(JsonPair jsonPair)
        {
            DateTime startTime = DateTime.Parse(jsonPair.DI);

            var tokens = jsonPair.PN.Split(':');

            Participant p1 = new(tokens[0].Trim());
            Participant p2 = new(tokens[1].Trim());

            Match match = new(startTime, p1, p2);

            var oddsResp = _getter.GetOdds(jsonPair.PID);

            foreach (var jsonOddResp in oddsResp)
            {
                if (jsonOddResp == null || jsonOddResp.T == null) continue;

                foreach (var jsonOdd in jsonOddResp.T)
                {
                    var oddString = jsonOdd.TipPecatiWeb.Trim();
                    if (betGameFromString.ContainsKey(oddString))
                    {
                        match.AddBetGame(betGameFromString[oddString], jsonOdd.Kvota);
                    }
                }
            }

            _data.Insert(match);
        }

        /// <summary>
        /// Map bet game name from json response to BettingGames enum
        /// </summary>
        static readonly Dictionary<string, BettingGames> betGameFromString = new()
        {
            {"1", BettingGames._1 },
            {"X", BettingGames._X },
            {"2", BettingGames._2 },
            {"12", BettingGames._12 },
            {"1X", BettingGames._1X },
            {"X2",BettingGames._X2 },
            {"1 I", BettingGames._1_I },
            {"X I", BettingGames._X_I },
            {"2 I", BettingGames._2_I },
            {"12 I", BettingGames._12_I },
            {"1X I", BettingGames._1X_I },
            {"X2 I", BettingGames._X2_I },
            {"1 II", BettingGames._1_II },
            {"X II", BettingGames._X_II },
            {"2 II", BettingGames._2_II },
            {"12 II", BettingGames._12_II },
            {"1X II", BettingGames._1X_II },
            {"X2 II", BettingGames._X2_II },
            {"GG", BettingGames._GG },
            {"NG", BettingGames._NG },
            {"GG I", BettingGames._GG_I },
            {"NG I", BettingGames._NG_I },
            {"GG II", BettingGames._GG_II },
            {"NG II", BettingGames._NG_II },
            {"0-1 golova", BettingGames._UG_0_1 },
            {"0-2 golova", BettingGames._UG_0_2 },
            {"0-3 golova", BettingGames._UG_0_3 },
            {"0-4 golova", BettingGames._UG_0_4 },
            {"2+", BettingGames._UG_2_PLUS },
            {"2-3 golova", BettingGames._UG_2_3 },
            {"3+", BettingGames._UG_3_PLUS },
            {"4+", BettingGames._UG_4_PLUS },
            {"5+", BettingGames._UG_5_PLUS },
        };
    }
}
