using Arbitrage.General;
using Arbitrage.Utils;
using NLog;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace Arbitrage.DataGetters.Mozzart
{
    public class MozzartParser : Parser
    {
        private readonly MozzartGetter _getter = new();

        private static Logger logger = LogManager.GetCurrentClassLogger();
        public MozzartParser() : base(BettingHouse.Mozzart) 
        {
            logger.Info("MozzartParser created!");
        }

        protected override void UpdateData()
        {
            var respMatches = _getter.GetMatches();

            //removed matches that expect n+ betGames in one ticket
            respMatches.Matches = respMatches.Matches.Where(x => x.SpecialType == 0).ToList();

            UpdateMatches(respMatches);

            var matchIDs = _data.GetMatches().Select(x => x.matchId).ToList();
            var respOdds = _getter.GetOdds(matchIDs);
            UpdateOdds(respOdds);
        }

        private void UpdateMatches(JsonMatchResponse resp)
        {
            foreach (var match in resp.Matches)
            {
                try
                {
                    var participants = match.Participants.ToList();
                    var p1 = participants[0];
                    var p2 = participants[1];

                    var participant1 = new Participant(p1.Id, p1.Name, p1.ShortName, p1.Description);
                    var participant2 = new Participant(p2.Id, p2.Name, p2.ShortName, p2.Description);

                    DateTime startTime = DateTimeConverter.DateTimeFromLong(match.StartTime, 1); // TODO SUUUSSSSSS  Promena sata daylight savings bblabla

                    _data.Insert(new Utils.Match(match.Id, startTime, participant1, participant2));
                }
                catch
                {
                    // TODO Exception is thrown for matches with 1 participant
                }
            }

        }

        private void UpdateOdds(List<JsonRoot> resp)
        {
            foreach (var matchOdds in resp)
            {
                var matchId = matchOdds.id;
                var kodds = matchOdds.kodds;

                foreach (var kvp in kodds)
                {
                    string subGameID = kvp.Key; // Mozzart.com subGameID - trenutno subgamesIds u MozzartGetter
                    JsonKodds kodd = kvp.Value;

                    if (kodd == null) continue;

                    double betValue = double.Parse(kodd.value); // Kvota

                    BettingGames game;
                    if (betGameFromID.TryGetValue(subGameID.Trim(), out game))
                    {
                        try
                        {
                            _data.UpdateMatchSubgame(matchId, game, betValue);
                        } 
                        catch (ArgumentException e)
                        {
                            Console.WriteLine("Exception thrown in UpdateMatchSubgame (MozzartParser):");
                            Console.WriteLine("BettingGame: " + game.ToString());
                            Console.WriteLine("Incoming value: " + betValue);
                            Console.WriteLine("Previous value: " + _data.GetSubgameValue(matchId, game));
                            Console.WriteLine(e.ParamName);
                            Console.WriteLine(e.Message);
                        }
                    }

                }
            }
        }

        static readonly Dictionary<string, BettingGames> betGameFromID = new()
        {
            {"1001130001", BettingGames._GG },
            {"1001130002", BettingGames._NG },
            {"1001130004", BettingGames._GG_I },
            {"1001130010", BettingGames._NG_I },
            {"1001130005", BettingGames._GG_II },
            {"1001130011", BettingGames._NG_II },
            {"1001001001", BettingGames._1 },
            {"1001001002", BettingGames._X },
            {"1001001003", BettingGames._2 },
            {"1001002002", BettingGames._12 },
            {"1001002001", BettingGames._1X },
            {"1001002003", BettingGames._X2 },
            {"1001297002", BettingGames._12_I },
            {"1001297001", BettingGames._1X_I },
            {"1001297003", BettingGames._X2_I },
            {"1001003001", BettingGames._UG_0_1 },
            {"1001003002", BettingGames._UG_0_2 },
            {"1001003013", BettingGames._UG_0_3 },
            {"1001003026", BettingGames._UG_0_4 },
            {"1001003003", BettingGames._UG_2_3 },
            {"1001003012", BettingGames._UG_2_PLUS },
            {"1001003004", BettingGames._UG_3_PLUS },
            {"1001003005", BettingGames._UG_4_PLUS },
            {"1001003007", BettingGames._UG_5_PLUS },
        };
    }
}
