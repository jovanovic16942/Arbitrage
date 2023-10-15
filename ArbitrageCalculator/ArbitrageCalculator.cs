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
        public ArbitrageCalculator() { }

        public void GetResults(List<EventData> events)
        {
            foreach (EventData eventData in events)
            {
                ProcessEvent(eventData);
            }
        }

        public void ProcessEvent(EventData eventData)
        {
            if (eventData.teams[0].Name == "USA")
            {

            }

            var best1 = eventData.GetBestOdd(General.BettingGames._1);
            var bestX = eventData.GetBestOdd(General.BettingGames._X);
            var best2 = eventData.GetBestOdd(General.BettingGames._2);

            var best1X = eventData.GetBestOdd(General.BettingGames._1X);
            var bestX2 = eventData.GetBestOdd(General.BettingGames._X2);
            var best12 = eventData.GetBestOdd(General.BettingGames._12);

            //var check1X2 = new List<double>() { best1.value, bestX.value, best2.value };
            //var check1_X2 = new List<double>() { best1.value, bestX2.value };
            //var check1X_2 = new List<double>() { best1X.value, best2.value };
            //var check12_X = new List<double>() { best12.value, bestX.value };


            var combinations = new List<List<OddData>>()
            {
                new List<OddData>() { best1, bestX, best2 }, // 1 X 2
                new List<OddData>() { best1, bestX2 }, // 1 X2
                new List<OddData>() { best1X, best2 }, // 1X 2
                new List<OddData>() { best12, bestX }, // 12 X
            };

            foreach (var combination in combinations)
            {
                var arbScore = ArbitrageScore(combination);

                if (arbScore > 0.0)
                {
                    PrintSuccess(eventData, combination, arbScore);
                }
            }

            //var res = ProbabilitySum(check1X2);

            //if (res < 1.0 && !check1X2.Contains(0.0))
            //{
            //    Console.WriteLine("NASO SAM NASO SAM HAJHAJAJHAHAHA");
            //    Console.WriteLine("1X2 score " + res);
            //    Console.WriteLine(string.Format("{0} - {1} | {2} ({3}) : {4} ({5}) : {6} ({7}) ",
            //        eventData.teams[0].Name, eventData.teams[1].Name,
            //        best1.value, best1.house.ToString(),
            //        bestX.value, bestX.house.ToString(),
            //        best2.value, best2.house.ToString()));
            //}


            //res = ProbabilitySum(check1_X2);
            //if (res < 1.0 && !check1_X2.Contains(0.0))
            //{
            //    Console.WriteLine("NASO SAM NASO SAM HAJHAJAJHAHAHA");
            //    Console.WriteLine("1_X2 score " + res);
            //    Console.WriteLine(string.Format("{0} - {1} | {2} ({3}) : {4} ({5}) ",
            //        eventData.teams[0].Name, eventData.teams[1].Name,
            //        best1.value, best1.house.ToString(),
            //        bestX2.value, bestX2.house.ToString()));
            //}


            //res = ProbabilitySum(check1X_2);
            //if (res < 1.0 && !check1X_2.Contains(0.0))
            //{
            //    Console.WriteLine("NASO SAM NASO SAM HAJHAJAJHAHAHA");
            //    Console.WriteLine("1X_2 score " + res);
            //    Console.WriteLine(string.Format("{0} - {1} | {2} ({3}) : {4} ({5}) ",
            //        eventData.teams[0].Name, eventData.teams[1].Name,
            //        best1X.value, best1X.house.ToString(),
            //        best2.value, best2.house.ToString()));
            //}

            //res = ProbabilitySum(check12_X);
            //if (res < 1.0 && !check12_X.Contains(0.0))
            //{
            //    Console.WriteLine("NASO SAM NASO SAM HAJHAJAJHAHAHA");
            //    Console.WriteLine("12_X score " + res);
            //    Console.WriteLine(string.Format("{0} - {1} | {2} ({3}) : {4} ({5}) ",
            //        eventData.teams[0].Name, eventData.teams[1].Name,
            //        best12.value, best12.house.ToString(),
            //        bestX.value, bestX.house.ToString()));
            //}

        }

        //public static void Check1X2(EventData eventData)
        //{
        //    var combination = new List<OddData>()
        //    {
        //        eventData.GetBestOdd(General.BettingGames._1),
        //        eventData.GetBestOdd(General.BettingGames._X),
        //        eventData.GetBestOdd(General.BettingGames._2)
        //    };

        //    double profit = ArbitrageScore(combination);

        //    if (profit > 0.0)
        //    {
        //        PrintSuccess(eventData, combination, profit);
        //    }
        //}


        public static void PrintSuccess(EventData eventData, List<OddData> combination, double score)
        {
            Console.WriteLine("NASO SAM NASO SAM HAJHAJAJHAHAHA");
            Console.WriteLine("Maximum profit: " + score.ToString());
            Console.WriteLine(string.Format("{0} vs {1} @ {2}", eventData.teams[0].Name, eventData.teams[1].Name, eventData.startTime.ToString()));
            combination.ForEach(x => Console.WriteLine(x.ToString()));
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
                Console.WriteLine("Warning: Incorrect combination: 0.0 odd value present");
                combination.ForEach(x => Console.WriteLine(x.ToString()));
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
