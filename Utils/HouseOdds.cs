using Arbitrage.General;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Utils
{

    /// <summary>
    /// DEPRECATED
    /// </summary>
    public class HouseOdds
    {
        public BettingHouse House;

        public Dictionary<BettingGames, double> Games;

        public HouseOdds(BettingHouse house) 
        {
            House = house;
            Games = new Dictionary<BettingGames, double>();
            foreach (BettingGames x in Enum.GetValues(typeof(BettingGames)))
            {
                Games.Add(x, 0);
            }
        }

        public void AddOdds(BettingGames bettingGame, double value)
        {
            Games[bettingGame] = value;
        }

        public override string ToString()
        {
            string houseStr = House.ToString();

            // TODO add bet prices to houseStr

            return houseStr;
        }

        public double GetValue(BettingGames bettingGame)
        {
            return Games[bettingGame];
        }
    }


}


