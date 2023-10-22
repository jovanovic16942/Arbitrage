using Arbitrage.General;
using Arbitrage.Utils;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            _winningCombos = _winningCombos.OrderByDescending(x => x.Profit).ToList();

            return _winningCombos;
        }

        public void ProcessEvent(EventData eventData)
        {
            // Prepare combinations
            var combinations = new List<List<OddData>>();

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

        public static void PrintAllCombinations(List<Combination> winningCombos, double profit_threshold = 0.0)
        {
            foreach (var combination in winningCombos)
            {
                if (combination.Profit > profit_threshold)
                {
                    PrintCombination(combination);
                }
            }
        }

        public static void PrintCombination(Combination winCombo)
        {
            Console.WriteLine("");
            Console.WriteLine("Winning combination found");
            Console.WriteLine("Maximum profit: " + winCombo.Profit.ToString());
            Console.WriteLine(string.Format("{0} vs {1} @ {2}", winCombo.Teams[0].Name, winCombo.Teams[1].Name, winCombo.StartTime.ToString()));
            winCombo.oddData.ForEach(x => Console.WriteLine(x.ToString()));
            Console.WriteLine("");
        }

        /// <summary>
        /// Check if combination contains invalid data (0.0)
        /// </summary>
        /// <param name="combination"></param>
        /// <returns>True if combination is valid</returns>
        public static bool ValidateCombination(List<OddData> combination)
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
        public static double ArbitrageScore(List<OddData> combination)
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
