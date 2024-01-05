using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.General
{
    public enum BettingHouse
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
        StarBet,
        OktagonBet,
        BetOle,
        Olimp,
        Bet365,
        DefaultHouse, // Dummy house
    }

    public enum BetGameType
    {
        /// <summary>
        /// Team 1 wins the game (No chance of draw) 
        /// </summary>
        W1,
        /// <summary>
        /// Team 2 wins the game (No chance of draw)
        /// </summary>
        W2,

        /// <summary>
        /// Team 1 wins the game (draw returns invest)
        /// </summary>
        W1_X_0,

        /// <summary>
        /// Team 2 wins the game (draw returns invest)
        /// </summary>
        W2_X_0,

        /// <summary>
        /// Team 1 wins the match (draw is possible)
        /// </summary>
        WX1,
        /// <summary>
        /// The match ends in a draw
        /// </summary>
        WXX,
        /// <summary>
        /// Team 2 wins the match (draw is possible)
        /// </summary>
        WX2,

        /// <summary>
        /// Team 1 wins or the match ends in a draw
        /// </summary>
        D1X,
        /// <summary>
        /// Team 2 wins or the match ends in a draw
        /// </summary>
        DX2,
        /// <summary>
        /// Team 1 wins or Team 2 wins
        /// </summary>
        D12,

        /// <summary>
        /// Both teams score at least 1 goal
        /// </summary>
        GG,
        /// <summary>
        /// At least 1 team does not score a goal
        /// </summary>
        NG,

        /// <summary>
        /// Number of scored points is above the given threshold
        /// </summary>
        OVER,
        /// <summary>
        /// Number of scored points is below the given threshold
        /// </summary>
        UNDER,
    }

    /// <summary>
    /// Time period of the match
    /// </summary>
    public enum GamePeriod
    {
        /// <summary>
        /// Full duration of the match
        /// </summary>
        M,
        /// <summary>
        /// First half of the match
        /// </summary>
        H1,
        /// <summary>
        /// Second half of the match
        /// </summary>
        H2,
        /// <summary>
        /// Second half of the match, includes overtime
        /// </summary>
        H2O,
        /// <summary>
        /// First quarter of the match
        /// </summary>
        Q1,
        /// <summary>
        /// Second quarter of the match
        /// </summary>
        Q2,
        /// <summary>
        /// Third quarter of the match
        /// </summary>
        Q3,
        /// <summary>
        /// Final quarter of the match
        /// </summary>
        Q4,
        /// <summary>
        /// Final quarter of the match, includes overtime
        /// </summary>
        Q4O,
        /// <summary>
        /// Quarter with most scored points
        /// </summary>
        QB,

        NONE,
    }

    /// <summary>
    /// Team for OVER UNDER bet games
    /// </summary>
    public enum Team
    {
        /// <summary>
        /// Total for both teams
        /// </summary>
        T,
        /// <summary>
        /// Team 1
        /// </summary>
        T1,
        /// <summary>
        /// Team 2
        /// </summary>
        T2,
    }

    /// <summary>
    /// Deprecated
    /// </summary>
    public enum BettingGames
    {
        /*
         * 
         * FOOTBALL BETTING GAMES
         * 
         */

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


        /*
         * 
         * BASKETBALL BETTING GAMES
         * 
         */

        // Winner with time extensions
        B_W1,
        B_W2,
        
        // Results
        B_1,
        B_X,
        B_2,
        B_1X,
        B_12,
        B_X2,
        // I half
        B_1_I,
        B_X_I,
        B_2_I,
        B_1X_I,
        B_12_I,
        B_X2_I,
        // II half
        B_1_II,
        B_X_II,
        B_2_II,
        B_1X_II,
        B_12_II,
        B_X2_II,

        // OVER UNDER
        B_OVER,
        B_UNDER,
        // Half-time
        B_OVER_I,
        B_UNDER_I,
        B_OVER_II,
        B_UNDER_II,
        // Quarters (each of 4 and best)
        B_OVER_Q1,
        B_UNDER_Q1,
        B_OVER_Q2,
        B_UNDER_Q2,
        B_OVER_Q3,
        B_UNDER_Q3,
        B_OVER_Q4,
        B_UNDER_Q4,
        B_OVER_QB,
        B_UNDER_QB,
        // Teams (total and half-times)
        B_T1_OVER,
        B_T1_UNDER,
        B_T1_OVER_I,
        B_T1_UNDER_I,
        B_T1_OVER_II,
        B_T1_UNDER_II,
        B_T2_OVER,
        B_T2_UNDER,
        B_T2_OVER_I,
        B_T2_UNDER_I,
        B_T2_OVER_II,
        B_T2_UNDER_II,

        DEFAULT // Dummy betting game
    }

    public enum Sport
    {
        Football,
        Basketball,
    }
}
