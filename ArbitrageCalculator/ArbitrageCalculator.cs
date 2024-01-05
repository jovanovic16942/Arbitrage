using Arbitrage.General;
using Arbitrage.Utils;

namespace Arbitrage.ArbitrageCalculator
{
    public class ArbitrageCalculator
    {
        public static readonly List<List<BettingGames>> ArbitrageCombinations = new()
        {
            // Match result - overall
            //new List<BettingGames>() {BettingGames._1, BettingGames._X, BettingGames._2},
            new List<BettingGames>() {BettingGames._1X, BettingGames._2},
            new List<BettingGames>() {BettingGames._12, BettingGames._X},
            new List<BettingGames>() {BettingGames._1, BettingGames._X2},

            // First half result
            //new List<BettingGames>() {BettingGames._1_I, BettingGames._X_I, BettingGames._2_I},
            new List<BettingGames>() {BettingGames._1X_I, BettingGames._2_I},
            new List<BettingGames>() {BettingGames._12_I, BettingGames._X_I},
            new List<BettingGames>() {BettingGames._1_I, BettingGames._X2_I},

            // Second half result
            //new List<BettingGames>() {BettingGames._1_II, BettingGames._X_II, BettingGames._2_II},
            new List<BettingGames>() {BettingGames._1X_II, BettingGames._2_II},
            new List<BettingGames>() {BettingGames._12_II, BettingGames._X_II},
            new List<BettingGames>() {BettingGames._1_II, BettingGames._X2_II},

            // Goals
            new List<BettingGames>() {BettingGames._GG, BettingGames._NG},
            new List<BettingGames>() {BettingGames._GG_I, BettingGames._NG_I},
            new List<BettingGames>() {BettingGames._GG_II, BettingGames._NG_II},

            // Total goals - overall
            new List<BettingGames>() {BettingGames._UG_0, BettingGames._UG_1_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_1, BettingGames._UG_2_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_2, BettingGames._UG_3_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_3, BettingGames._UG_4_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_4, BettingGames._UG_5_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_5, BettingGames._UG_6_PLUS},

        };

        private List<Combination> _winningCombos = new();

        public ArbitrageCalculator() { }

        public void ProcessResults(List<EventData> events)
        {
            foreach (EventData eventData in events)
            {
                ProcessEvent(eventData);
            }
        }

        public void ProcessEvent(EventData eventData)
        {
            HashSet<BetGame> games = new();

            foreach (var houseData in eventData.data)
            {
                games.UnionWith(houseData.betGames);
            }

            foreach (var betGame in games)
            {
                var bestGame = eventData.GetBestOdd(betGame);
                var bestOppGame = eventData.GetBestOdd(betGame.GetOppositeGame());

                // TODO log and handle exceptions
                if (bestGame == null || bestOppGame == null) continue;

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

                    var comb = new Combination(tickets, arbScore);
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

        public static void ShowStakes(List<Combination> combos, double budget)
        {
            foreach(var combination in combos)
            {
                combination.CalculateStakes(budget);
                PrintCombination(combination);
            }
        }

        public static void PrintCombinations(List<Combination> combos, double profit_threshold = 0.0)
        {
            foreach (var combination in combos)
            {
                if (combination.profit > profit_threshold)
                {
                    PrintCombination(combination);
                }
            }
        }

        /// <summary>
        /// Get the list of combinations that can be staked on at the same time for maximum profit
        /// </summary>
        /// <returns>List of combinations</returns>
        public List<Combination> GetBetList()
        {
            List<Combination> bet_list = new();
            List<BettingHouse> houses = new();

            var potential_combos = new List<Combination>(_winningCombos);

            while(potential_combos.Any())
            {
                var best_bet = potential_combos.First();

                foreach (var ticket in best_bet.tickets)
                {
                    houses.Add(ticket.house);
                    potential_combos = potential_combos.Where(x => x.tickets.FirstOrDefault(y => y.house == ticket.house) == null).ToList();
                }

                bet_list.Add(best_bet);
            }

            return bet_list;
        }

        public static void PrintCombination(Combination winCombo)
        {
            Console.WriteLine("");
            Console.WriteLine("Winning combination found");
            Console.WriteLine(winCombo.ToString());
            Console.WriteLine("");
        }

        /// <summary>
        /// Check if combination contains invalid data (0.0)
        /// </summary>
        /// <param name="combination"></param>
        /// <returns>True if combination is valid</returns>
        public static bool ValidateCombination(List<Ticket> combination)
        {
            if (combination.Any(x => x.game.Value == 0.0))
            {
                //Console.WriteLine("Warning: Incorrect combination: 0.0 odd value present");
                //combination.ForEach(x => Console.WriteLine(x.ToString()));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deprecated and has errors 
        /// Calculate arbitrage score (profit) from given combination, with validity check
        /// </summary>
        /// <param name="combination">Combination off OddDatas</param>
        /// <returns>Maximum profit from given combination, 0.0 if invalid</returns>
        public static double ArbitrageScore(List<Ticket> combination)
        {
            if (!ValidateCombination(combination)) { return 0.0; }

            var arbScore = 1.0 - ProbabilitySum(combination.Select(x => x.game.Value).ToList());

            return arbScore;
        }

        /// <summary>
        /// Calculate sum of probabilities based on odds
        /// Sum the inverses of all odds
        /// </summary>
        /// <param name="odds"> Betting odds </param>
        /// <returns>Sum of probabilities</returns>
        public static double ProbabilitySum (List<double> odds)
        {
            return odds.Aggregate(0.0, (sum, value) => sum + (value != 0 ? 1.0 / value : 0));
        }
    }
}
