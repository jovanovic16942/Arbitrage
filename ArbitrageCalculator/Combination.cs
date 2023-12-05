using Arbitrage.Utils;

namespace Arbitrage.ArbitrageCalculator
{
    public class Combination
    {
        public Combination(List<Ticket> tickets, double profit) 
        {
            this.tickets = tickets;
            this.profit = profit;
        }

        /// <summary>
        /// Calculate how much to stake on this combination based on available budget
        /// This modifies 
        /// <param name="budget">Available budget</param>
        /// </summary>
        public void CalculateStakes(double budget)
        {
            double winnings = budget * (1 + this.profit);

            tickets.ForEach(data => { data.SetStake(winnings); });
        }

        public override string ToString()
        {
            //var combStr = string.Format("{0} vs {1} @ {2}", teams[0].Name, teams[1].Name, startTime.ToString()) + Environment.NewLine;

            var combStr = string.Format("Profit: {0}", profit) + Environment.NewLine;

            foreach (var t in tickets)
            {
                combStr += t.ToString() + Environment.NewLine;
            }

            return combStr;
        }

        public List<Ticket> tickets;

        public double profit;
    }
}
