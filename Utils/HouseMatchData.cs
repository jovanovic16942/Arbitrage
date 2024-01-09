using Arbitrage.EntityFramework.Models;
using Arbitrage.General;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Utils
{
    public class HouseMatchData
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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
                logger.Warn("BetGame already present: " + game.ToString());

                var oldValue = GetOddValue(game);
                if (oldValue != game.Value)
                {
                    logger.Error("Different values detected for bet game: " + game.ToString());
                }

                return;
            }

            betGames.Add(game);
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

        public string MatchDataString()
        {
            string eventString = "";
            eventString += "{" + team1 + "}";
            eventString += "{" + team2 + "}";
            eventString += "[" + house.ToString() + "]";
            return eventString;
        }
    }
}
