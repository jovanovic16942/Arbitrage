using Arbitrage.General;
using Arbitrage.Utils;
using NLog;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

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
        public MozzartParser() : base(BettingHouse.Mozzart) 
        {
            logger.Info("MozzartParser created!");
        }

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

            //{"", new(BetGameType.WX1, GamePeriod.H1) },
            //{"", new(BetGameType.WXX, GamePeriod.H1) },
            //{"", new(BetGameType.WX2, GamePeriod.H1) },
            {"1001297002", new(BetGameType.D12, GamePeriod.H1) },
            {"1001297001", new(BetGameType.D1X, GamePeriod.H1) },
            {"1001297003", new(BetGameType.DX2, GamePeriod.H1) },
            
            //{"", new(BetGameType.WX1, GamePeriod.H2) },
            //{"", new(BetGameType.WXX, GamePeriod.H2) },
            //{"", new(BetGameType.WX2, GamePeriod.H2) },
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
            
            //{"", new(BetGameType.UNDER, GamePeriod.H1, thr: 1.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H1, thr: 2.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H1, thr: 3.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H1, thr: 4.5) },
            //{"", new(BetGameType.OVER, GamePeriod.H1, thr: 1.5) },
            //{"", new(BetGameType.OVER, GamePeriod.H1, thr: 2.5) },
            //{"", new(BetGameType.OVER, GamePeriod.H1, thr: 3.5) },
            //{"", new(BetGameType.OVER, GamePeriod.H1, thr: 4.5) },

            //{"", new(BetGameType.UNDER, GamePeriod.H2, thr: 1.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H2, thr: 2.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H2, thr: 3.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H2, thr: 4.5) },
            //{"", new(BetGameType.OVER, GamePeriod.H2, thr: 1.5) },
            //{"", new(BetGameType.OVER, GamePeriod.H2, thr: 2.5) },
            //{"", new(BetGameType.OVER, GamePeriod.H2, thr: 3.5) },
            //{"", new(BetGameType.OVER, GamePeriod.H2, thr: 4.5) },
        };
    }
}
