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
        public Ticket(BettingHouses house, BettingGames game, double value)
        {
            this.house = house;
            this.game = game;
            this.value = value;
        }

        public void SetStake(double totalWinnings)
        {
            stake = totalWinnings / value;
        }

        public override string ToString()
        {
            string sep = " ";
            var oddString = house.ToString() + sep + game.ToString() + sep + value.ToString();
            if (value > 0)
            {
                oddString += sep + stake.ToString();
            }

            return oddString;
        }

        public BettingHouses house;
        public BettingGames game;
        public double value;
        public double stake = 0;
    }
}
