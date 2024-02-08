using Arbitrage.General;
using Arbitrage.Utils;
using NLog;

namespace Arbitrage.ArbitrageCalculator
{
    public class ArbitrageCalculator
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public ArbitrageCalculator() { }

        public void ProcessResults(List<EventData> events)
        {
            for (int i = 0; i < events.Count; i++)
            {
                ProcessEvent(events.ElementAt(i));
            }
        }

        public void ProcessEvent(EventData eventData)
        {
            HashSet<BetGame> games = new();

            foreach (var houseData in eventData.data)
            {
                foreach (var bg in houseData.betGames)
                {
                    if (games.Contains(bg) || games.Contains(bg.GetOppositeGame())) 
                    {
                        continue;
                    }
                    
                    games.Add(bg);
                }
            }

            foreach (var betGame in games)
            {
                var opGame = betGame.GetOppositeGame();
                var bestGame = eventData.GetBestOdd(betGame);
                var bestOppGame = eventData.GetBestOdd(opGame);

                if (bestGame == null)
                {
                    log.Debug("Unable to find best odd for bet game: " + betGame.ToString());
                    continue;
                }

                if (bestOppGame == null)
                {
                    log.Debug("Unable to find best odd for bet game: " + opGame.ToString());
                    continue;
                }

                // Calculate Arbitrage
                var arbScore = CalculateArbitrage(bestGame.Game.Value, bestOppGame.Game.Value);

                if (arbScore > 0.0)
                {
                    // Create Combination
                    // TODO risk assesment
                    List<Ticket> tickets = new()
                    {
                        new(bestGame.House,  bestGame.Game),
                        new(bestOppGame.House, bestOppGame.Game),
                    };

                    List<HouseMatchData> houses = new()
                    {
                        eventData.data.First(x => x.house == bestGame.House),
                        eventData.data.First(x => x.house == bestOppGame.House),
                    };

                    var comb = new Combination(tickets, arbScore, houses);
                    eventData.combinations.Add(comb);
                }
            }
        }

        public static double CalculateArbitrage(double v1, double v2, double investment = 1)
        {
            if (v1 == 0) return 0;
            if (v2 == 0) return 0;

            var p1 = 100 / v1;
            var p2 = 100 / v2;

            var sum_prob = (p1 + p2) / 100;

            return investment / sum_prob - investment;
        }
    }
}
