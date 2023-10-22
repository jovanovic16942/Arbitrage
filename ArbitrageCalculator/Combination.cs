using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.ArbitrageCalculator
{
    public class Combination
    {
        public Combination(List<OddData> oddDatas, double profit, List<Participant> teams, DateTime starTime) 
        {
            this.oddData = oddDatas;
            this.Teams = teams;
            this.StartTime = starTime;
            this.Profit = profit;
        }

        public List<OddData> oddData;

        public double Profit;

        public List<Participant> Teams;

        public DateTime StartTime;
    }
}
