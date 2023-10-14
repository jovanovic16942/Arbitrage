using Arbitrage.Utils;
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

            var check1X2 = new List<double>() { best1.value, bestX.value, best2.value };
            var check1_X2 = new List<double>() { best1.value, bestX2.value };
            var check1X_2 = new List<double>() { best1X.value, best2.value };
            var check12_X = new List<double>() { best12.value, bestX.value };

            

            var res = ProbabilitySum(check1X2);

            if (res < 1.0 && !check1X2.Contains(0.0))
            {
                Console.WriteLine("NASO SAM NASO SAM HAJHAJAJHAHAHA");
                Console.WriteLine("1X2 score " + res);
                Console.WriteLine(string.Format("{0} - {1} | {2} ({3}) : {4} ({5}) : {6} ({7}) ",
                    eventData.teams[0].Name, eventData.teams[1].Name,
                    best1.value, best1.house.ToString(),
                    bestX.value, bestX.house.ToString(),
                    best2.value, best2.house.ToString()));
            }


            res = ProbabilitySum(check1_X2);
            if (res < 1.0 && !check1_X2.Contains(0.0))
            {
                Console.WriteLine("NASO SAM NASO SAM HAJHAJAJHAHAHA");
                Console.WriteLine("1_X2 score " + res);
                Console.WriteLine(string.Format("{0} - {1} | {2} ({3}) : {4} ({5}) ",
                    eventData.teams[0].Name, eventData.teams[1].Name,
                    best1.value, best1.house.ToString(),
                    bestX2.value, bestX2.house.ToString()));
            }


            res = ProbabilitySum(check1X_2);
            if (res < 1.0 && !check1X_2.Contains(0.0))
            {
                Console.WriteLine("NASO SAM NASO SAM HAJHAJAJHAHAHA");
                Console.WriteLine("1X_2 score " + res);
                Console.WriteLine(string.Format("{0} - {1} | {2} ({3}) : {4} ({5}) ",
                    eventData.teams[0].Name, eventData.teams[1].Name,
                    best1X.value, best1X.house.ToString(),
                    best2.value, best2.house.ToString()));
            }

            res = ProbabilitySum(check12_X);
            if (res < 1.0 && !check12_X.Contains(0.0))
            {
                Console.WriteLine("NASO SAM NASO SAM HAJHAJAJHAHAHA");
                Console.WriteLine("12_X score " + res);
                Console.WriteLine(string.Format("{0} - {1} | {2} ({3}) : {4} ({5}) ",
                    eventData.teams[0].Name, eventData.teams[1].Name,
                    best12.value, best12.house.ToString(),
                    bestX.value, bestX.house.ToString()));
            }

        }

        /// <summary>
        /// Calculate sum of probabilities based on odds
        /// </summary>
        /// <param name="odds"> Betting odds </param>
        /// <returns>Sum of probabilities</returns>
        public double ProbabilitySum (List<double> odds)
        {
            return odds.Aggregate(0.0, (sum, value) => sum + (value != 0 ? 1.0 / value : 0));
        }
    }
}
