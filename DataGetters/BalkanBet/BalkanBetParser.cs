using Arbitrage.General;
using Arbitrage.Utils;
using NLog;

namespace Arbitrage.DataGetters.BalkanBet
{
    internal class BalkanBetParser : Parser
    {
        private readonly BalkanBetGetter _getter = new();

        protected static readonly Logger log = LogManager.GetCurrentClassLogger();

        public BalkanBetParser() : base(BettingHouse.BalkanBet) { }

        protected override void ParseFootball()
        {
            var idsResp = _getter.GetMatchIdsFootball();

            foreach(var ev in idsResp.data.events)
            {
                try
                {
                    var resp = _getter.GetMatchResponse(ev.a);
                    ParseMatchResponse(resp, Sport.Football);
                } catch (Exception ex)
                {
                    log.Error("Exception while parsing match response" + ex.Message);
                }
            }
        }

        private void ParseMatchResponse(JsonMatchResponse resp, Sport sport)
        {
            DateTime startTime = DateTime.Parse(resp.data.startsAt);

            string team1 = resp.data.competitors.First(x => x.type == 1).name;
            string team2 = resp.data.competitors.First(x => x.type == 2).name;

            HouseMatchData match = new(House, sport, startTime, team1, team2);

            foreach (var jsonMarket in resp.data.markets)
            {
                if (jsonMarket == null || jsonMarket.outcomes == null)
                {
                    continue;
                }

                foreach (var jsonOdd in jsonMarket.outcomes)
                {
                    try
                    {
                        var sgName = jsonOdd.name;
                        if (betGameFromString.ContainsKey(sgName))
                        {
                            BetGame game = betGameFromString[sgName].Clone();
                            game.Value = jsonOdd.odd;
                            match.AddBetGame(game);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Exception while parsing betgame: " + e);
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
            {"I 1", new(BetGameType.WX1, GamePeriod.H1) },
            {"I X", new(BetGameType.WXX, GamePeriod.H1) },
            {"I 2", new(BetGameType.WX2, GamePeriod.H1) },
            {"I 12", new(BetGameType.D12, GamePeriod.H1) },
            {"I 1X", new(BetGameType.D1X, GamePeriod.H1) },
            {"I X2", new(BetGameType.DX2, GamePeriod.H1) },
            {"II 1", new(BetGameType.WX1, GamePeriod.H2) },
            {"II X", new(BetGameType.WXX, GamePeriod.H2) },
            {"II 2", new(BetGameType.WX2, GamePeriod.H2) },
            {"II 12", new(BetGameType.D12, GamePeriod.H2) },
            {"II 1X", new(BetGameType.D1X, GamePeriod.H2) },
            {"II X2", new(BetGameType.DX2, GamePeriod.H2) },
            {"GG", new(BetGameType.GG) },
            {"NG", new(BetGameType.NG) },
            {"I GG", new(BetGameType.GG, GamePeriod.H1) },
            {"NE I GG", new(BetGameType.NG, GamePeriod.H1) },
            {"II GG", new(BetGameType.GG, GamePeriod.H2) },
            {"NE 2 GG", new(BetGameType.NG, GamePeriod.H2) },
            {"0-1", new(BetGameType.UNDER, thr: 1.5) },
            {"0-2", new(BetGameType.UNDER, thr: 2.5) },
            {"0-3", new(BetGameType.UNDER, thr: 3.5) },
            {"0-4", new(BetGameType.UNDER, thr: 4.5) },
            {"2+", new(BetGameType.OVER, thr: 1.5) },
            {"3+", new(BetGameType.OVER, thr: 2.5)},
            {"4+", new(BetGameType.OVER, thr: 3.5) },
            {"5+", new(BetGameType.OVER, thr: 4.5) },
        };
    }
}
