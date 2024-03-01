using Arbitrage.General;
using Arbitrage.Utils;
using NLog;

namespace Arbitrage.DataGetters.StarBet
{
    public class StarBetParser : Parser
    {
        private readonly StarBetGetter _getter = new();

        protected static readonly Logger log = LogManager.GetCurrentClassLogger();

        public StarBetParser() : base(BettingHouse.StarBet) { }

        protected override void ParseFootball()
        {
            var leagueResp = _getter.GetLeagues();

            foreach (var leagues in leagueResp)
            {
                if (leagues.SN == "Fudbal")
                {
                    ParseLeaguesResponse(leagues, Sport.Football);
                }
            }
        }

        private void ParseLeaguesResponse(JsonLeagueResponse leagues, Sport sport)
        {
            List<int> leagueIDs = leagues.L.Select(x => x.LID).ToList();

            var matchesResp = _getter.GetMatchesResponse(leagueIDs);

            foreach (var matchResp in matchesResp)
            {
                if (matchResp == null || matchResp.P == null) continue;

                foreach (var jsonPair in matchResp.P)
                {
                    try
                    {
                        ParseJsonPair(jsonPair, sport);
                    } catch (Exception e)
                    {
                        log.Error("Exception while parsing jsonPair: " + e);
                    }
                }
            }
        }

        private void ParseJsonPair(JsonPair jsonPair, Sport sport)
        {
            DateTime startTime = DateTime.Parse(jsonPair.DI);

            var tokens = jsonPair.PN.Split(':');

            HouseMatchData match = new(House, sport, startTime, tokens[0], tokens[1]);

            var oddsResp = _getter.GetOdds(jsonPair.PID);

            foreach (var jsonOddResp in oddsResp)
            {
                if (jsonOddResp == null || jsonOddResp.T == null) continue;

                foreach (var jsonOdd in jsonOddResp.T)
                {
                    try
                    {
                        var oddString = jsonOdd.TipPecatiWeb.Trim();
                        if (betGameFromString.ContainsKey(oddString))
                        {
                            BetGame bg = betGameFromString[oddString].Clone();
                            bg.Value = jsonOdd.Kvota;
                            match.AddBetGame(bg);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Exception while parsing jsonOdd: " + e);
                    }
                }
            }

            _parsedData.Add(match);
        }

        /// <summary>
        /// Map bet game name from json response to BetGame
        /// </summary>
        static readonly Dictionary<string, BetGame> betGameFromString = new()
        {
            {"1", new(BetGameType.WX1) },
            {"X", new(BetGameType.WXX) },
            {"2", new(BetGameType.WX2) },
            {"12", new(BetGameType.D12) },
            {"1X", new(BetGameType.D1X) },
            {"X2", new(BetGameType.DX2) },
            {"1 I", new(BetGameType.WX1, GamePeriod.H1) },
            {"X I", new(BetGameType.WXX, GamePeriod.H1) },
            {"2 I", new(BetGameType.WX2, GamePeriod.H1) },
            {"12 I", new(BetGameType.D12, GamePeriod.H1) },
            {"1X I", new(BetGameType.D1X, GamePeriod.H1) },
            {"X2 I", new(BetGameType.DX2, GamePeriod.H1) },
            {"1 II", new(BetGameType.WX1, GamePeriod.H2) },
            {"X II", new(BetGameType.WXX, GamePeriod.H2) },
            {"2 II", new(BetGameType.WX2, GamePeriod.H2) },
            {"12 II", new(BetGameType.D12, GamePeriod.H2) },
            {"1X II", new(BetGameType.D1X, GamePeriod.H2) },
            {"X2 II", new(BetGameType.DX2, GamePeriod.H2) },
            {"GG", new(BetGameType.GG) },
            {"NG", new(BetGameType.NG) },
            {"GG I", new(BetGameType.GG, GamePeriod.H1) },
            {"NG I", new(BetGameType.NG, GamePeriod.H1) },
            {"GG II", new(BetGameType.GG, GamePeriod.H2) },
            {"NG II", new(BetGameType.NG, GamePeriod.H2) },
            {"0-1 golova", new(BetGameType.UNDER, thr: 1.5) },
            {"0-2 golova", new(BetGameType.UNDER, thr: 2.5) },
            {"0-3 golova", new(BetGameType.UNDER, thr: 3.5) },
            {"0-4 golova", new(BetGameType.UNDER, thr: 4.5) },
            {"2+", new(BetGameType.OVER, thr: 1.5) },
            {"3+", new(BetGameType.OVER, thr: 2.5) },
            {"4+", new(BetGameType.OVER, thr: 3.5) },
            {"5+", new(BetGameType.OVER, thr: 4.5) },
        };
    }
}
