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
        public DateTime startTime;

        public List<Participant> teams = new();

        public List<HouseOdds> odds = new();
        public EventData(DateTime time, List<Participant> participants) 
        {
            startTime = time;
            teams = participants;
        }

        public Ticket GetBestOdd(BettingGames game)
        {
            BettingHouses bestHouse = BettingHouses.DefaultHouse;
            double bestValue = 0.0;
            double sumWithoutBest = 0.0;

            foreach (var houseOdds in odds)
            {
                var value = houseOdds.GetValue(game);
                // Add odd value to total sumWithoutBest
                sumWithoutBest += value;

                if (value > bestValue)
                {
                    sumWithoutBest -= value;
                    sumWithoutBest += bestValue;
                    bestValue = value;
                    bestHouse = houseOdds.House;
                }
            }

            double avg = sumWithoutBest / (odds.Count - 1);
            double risk = bestValue - avg;

            return new Ticket(bestHouse, game, bestValue, risk);
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
    }
}
