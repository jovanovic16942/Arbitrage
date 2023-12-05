using Arbitrage.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.ArbitrageCalculator
{
    public class Ticket
    {
        public Ticket(BettingHouse house, BetGame game, double risk = 0.0)
        {
            this.house = house;
            this.game = game;
            this.risk = risk;
        }

        public void SetStake(double totalWinnings)
        {
            stake = totalWinnings / game.Value;
        }

        public override string ToString()
        {
            string sep = " ";
            var oddString = house.ToString() + sep + game.ToString() + sep + game.Value.ToString();
            if (stake > 0)
            {
                oddString += sep + stake.ToString();
            }

            return oddString;
        }

        public BettingHouse house;
        public BetGame game;
        public double stake = 0;
        public double risk;
    }
}
