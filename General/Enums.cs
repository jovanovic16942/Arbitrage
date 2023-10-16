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

        // Oba tima daju gol
        _GG, // oba tima daju barem 1 gol na mecu
        _NG, // bar 1 tim ne postize gol na mecu
        _GG1, // oba tima daju barem 1 gol u prvom poluvremenu
        _NG1, // bar 1 tim ne postize gol u prvom poluvremenu
        _GG2, // oba tima daju barem 1 gol u drugom poluvremenu
        _NG2, // bar 1 tim ne postize gol u drugom poluvremenu

        //  Ukupno golova - TODO razmislilit da li ovako ili _X_TO_Y za svaki opseg (mozzart npr ima i [2-4]
        _OVER_0_5,      // 1+
        _UNDER_0_5,     // 0
        _OVER_1_5,      // 2+
        _UNDER_1_5,     // [0-1]
        _OVER_2_5,      // 3+
        _UNDER_2_5,     // [0-2]
                        // ...

        // TODO izbaci ova 3
        _0_TO_2,
        _2_OR_MORE,
        _3_OR_MORE,

        // Prvo poluvreme
        _1_I,
        _X_I,
        _2_I,
        _1X_I,
        _X2_I,
        _12_I,

        // Drugo poluvreme
        _1_II,
        _X_II,
        _2_II,
        _1X_II,
        _X2_II,
        _12_II,

        // TODO ukupno golova za prvo i drugo poluvreme (ukupno i za svaki tim posebno)


        DEFAULT // Dummy betting game
    }

    public enum Sports
    {
        Fudbal,
        Basket
    }
}
