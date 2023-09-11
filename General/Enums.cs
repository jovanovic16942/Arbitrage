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
        Meridian
    }

    public enum BettingGames
    {
        // Konacan ishod
        BG_1,
        BG_X,
        BG_2,
        // Dupla sansa
        BG_1X,
        BG_X2,
        BG_12,
        // Ukupno golova
        BG_0_TO_2,
        BG_2_OR_MORE,
        BG_3_OR_MORE
    }

    public enum Sports
    {
        Fudbal,
        Basket
    }
}
