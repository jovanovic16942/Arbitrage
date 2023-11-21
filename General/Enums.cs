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
        MerkurXTip,
        PinnBet,
        SuperBet,
        BalkanBet,
        DefaultHouse // Dummy house
    }

    public enum BettingGames
    {
        // Match result
        _1,
        _X,
        _2,
        _1X,
        _X2,
        _12,

        // First half result
        _1_I,
        _X_I,
        _2_I,
        _1X_I,
        _X2_I,
        _12_I,

        // Second half result
        _1_II,
        _X_II,
        _2_II,
        _1X_II,
        _X2_II,
        _12_II,

        // Both teams score - At least one doesn't
        _GG,
        _NG,
        _GG_I,
        _NG_I,
        _NG_II,
        _GG_II,

        // _UG_X_Y = Total goals in [X, Y] inclusive
        _UG_0_1,
        _UG_0_2,
        _UG_0_3,
        _UG_0_4,
        _UG_0_5,
        _UG_0_6,
        _UG_1_2,
        _UG_1_3,
        _UG_1_4,
        _UG_1_5,
        _UG_1_6,
        _UG_2_3,
        _UG_2_4,
        _UG_2_5,
        _UG_2_6,
        _UG_3_4,
        _UG_3_5,
        _UG_3_6,
        _UG_4_5,
        _UG_4_6,
        _UG_5_6,

        // _UG_X_PLUS = Total goals >= X
        _UG_1_PLUS,
        _UG_2_PLUS,
        _UG_3_PLUS,
        _UG_4_PLUS,
        _UG_5_PLUS,
        _UG_6_PLUS,
        _UG_7_PLUS,

        // _UG_X = Total goals = X
        _UG_0,
        _UG_1,
        _UG_2,
        _UG_3,
        _UG_4,
        _UG_5,
        _UG_6,


        // TODO ukupno golova za prvo i drugo poluvreme (ukupno i za svaki tim posebno)


        DEFAULT // Dummy betting game
    }

    public enum Sports
    {
        Fudbal,
        Basket
    }
}
