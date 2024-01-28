using Arbitrage.General;
using NLog;

namespace Arbitrage.Utils
{
    public class HouseMatchData
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public BettingHouse house;
        public Sport sport;

        public DateTime startTime;

        public HashSet<BetGame> betGames;

        // Team names
        public string team1;
        public string team2;

        public HouseMatchData(BettingHouse house, Sport sport, DateTime startTime, string t1, string t2)
        {
            this.house = house;
            this.sport = sport;
            this.startTime = startTime;
            team1 = t1.ToUpper();
            team2 = t2.ToUpper();
            betGames = new();
        }

        public void AddBetGame(BetGame game)
        {
            if (betGames.Contains(game))
            {
                log.Warn("BetGame: " + game.ToString() + " already present in match: " + ToString());

                var oldValue = GetOddValue(game);
                if (oldValue != game.Value)
                {
                    log.Error(
                        string.Format("Different values detected for: {0} Original value: {1} - New value: {2} ",
                            game.ToString(), oldValue, game.Value)
                        );
                }

                return;
            }

            betGames.Add(game);
        }
        public void UpdateBetGame(BetGame game)
        {
            var oldGame = GetBetGame(game);

            if (oldGame == null)
            {
                log.Warn("Bet game not present: " + game);
                AddBetGame(game);
            } else
            {
                oldGame.Value = game.Value;
            }
        }

        public BetGame? GetBetGame(BetGame g)
        {
            if (betGames.TryGetValue(g, out BetGame game))
            {
                return game;
            }

            return null;
        }

        public double GetOddValue(BetGame game)
        {
            if (!betGames.TryGetValue(game, out BetGame? tmp))
            {
                // log
                return 0.0;
            }

            return tmp.Value;
        }

        public override string ToString()
        {
            string eventString = "";
            eventString += "{" + team1 + "}";
            eventString += "{" + team2 + "}";
            eventString += "[" + house.ToString() + "]";
            return eventString;
        }
    }
}
