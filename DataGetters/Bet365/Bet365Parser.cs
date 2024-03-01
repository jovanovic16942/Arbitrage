using Arbitrage.General;
using Arbitrage.Utils;

namespace Arbitrage.DataGetters.Bet365
{
    internal class Bet365Parser : Parser
    {
        private readonly Bet365Getter _getter = new();

        private List<JsonSport> _sportResp;

        public Bet365Parser() : base(BettingHouse.Bet365)
        {
        }

        protected override void ParseFootball()
        {
            _sportResp ??= _getter.GetLeagues();

            if (_sportResp == null || _sportResp.Count == 0) { return; }

            foreach (var jsonSport in _sportResp) 
            {
                if (jsonSport.sportType != "S") continue;
                ParseJsonSport(jsonSport, Sport.Football);
            }
        }

        private void ParseJsonSport(JsonSport jsonSport, Sport sport)
        {
            if (jsonSport == null || jsonSport.leagues == null || jsonSport.leagues.Count == 0) { return; }

            foreach (var jsonLeague in jsonSport.leagues)
            {
                ParseJsonLeague(jsonLeague, sport);
            }
        }

        private void ParseJsonLeague(JsonLeague jsonLeague, Sport sport)
        {
            if (jsonLeague == null || jsonLeague.numOfMatches == 0) { return; }

            var leagueId = jsonLeague.betLeagueId;

            try
            {
                var leagueMatchesResp = _getter.GetMatchesInLeague(leagueId);
                
                foreach (var jsonMatch in leagueMatchesResp.matchList)
                {
                    TryParseJsonMatch(jsonMatch, sport);
                }

            } catch (Exception e)
            {
                // log
            }
        }

        private void TryParseJsonMatch(JsonMatch jsonMatch, Sport sport)
        {
            try
            {
                DateTime startTime = DateTimeConverter.DateTimeFromLong(jsonMatch.kickOffTime, 1);

                HouseMatchData hmd = new(House, sport, startTime, jsonMatch.home, jsonMatch.away);

                // Add odds
                var matchId = jsonMatch.id;
                var jsonMatchResp = _getter.GetMatchResponse(matchId);

                foreach (var jsonBetGroup in jsonMatchResp.odBetPickGroups)
                {
                    TryParseBetGroup(hmd, jsonBetGroup);
                }

                _parsedData.Add(hmd);
            }
            catch
            {
                // log
                return;
            }
        }

        private void TryParseBetGroup(HouseMatchData hmd, JsonBetGroup jsonBetGroup)
        {
            if (jsonBetGroup.tipTypes == null || !jsonBetGroup.tipTypes.Any()) { return; } // log

            foreach (var jsonBet in jsonBetGroup.tipTypes)
            {
                try
                {
                    if (betGameFromString.ContainsKey(jsonBet.name))
                    {
                        BetGame game = betGameFromString[jsonBet.name].Clone();
                        game.Value =  jsonBet.value;
                        hmd.AddBetGame(game);
                    }
                } catch
                {
                    continue; // log
                }
            }
        }

        /// <summary>
        /// Map bet game name from json response name to BetGame
        /// </summary>
        static readonly Dictionary<string, BetGame> betGameFromString = new()
        {
            {"1", new(BetGameType.WX1) },
            {"X", new(BetGameType.WXX) },
            {"2", new(BetGameType.WX2) },
            {"12", new(BetGameType.D12) },
            {"1X", new(BetGameType.D1X) },
            {"X2", new(BetGameType.DX2) },
            {"1P1", new(BetGameType.WX1, GamePeriod.H1) },
            {"1PX", new(BetGameType.WXX, GamePeriod.H1) },
            {"1P2", new(BetGameType.WX2, GamePeriod.H1) },
            {"1P 12", new(BetGameType.D12, GamePeriod.H1) },
            {"1P 1X", new(BetGameType.D1X, GamePeriod.H1) },
            {"1P X2", new(BetGameType.DX2, GamePeriod.H1) },
            {"2P1", new(BetGameType.WX1, GamePeriod.H2) },
            {"2PX", new(BetGameType.WXX, GamePeriod.H2) },
            {"2P2", new(BetGameType.WX2, GamePeriod.H2) },
            {"2P 12", new(BetGameType.D12, GamePeriod.H2) },
            {"2P 1X", new(BetGameType.D1X, GamePeriod.H2) },
            {"2P X2", new(BetGameType.DX2, GamePeriod.H2) },
            {"GG", new(BetGameType.GG) },
            {"NG", new(BetGameType.NG) },
            {"1P GG", new(BetGameType.GG, GamePeriod.H1) },
            {"1P NG", new(BetGameType.NG, GamePeriod.H1) },
            {"2P GG", new(BetGameType.GG, GamePeriod.H2) },
            {"2P NG", new(BetGameType.NG, GamePeriod.H2) },

            {"0-1", new(BetGameType.UNDER,  thr: 1.5) },
            {"0-2", new(BetGameType.UNDER,  thr: 2.5) },
            {"0-3", new(BetGameType.UNDER,  thr: 3.5) },
            {"0-4", new(BetGameType.UNDER,  thr: 4.5) },
            {"0-5", new(BetGameType.UNDER, thr : 5.5) },
            {"0-6", new(BetGameType.UNDER, thr : 6.5) },
            {"2+", new(BetGameType.OVER,  thr: 1.5) },
            {"3+", new(BetGameType.OVER,  thr: 2.5) },
            {"4+", new(BetGameType.OVER,  thr: 3.5) },
            {"5+", new(BetGameType.OVER,  thr: 4.5) },
            {"6+", new(BetGameType.OVER,  thr: 5.5) },
        };
    }
}
