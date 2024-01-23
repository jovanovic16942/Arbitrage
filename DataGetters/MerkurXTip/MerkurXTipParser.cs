using Arbitrage.DataGetters.MaxBet;
using Arbitrage.General;
using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.MerkurXTip
{
    public class MerkurXTipParser : Parser
    {
        private readonly MerkurXTipGetter _getter = new();

        public MerkurXTipParser() : base(BettingHouse.MerkurXTip) { }

        private void ParseMatch(JsonMatch jsonMatch)
        {
            var oddsResp = _getter.GetMatch(jsonMatch.id); // TODO handle exceptions

            DateTime startTime = DateTimeConverter.DateTimeFromLong(jsonMatch.kickOffTime, 1); // SSSSSSUUUUUSSSSSSSSS

            HouseMatchData hmd = new(House, Sport.Football, startTime, jsonMatch.home.Trim(), jsonMatch.away.Trim());

            // Add odds
            foreach (var (oddID, oddValue) in oddsResp.odds)
            {
                if (betGameFromID.Keys.Contains(oddID))
                {
                    BetGame game = betGameFromID[oddID].Clone();
                    game.Value = oddValue;
                    hmd.AddBetGame(game);
                }
            }

            _parsedData.Add(hmd);
        }

        protected override void ParseFootball()
        {
            var resp = _getter.GetMatches();

            foreach (var jsonMatch in resp.esMatches)
            {
                ParseMatch(jsonMatch);
            }
        }

        /// <summary>
        /// Map betGames from json response bpc to BetGame
        /// </summary>
        static readonly Dictionary<int, BetGame> betGameFromID = new()
        {
            {272, new(BetGameType.GG) },
            {273, new(BetGameType.NG) },
            {291, new(BetGameType.GG, GamePeriod.H1) },
            {292, new(BetGameType.NG, GamePeriod.H1) },
            {299, new(BetGameType.GG, GamePeriod.H2) },
            {300, new(BetGameType.NG, GamePeriod.H2) },

            {1, new(BetGameType.WX1) },
            {2, new(BetGameType.WXX) },
            {3, new(BetGameType.WX2) },
            {7, new(BetGameType.D1X) },
            {8, new(BetGameType.D12) },
            {9, new(BetGameType.DX2) },

            {4, new(BetGameType.WX1, GamePeriod.H1) },
            {5, new(BetGameType.WXX, GamePeriod.H1) },
            {6, new(BetGameType.WX2, GamePeriod.H1) },
            {397, new(BetGameType.D1X, GamePeriod.H1) },
            {398, new(BetGameType.D12, GamePeriod.H1) },
            {399, new(BetGameType.DX2, GamePeriod.H1) },

            {235, new(BetGameType.WX1, GamePeriod.H2) },
            {236, new(BetGameType.WXX, GamePeriod.H2) },
            {237, new(BetGameType.WX2, GamePeriod.H2) },
            {608, new(BetGameType.D1X, GamePeriod.H2) },
            {609, new(BetGameType.D12, GamePeriod.H2) },
            {610, new(BetGameType.DX2, GamePeriod.H2) },

            {21, new(BetGameType.UNDER, thr: 1.5) },
            {22, new(BetGameType.UNDER, thr: 2.5) },
            {219, new(BetGameType.UNDER, thr: 3.5) },
            {453, new(BetGameType.UNDER, thr: 4.5) },
            {242, new(BetGameType.OVER, thr: 1.5) },
            {24, new(BetGameType.OVER, thr: 2.5) },
            {25, new(BetGameType.OVER, thr: 3.5) },
            {27, new(BetGameType.OVER, thr: 4.5) },

            {267, new(BetGameType.UNDER, GamePeriod.H1, thr: 0.5) },
            {211, new(BetGameType.UNDER, GamePeriod.H1, thr: 1.5) },
            {472, new(BetGameType.UNDER, GamePeriod.H1, thr: 2.5) },
            {207, new(BetGameType.OVER, GamePeriod.H1, thr: 0.5) },
            {208, new(BetGameType.OVER, GamePeriod.H1, thr: 1.5) },
            {209, new(BetGameType.OVER, GamePeriod.H1, thr: 2.5) },
            {210, new(BetGameType.OVER, GamePeriod.H1, thr: 3.5) },

            {269, new(BetGameType.UNDER, GamePeriod.H2, thr: 0.5) },
            {217, new(BetGameType.UNDER, GamePeriod.H2, thr: 1.5) },
            {474, new(BetGameType.UNDER, GamePeriod.H2, thr: 2.5) },
            {213, new(BetGameType.OVER, GamePeriod.H2, thr: 0.5) },
            {214, new(BetGameType.OVER, GamePeriod.H2, thr: 1.5) },
            {215, new(BetGameType.OVER, GamePeriod.H2, thr: 2.5) },
            {216, new(BetGameType.OVER, GamePeriod.H2, thr: 2.5) },
            
            {239, new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 0.5) },
            {247, new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 1.5) },
            {551, new(BetGameType.UNDER, GamePeriod.M, Team.T1, thr: 2.5) }, // check
            {238, new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 0.5) },
            {248, new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 1.5) },
            {276, new(BetGameType.OVER, GamePeriod.M, Team.T1, thr: 2.5) },

            {337, new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 0.5) },
            {817, new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 1.5) }, // check
            //{"", new(BetGameType.UNDER, GamePeriod.H1, Team.T1, thr: 2.5) },
            {307, new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 0.5) },
            {274, new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 1.5) },
            {349, new(BetGameType.OVER, GamePeriod.H1, Team.T1, thr: 2.5) },

            {339, new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 0.5) },
            {825, new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 1.5) },
            //{"", new(BetGameType.UNDER, GamePeriod.H2, Team.T1, thr: 2.5) },
            {312, new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 0.5) },
            {297, new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 1.5) },
            {351, new(BetGameType.OVER, GamePeriod.H2, Team.T1, thr: 2.5) },

            {241, new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 0.5) },
            {249, new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 1.5) },
            {552, new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 2.5) },
            {554, new(BetGameType.UNDER, GamePeriod.M, Team.T2, thr: 3.5) },
            {240, new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 0.5) },
            {250, new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 1.5) },
            {277, new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 2.5) },
            {556, new(BetGameType.OVER, GamePeriod.M, Team.T2, thr: 3.5) },

            {338, new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 0.5) },
            {821, new(BetGameType.UNDER, GamePeriod.H1, Team.T2, thr: 1.5) },
            {308, new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 0.5) },
            {275, new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 1.5) },
            {350, new(BetGameType.OVER, GamePeriod.H1, Team.T2, thr: 2.5) },

            {340, new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 0.5) },
            {829, new(BetGameType.UNDER, GamePeriod.H2, Team.T2, thr: 1.5) },
            {313, new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 0.5) },
            {298, new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 1.5) },
            {352, new(BetGameType.OVER, GamePeriod.H2, Team.T2, thr: 2.5) },
        };
    }
}
