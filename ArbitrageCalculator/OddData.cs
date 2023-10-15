﻿using Arbitrage.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.ArbitrageCalculator
{
    public class OddData
    {
        public OddData(BettingHouses house, BettingGames game, double value)
        {
            this.house = house;
            this.game = game;
            this.value = value;
        }

        public override string ToString()
        {
            string sep = " ";
            return house.ToString() + sep + game.ToString() + sep + value.ToString();
        }

        public BettingHouses house;
        public BettingGames game;
        public double value;
    }
}