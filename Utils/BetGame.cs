
using NLog;

namespace Arbitrage.General
{
    public class BetGame
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static readonly Dictionary<BetGameType, BetGameType> ArbCombos = new()
        {
            { BetGameType.W1, BetGameType.W2 },
            { BetGameType.W2, BetGameType.W1 },

            { BetGameType.WX1, BetGameType.DX2 },
            { BetGameType.DX2, BetGameType.WX1 },

            { BetGameType.WXX, BetGameType.D12 },
            { BetGameType.D12, BetGameType.WXX },

            { BetGameType.WX2, BetGameType.D1X },
            { BetGameType.D1X, BetGameType.WX2 },

            { BetGameType.GG, BetGameType.NG },
            { BetGameType.NG, BetGameType.GG },

            { BetGameType.OVER, BetGameType.UNDER },
            { BetGameType.UNDER, BetGameType.OVER },

            { BetGameType.W1_X_0, BetGameType.W2_X_0 },
            { BetGameType.W2_X_0, BetGameType.W1_X_0 },
        };

        /// <summary>
        /// Type of bet game (1, X, 2, OVER, UNDER, etc)
        /// </summary>
        public readonly BetGameType type;
        /// <summary>
        /// Match time period of interest for this bet
        /// </summary>
        public GamePeriod period;

        /// <summary>
        /// Both teams, team1 or team2
        /// </summary>
        public readonly Team? team;

        private string id_str;

        private readonly string sep = "_";

        /// <summary>
        /// The threshold used for over/under bet betGames (total number of goals, points, etc)
        /// </summary>
        public double? threshold;

        public double Value { get; set; }

        public BetGame Clone()
        {
            return new(type, period, team, threshold);
        }

        public BetGame(BetGameType t, GamePeriod p = GamePeriod.M, Team? tm = null, double? thr = null)
        {
            type = t;
            period = p;
            team = tm;
            threshold = thr;
            Value = 0;
            InitIdString();
        }

        public void SetPeriod(GamePeriod p)
        {
            period = p;
            InitIdString();
        }
        public void SetThreshold(double thr)
        {
            threshold = thr;
            InitIdString();
        }

        public void CheckBetGame()
        {
            if ((type == BetGameType.OVER || type == BetGameType.UNDER) && threshold == null)
            {
                throw new ArgumentException("Bet games of type UNDER/OVER require a threshold value");
            }
        }

        /// <summary>
        /// Get the opposite bet game (1 - X2, 1X - 2, Over - Under, GG - NG, etc)
        /// </summary>
        /// <returns>BetGame object</returns>
        public BetGame GetOppositeGame()
        {
            return new BetGame(ArbCombos[type], period, team, threshold);
        }

        /// <summary>
        /// Generate unique bet game id string based on other member parameters
        /// </summary>
        /// <returns></returns>
        private void InitIdString()
        {
            var tokens = new List<string?>()
            {
                period.ToString(),
                type.ToString(),
                threshold?.ToString(),
                team?.ToString(),
            };

            id_str = string.Join(sep, tokens.Where(x => x != null && x != ""));
        }

        public override int GetHashCode()
        {
            return id_str.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            return Equals((BetGame)obj);
        }

        public override string ToString()
        {
            return id_str;
        }

        public bool Equals(BetGame obj)
        {
            return obj != null && obj.id_str == id_str;
        }
    }
}
