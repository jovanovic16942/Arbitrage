
namespace Arbitrage.General
{
    public class ParserFactory
    {
        public static IParser GetParser(BettingHouse house)
        {
            string parserTypeStr = "Arbitrage.DataGetters." + house + "." + house + "Parser";
            Type type = Type.GetType(parserTypeStr)             ?? throw new Exception(string.Format("Unable to get type from string '{0}'", parserTypeStr));
            object instance = Activator.CreateInstance(type)    ?? throw new Exception(string.Format("Unable to create instance from type '{0}'", type.ToString()));
            return (IParser)instance;

            //switch (house)
            //{
            //    case BettingHouse.Mozzart:
            //        return new MozzartParser();
            //    case BettingHouse.AdmiralBet:
            //        return new AdmiralBetParser();
            //    case BettingHouse.Meridian:
            //        return new MeridianParser();
            //    case BettingHouse.MerkurXTip:
            //        return new MerkurXTipParser();
            //    case BettingHouse.MaxBet:
            //        return new MaxBetParser();
            //    case BettingHouse.SoccerBet:
            //        return new SoccerBetParser();
            //    case BettingHouse.PinnBet:
            //        return new PinnBetParser();
            //    case BettingHouse.Olimp:
            //        return new OlimpParser();
            //    case BettingHouse.OktagonBet:
            //        return new OktagonBetParser();
            //    case BettingHouse.SuperBet:
            //        return new SuperBetParser();
            //    case BettingHouse.StarBet:
            //        return new StarBetParser();
            //    case BettingHouse.Bet365:
            //        return new Bet365Parser();
            //    case BettingHouse.BetOle:
            //        return new BetOleParser();
            //    case BettingHouse.BalkanBet:
            //        return new BalkanBetParser();
            //    case BettingHouse.DefaultHouse:
            //    default:
            //        throw new InvalidDataException("Invalid argument: " + house.ToString());
            //}
        }
    }
}
