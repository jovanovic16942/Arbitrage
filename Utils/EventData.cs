using Arbitrage.ArbitrageCalculator;
using Arbitrage.General;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Utils
{
    public class EventData
    {
        public EventData(DateTime time, List<Participant> participants) 
        {
            startTime = time;
            teams = participants;
        }

        public override string ToString() 
        {
            string eventString = "<" + startTime.ToString() + ">";

            foreach (Participant participant in teams)
            {
                eventString += "{" + participant.Name + "}";
            }

            foreach (HouseOdds house in odds)
            {
                eventString += "[" + house.ToString() + "]";
            }

            return eventString;
        }

        public DateTime startTime;

        public List<Participant> teams = new List<Participant>();

        public List<HouseOdds> odds = new List<HouseOdds>();

        public OddData GetBestOdd(BettingGames game)
        {
            BettingHouses bestHouse = BettingHouses.DefaultHouse;
            double bestValue = 0.0;

            foreach (var houseOdds in odds)
            {
                var value = houseOdds.GetValue(game);
                
                if (value > bestValue)
                {
                    bestValue = value;
                    bestHouse = houseOdds.House;
                }
            }

            //var games = odds.Where(x => x.GetValue(game) Select(x => new )

            return new OddData(bestHouse, game, bestValue);
        }
    }
}
