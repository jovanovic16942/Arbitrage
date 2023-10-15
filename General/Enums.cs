using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.General
{
    public enum BettingHouses
    {
        Mozzart,
        Meridian,
        MaxBet,
        AdmiralBet,
        SoccerBet,
        DefaultHouse // Dummy house
    }

    public enum BettingGames
    {
        // Konacan ishod
        _1,
        _X,
        _2,
        // Dupla sansa
        _1X,
        _X2,
        _12,
        // Ukupno golova
        _0_TO_2,
        _2_OR_MORE,
        _3_OR_MORE
    }

    public enum Sports
    {
        Fudbal,
        Basket
    }
}
