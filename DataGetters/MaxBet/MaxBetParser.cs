using Arbitrage.DataGetters.MMOB;
using Arbitrage.General;
using Arbitrage.Utils;
using NLog;

namespace Arbitrage.DataGetters.MaxBet
{
    internal class MaxBetParser : Parser
    {
        private readonly MaxBetGetter _getter = new();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public MaxBetParser() : base(BettingHouse.MaxBet) { }

        private void ParseMatch(JsonMatch jsonMatch, Sport sport)
        {
            if (jsonMatch == null || jsonMatch.odds == null) return;

            DateTime startTime = DateTimeConverter.DateTimeFromLong(jsonMatch.kickOffTime, 1); // TODO SUS 

            HouseMatchData hmd = new(House, sport, startTime, jsonMatch.home.Trim(), jsonMatch.away.Trim());

            // Add odds
            foreach (var (id, val) in jsonMatch.odds)
            {
                try
                {
                    if (MMOBParser.betGameFromID.ContainsKey(id))
                    {
                        BetGame bg = MMOBParser.betGameFromID[id].Clone();
                        bg.Value = val;

                        // Special parsing for basketball odds
                        if (sport is Sport.Basketball && bg.type is BetGameType.OVER or BetGameType.UNDER)
                        {
                            var thr = MMOBParser.GetThreshold(id, jsonMatch.betParams);
                            bg.SetThreshold(thr);
                        }

                        hmd.AddBetGame(bg);
                    }
                } catch (Exception ex)
                {
                    log.Error("Exception while parsing bet game:");
                    log.Error(ex);
                }
            }

            _parsedData.Add(hmd);
        }

        private void ParseLeagues(List<string> leagueIDs, Sport sport)
        {
            foreach (var leagueID in leagueIDs)
            {
                //specials
                if (leagueID == "138547")
                {
                    continue;
                }

                //Console.WriteLine("MaxBetParser leagueID: " + leagueID);

                var sportStr = sport switch
                {
                    Sport.Football => "S",
                    Sport.Basketball => "B",
                    _ => "S",
                };

                try
                {
                    JsonMatchResponse resp = _getter.GetMatches(leagueID, sportStr);
                    if (resp == null)
                    {
                        continue;
                    }

                    foreach (var match in resp.esMatches)
                    {
                        ParseMatch(match, sport);
                    }
                } catch (Exception e)
                {
                    log.Error(e);
                }
            }
        }

        protected override void ParseFootball()
        {
            var leagueIDs = _getter.GetLeagues("S");
            ParseLeagues(leagueIDs, Sport.Football);
        }
        protected override void ParseBasketball()
        {
            var leagueIDs = _getter.GetLeagues("B");
            ParseLeagues(leagueIDs, Sport.Basketball);
        }
    }
}
