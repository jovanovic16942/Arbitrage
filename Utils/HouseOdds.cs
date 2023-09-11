using Arbitrage.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Utils
{
    public class HouseOdds
    {
        public BettingHouses House;

        public Dictionary<BettingGames, double> Games;

        public HouseOdds(BettingHouses house) 
        {
            House = house;
            Games = new Dictionary<BettingGames, double>();
            foreach (BettingGames x in Enum.GetValues(typeof(BettingGames)))
            {
                Games.Add(x, 0);
            }
        }
    }


}


