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
            // TODO this can be paralel
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
                try
                {
                    ProcessEventBetGameAll(eventData, betGame);
                }
                catch (Exception e)
                {
                    log.Error("Exception while processing event: {0} - Exception while processing bet game: {1} - Exception: {2}", eventData, betGame, e);
                }
            }

            // Sort the results
            eventData.combinations.Sort((x, y) => y.profit.CompareTo(x.profit));
        }

        public void ProcessEventBetGameAll(EventData eventData, BetGame betGame)
        {
            var opGame = betGame.GetOppositeGame();
            var bestGames = eventData.GetSortedOdds(betGame);
            var bestOppGames = eventData.GetSortedOdds(opGame);

            if (bestGames.Count == 0)
            {
                log.Debug("Unable to find best odd for bet game: " + betGame.ToString());
                return;
            }

            if (bestOppGames.Count == 0)
            {
                log.Debug("Unable to find best odd for bet game: " + opGame.ToString());
                return;
            }

            foreach (var bestGame in bestGames)
            {
                bool done = true;
                foreach (var bestOpGame in bestOppGames)
                {
                    if (!ProcessCombination(eventData, bestGame, bestOpGame))
                    {
                        break;
                    }
                    done = false;
                }

                if (done) break;
            }
        }

        public void ProcessEventBetGame(EventData eventData, BetGame betGame)
        {
            var opGame = betGame.GetOppositeGame();
            var bestGame = eventData.GetBestOdd(betGame);
            var bestOppGame = eventData.GetBestOdd(opGame);

            if (bestGame == null)
            {
                log.Debug("Unable to find best odd for bet game: " + betGame.ToString());
                return;
            }

            if (bestOppGame == null)
            {
                log.Debug("Unable to find best odd for bet game: " + opGame.ToString());
                return;
            }

            ProcessCombination(eventData, bestGame, bestOppGame);
        }


        /// <summary>
        /// Calculate potential profit and create Combination object
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="houseGame"></param>
        /// <param name="houseOpGame"></param>
        /// <returns>True if combination was created, False otherwise </returns>
        public bool ProcessCombination(EventData eventData, HouseBetGame houseGame, HouseBetGame houseOpGame)
        {
            // Calculate Arbitrage
            var arbScore = CalculateArbitrage(houseGame.Game.Value, houseOpGame.Game.Value);

            if (arbScore > 0.0)
            {
                // Create Combination
                // TODO risk assesment
                List<Ticket> tickets = new()
                {
                    new(houseGame.House,  houseGame.Game),
                    new(houseOpGame.House, houseOpGame.Game),
                };

                List<HouseMatchData> houses = new()
                {
                    eventData.data.First(x => x.house == houseGame.House),
                    eventData.data.First(x => x.house == houseOpGame.House),
                };

                var comb = new Combination(tickets, arbScore, houses);
                eventData.combinations.Add(comb);
                return true;
            }

            return false;
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
