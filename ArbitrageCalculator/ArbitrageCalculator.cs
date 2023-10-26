using Arbitrage.General;
using Arbitrage.Utils;

namespace Arbitrage.ArbitrageCalculator
{
    public class ArbitrageCalculator
    {

        public static readonly List<List<BettingGames>> ArbitrageCombinations = new()
        {
            // Match result - overall
            new List<BettingGames>() {BettingGames._1, BettingGames._X, BettingGames._2},
            new List<BettingGames>() {BettingGames._1X, BettingGames._2},
            new List<BettingGames>() {BettingGames._12, BettingGames._X},
            new List<BettingGames>() {BettingGames._1, BettingGames._X2},

            // First half result
            new List<BettingGames>() {BettingGames._1_I, BettingGames._X_I, BettingGames._2_I},
            new List<BettingGames>() {BettingGames._1X_I, BettingGames._2_I},
            new List<BettingGames>() {BettingGames._12_I, BettingGames._X_I},
            new List<BettingGames>() {BettingGames._1_I, BettingGames._X2_I},

            // Second half result
            new List<BettingGames>() {BettingGames._1_II, BettingGames._X_II, BettingGames._2_II},
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

            new List<BettingGames>() {BettingGames._UG_0_1, BettingGames._UG_2_3, BettingGames._UG_4_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_1, BettingGames._UG_2_4, BettingGames._UG_5_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_1, BettingGames._UG_2_5, BettingGames._UG_6_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_2, BettingGames._UG_3_4, BettingGames._UG_5_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_2, BettingGames._UG_3_5, BettingGames._UG_6_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_3, BettingGames._UG_4_5, BettingGames._UG_6_PLUS},

            new List<BettingGames>() {BettingGames._UG_0, BettingGames._UG_1, BettingGames._UG_2_PLUS},
            new List<BettingGames>() {BettingGames._UG_0, BettingGames._UG_1_2, BettingGames._UG_3_PLUS},
            new List<BettingGames>() {BettingGames._UG_0, BettingGames._UG_1_3, BettingGames._UG_4_PLUS},
            new List<BettingGames>() {BettingGames._UG_0, BettingGames._UG_1_4, BettingGames._UG_5_PLUS},
            new List<BettingGames>() {BettingGames._UG_0, BettingGames._UG_1_5, BettingGames._UG_6_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_1, BettingGames._UG_2, BettingGames._UG_3_PLUS},

            new List<BettingGames>() {BettingGames._UG_0_2, BettingGames._UG_3, BettingGames._UG_4_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_3, BettingGames._UG_4, BettingGames._UG_5_PLUS},
            new List<BettingGames>() {BettingGames._UG_0_4, BettingGames._UG_5, BettingGames._UG_6_PLUS},

            // TODO - Total goals - first half
            // TODO - Total goals - second half
            // TODO - Total goals - home team
            // TODO - Total goals - away team
        };

        private List<Combination> _winningCombos = new();

        public ArbitrageCalculator() { }

        public List<Combination> GetResults(List<EventData> events)
        {
            foreach (EventData eventData in events)
            {
                ProcessEvent(eventData);
            }

            _winningCombos = _winningCombos.OrderByDescending(x => x.profit).ToList();

            return _winningCombos;
        }

        public void ProcessEvent(EventData eventData)
        {
            // Prepare combinations
            var combinations = new List<List<Ticket>>();

            foreach (var betGameComb in ArbitrageCombinations)
            {
                combinations.Add(betGameComb.Select(betGame => eventData.GetBestOdd(betGame)).ToList());
            }

            // Calculate score for each combo and save those with positive score
            foreach (var combination in combinations)
            {
                var arbScore = ArbitrageScore(combination);

                if (arbScore > 0.0)
                {
                    _winningCombos.Add(new Combination(combination, arbScore, eventData.teams, eventData.startTime));
                }
            }
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
            List<BettingHouses> houses = new();

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
            if (combination.Any(x => x.value == 0.0))
            {
                //Console.WriteLine("Warning: Incorrect combination: 0.0 odd value present");
                //combination.ForEach(x => Console.WriteLine(x.ToString()));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculate arbitrage score (profit) from given combination, with validity check
        /// </summary>
        /// <param name="combination">Combination off OddDatas</param>
        /// <returns>Maximum profit from given combination, 0.0 if invalid</returns>
        public static double ArbitrageScore(List<Ticket> combination)
        {
            if (!ValidateCombination(combination)) { return 0.0; }

            var arbScore = 1.0 - ProbabilitySum(combination.Select(x => x.value).ToList());

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
