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
        private List<Combination> _winningCombos = new();

        public ArbitrageCalculator() { }

        public List<Combination> GetResults(List<EventData> events)
        {
            foreach (EventData eventData in events)
            {
                ProcessEvent(eventData);
            }

            _winningCombos = _winningCombos.OrderByDescending(x => x.MaxProfit).ToList();

            return _winningCombos;
        }

        public void ProcessEvent(EventData eventData)
        {
            // Get the maximum values for all relevant odds
            var best1 = eventData.GetBestOdd(General.BettingGames._1);
            var bestX = eventData.GetBestOdd(General.BettingGames._X);
            var best2 = eventData.GetBestOdd(General.BettingGames._2);

            var best1X = eventData.GetBestOdd(General.BettingGames._1X);
            var bestX2 = eventData.GetBestOdd(General.BettingGames._X2);
            var best12 = eventData.GetBestOdd(General.BettingGames._12);

            var bestUnder2 = eventData.GetBestOdd(General.BettingGames._0_TO_2);
            var bestOver2 = eventData.GetBestOdd(General.BettingGames._2_OR_MORE);

            // Define potential arbitrage combinations
            var combinations = new List<List<OddData>>()
            {
                new List<OddData>() { best1, bestX, best2 }, // 1 X 2
                new List<OddData>() { best1, bestX2 }, // 1 X2
                new List<OddData>() { best1X, best2 }, // 1X 2
                new List<OddData>() { best12, bestX }, // 12 X
                new List<OddData>() { bestUnder2, bestOver2 }, // <2 >=2 // TODO check this for every source
            };

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
                if (combination.MaxProfit > profit_threshold)
                {
                    PrintCombination(combination);
                }
            }
        }

        public static void PrintCombination(Combination winCombo)
        {
            Console.WriteLine("");
            Console.WriteLine("Winning combination found");
            Console.WriteLine("Maximum profit: " + winCombo.MaxProfit.ToString());
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
