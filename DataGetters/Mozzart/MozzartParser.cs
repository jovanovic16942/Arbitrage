using Arbitrage.General;
using Arbitrage.Utils;
using NLog;

namespace Arbitrage.DataGetters.Mozzart
{
    public class MozzartParser : Parser
    {
        private readonly MozzartGetter _getter = new();

        /// <summary>
        /// This maps json response match ID to our classes, this is a special case for Mozzart
        /// </summary>
        private readonly Dictionary<int, HouseMatchData> _mozzartIdToMatchData = new();

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public MozzartParser() : base(BettingHouse.Mozzart) {}

        protected override void ParseFootball()
        {
            // Call GetMatches and check responses
            JsonMatchResponse matchResponse;
            try
            {
                matchResponse = _getter.GetMatches();
            } catch (Exception ex)
            {
                logger.Error("Exception thrown in Getter: " + ex.Message);
                return;
            }

            // removed matches that expect n+ betGames in one ticket
            matchResponse.Matches = matchResponse.Matches.Where(x => x.SpecialType == 0).ToList();

            if (matchResponse == null || matchResponse.Matches == null)
            {
                logger.Error("Failed to get match response!");
                return;
            }

            else if (matchResponse.Total == 0 || matchResponse.Matches.Count == 0)
            {
                logger.Warn("Match response contains no matches!");
                return;
            }

            ParseMatches(matchResponse);

            var matchIDs = _mozzartIdToMatchData.Keys;
            var oddsResponse = _getter.GetOdds(matchIDs);
            ParseOdds(oddsResponse);

            foreach (var matchData in _mozzartIdToMatchData.Values)
            {
                _parsedData.Add(matchData);
            }
        }

        private void ParseMatches(JsonMatchResponse resp)
        {
            foreach (var match in resp.Matches)
            {
                try
                {
                    var participants = match.Participants.ToList();
                    if (participants.Count != 2) continue;

                    var p1 = participants[0];
                    var p2 = participants[1];

                    DateTime startTime = DateTimeConverter.DateTimeFromLong(match.StartTime, 1); // TODO SUUUSSSSSS  Promena sata daylight savings bblabla

                    // TODO get sport from match.Competition.Sport.Id
                    HouseMatchData matchData = new(BettingHouse.Mozzart, Sport.Football, startTime, p1.Name, p2.Name);

                    _mozzartIdToMatchData.Add(match.Id, matchData);
                }
                catch
                {
                    
                }
            }

        }
        
        private void ParseOdds(List<JsonRoot> resp)
        {
            foreach (var matchOdds in resp)
            {
                var matchId = matchOdds.id;
                var kodds = matchOdds.kodds;

                foreach (var kvp in kodds)
                {
                    string subGameID = kvp.Key.Trim(); // Mozzart.com subGameID - trenutno subgamesIds u MozzartGetter
                    JsonKodds kodd = kvp.Value;

                    if (kodd == null) continue;

                    double betValue = double.Parse(kodd.value); // Kvota

                    if (!betGameFromID.ContainsKey(subGameID))
                    {
                        logger.Trace("Unknown betting game: " + kvp.ToString());
                        continue;
                    }

                    BetGame game = betGameFromID[subGameID].Clone();
                    game.Value = betValue;

                    _mozzartIdToMatchData[matchId].AddBetGame(game);
                }
            }
        }

        static readonly Dictionary<string, BetGame> betGameFromID = new()
        {
            {"1001130001", new(BetGameType.GG) },
            {"1001130002", new(BetGameType.NG) },
            {"1001130004", new(BetGameType.GG, GamePeriod.H1) },
            {"1001130010", new(BetGameType.NG, GamePeriod.H1) },
            {"1001130005", new(BetGameType.GG, GamePeriod.H2) },
            {"1001130011", new(BetGameType.NG, GamePeriod.H2) },

            {"1001001001", new(BetGameType.WX1) },
            {"1001001002", new(BetGameType.WXX) },
            {"1001001003", new(BetGameType.WX2) },
            {"1001002002", new(BetGameType.D12) },
            {"1001002001", new(BetGameType.D1X) },
            {"1001002003", new(BetGameType.DX2) },

            {"1001004001", new(BetGameType.WX1, GamePeriod.H1) },
            {"1001004002", new(BetGameType.WXX, GamePeriod.H1) },
            {"1001004003", new(BetGameType.WX2, GamePeriod.H1) },
            {"1001297002", new(BetGameType.D12, GamePeriod.H1) },
            {"1001297001", new(BetGameType.D1X, GamePeriod.H1) },
            {"1001297003", new(BetGameType.DX2, GamePeriod.H1) },
            
            {"1001019001", new(BetGameType.WX1, GamePeriod.H2) },
            {"1001019002", new(BetGameType.WXX, GamePeriod.H2) },
            {"1001019003", new(BetGameType.WX2, GamePeriod.H2) },
            //{"", new(BetGameType.D12, GamePeriod.H2) },
            //{"", new(BetGameType.D1X, GamePeriod.H2) },
            //{"", new(BetGameType.DX2, GamePeriod.H2) },

            {"1001003001", new(BetGameType.UNDER, thr: 1.5) },
            {"1001003002", new(BetGameType.UNDER, thr: 2.5) },
            {"1001003013", new(BetGameType.UNDER, thr: 3.5) },
            {"1001003026", new(BetGameType.UNDER, thr: 4.5) },
            {"1001003012", new(BetGameType.OVER, thr: 1.5) },
            {"1001003004", new(BetGameType.OVER, thr: 2.5) },
            {"1001003005", new(BetGameType.OVER, thr: 3.5) },
            {"1001003007", new(BetGameType.OVER, thr: 4.5) },
            // TODO MORE
            
            
            {"1001008008", new(BetGameType.UNDER, GamePeriod.H1, thr: 0.5) },
            {"1001008005", new(BetGameType.UNDER, GamePeriod.H1, thr: 1.5) },
            {"1001008009", new(BetGameType.UNDER, GamePeriod.H1, thr: 2.5) },
            {"1001008001", new(BetGameType.OVER, GamePeriod.H1, thr: 0.5) },
            {"1001008002", new(BetGameType.OVER, GamePeriod.H1, thr: 1.5) },
            {"1001008003", new(BetGameType.OVER, GamePeriod.H1, thr: 2.5) },

            {"1001009008", new(BetGameType.UNDER, GamePeriod.H2, thr: 0.5) },
            {"1001009005", new(BetGameType.UNDER, GamePeriod.H2, thr: 1.5) },
            {"1001009009", new(BetGameType.UNDER, GamePeriod.H2, thr: 2.5) },
            {"1001009001", new(BetGameType.OVER, GamePeriod.H2, thr: 0.5) },
            {"1001009002", new(BetGameType.OVER, GamePeriod.H2, thr: 1.5) },
            {"1001009003", new(BetGameType.OVER, GamePeriod.H2, thr: 2.5) },


            {"1001131003", new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 0.5) },
            {"1001131009", new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 1.5) },
            {"1001131010", new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 2.5) },
            {"1001131001", new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 0.5) },
            {"1001131002", new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 1.5) },
            {"1001131008", new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 2.5) },

            {"1001128003", new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 0.5) },
            {"1001128004", new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 1.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 2.5) },
            {"1001128001", new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 0.5) },
            {"1001128002", new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 1.5) },
            {"1001128007", new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 2.5) },

            {"1001142003", new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 0.5) },
            {"1001142004", new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 1.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 2.5) },
            {"1001142001", new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 0.5) },
            {"1001142002", new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 1.5) },
            {"1001142007", new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 2.5) },

            {"1001132003", new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 0.5) },
            {"1001132009", new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 1.5) },
            {"1001132010", new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 2.5) },
            {"1001132001", new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 0.5) },
            {"1001132002", new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 1.5) },
            {"1001132008", new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 2.5) },

            {"1001129003", new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 0.5) },
            {"1001129004", new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 1.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 2.5) },
            {"1001129001", new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 0.5) },
            {"1001129002", new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 1.5) },
            {"1001129007", new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 2.5) },

            {"1001143003", new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 0.5) },
            {"1001143004", new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 1.5) },
            {"1001143005", new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 2.5) },
            {"1001143001", new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 0.5) },
            {"1001143002", new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 1.5) },
            {"1001143007", new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 2.5) },
        };
    }
}
