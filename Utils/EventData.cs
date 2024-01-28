using Arbitrage.ArbitrageCalculator;
using Arbitrage.General;

namespace Arbitrage.Utils
{
    public record HouseBetGame(BettingHouse House, BetGame Game);

    public class EventData
    {
        public Sport sport;
        public DateTime startTime;

        /// <summary>
        /// Data from each betting house. Contains 1 element before matching
        /// </summary>
        public List<HouseMatchData> data = new(); 

        /// <summary>
        /// Betting combinations obtained from arbitrage calculations
        /// </summary>
        public List<Combination> combinations = new();

        public EventData(HouseMatchData houseData)
        {
            sport = houseData.sport;
            startTime = houseData.startTime;
            data.Add(houseData);
        }

        public List<HouseBetGame> GetSortedOdds(BetGame game)
        {
            var bgList = new List<HouseBetGame>();

            foreach (var houseData in data)
            {
                var houseGame = houseData.GetBetGame(game);
                if (houseGame == null) continue;
                bgList.Add(new(houseData.house, houseGame));
            }

            return bgList.OrderByDescending(x => x.Game.Value).ToList();
        }

        public HouseBetGame? GetBestOdd(BetGame game)
        {
            var sortedOdds = GetSortedOdds(game);
            return sortedOdds.FirstOrDefault();
        }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        public List<Participant> teams = new();
        /// <summary>
        /// DEPRECATED
        /// </summary>
        public List<HouseOdds> odds = new();

        /// <summary>
        /// Deprecated constructor
        /// </summary>
        /// <param name="time"></param>
        /// <param name="participants"></param>
        public EventData(DateTime time, List<Participant> participants)
        {
            startTime = time;
            teams = participants;
        }

        //public Ticket GetBestOdd(BettingGames game)
        //{
        //    BettingHouse bestHouse = BettingHouse.DefaultHouse;
        //    double bestValue = 0.0;
        //    double sumWithoutBest = 0.0;

        //    foreach (var houseOdds in odds)
        //    {
        //        var value = houseOdds.GetValue(game);
        //        // Add odd value to total sumWithoutBest
        //        sumWithoutBest += value;

        //        if (value > bestValue)
        //        {
        //            sumWithoutBest -= value;
        //            sumWithoutBest += bestValue;
        //            bestValue = value;
        //            bestHouse = houseOdds.House;
        //        }
        //    }

        //    double avg = sumWithoutBest / (odds.Count - 1);
        //    double risk = bestValue - avg;

        //    return new Ticket(bestHouse, game, bestValue, risk);
        //}

        public override string ToString()
        {
            string eventString = "<" + startTime.ToString() + ">";

            foreach (var houseData in data)
            {
                eventString += houseData + System.Environment.NewLine;
            }

            return eventString;
        }
    }
}
