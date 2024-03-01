using Arbitrage.General;
using Arbitrage.Utils;
using HtmlAgilityPack;
using System.Globalization;

namespace Arbitrage.DataGetters.Olimp
{
    internal class OlimpParser : Parser
    {
        private readonly OlimpGetter _getter = new();

        public OlimpParser() : base(BettingHouse.Olimp)
        {
        }

        protected override void ParseFootball()
        {
            var leaguesResponse = _getter.GetLeagues();

            if (string.IsNullOrEmpty(leaguesResponse))
            {
                //log
                return;
            }

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(leaguesResponse);

            if(html.DocumentNode == null)
            {
                //log
                return;
            }

            // TODO FIXME Vraca null 
            var footbalNode = html.DocumentNode.SelectSingleNode(".//div[@id_str='meniLevoPonudaID']//li[@class='has-children']//label[contains(.,'Fudbal')]");

            var leagueNodes = footbalNode.ParentNode.SelectNodes(".//ul[contains(@class, 'cd-accordion-menu')]//li[@class='has-children']//ul[contains(@class,'group')]//li//label");


            List<int> leagueIds = new();
            foreach(var leagueNode in leagueNodes)
            {
                try
                {
                    var leagueId = leagueNode.Attributes["for"].Value;

                    int index = leagueId.LastIndexOf('-');

                    leagueId = leagueId.Substring(index + 1).Trim();

                    leagueIds.Add(int.Parse(leagueId));
                }
                catch 
                {
                    //TODO Logging
                }
            }

            var matchesResponse = _getter.GetMatches(leagueIds);

            ParseMatchesResponse(matchesResponse);
        }

        private void ParseMatchesResponse(string matchesResponse)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(matchesResponse);

            if (html.DocumentNode == null)
            {
                //log
                return;
            }

            var matcheNodes = html.DocumentNode.SelectNodes(".//tr[contains(@id_str,'pkp_scroll')]");

            HashSet<string> matchIDs = new HashSet<string>();

            // Get IDs
            foreach(var matcheNode in matcheNodes)
            {
                try
                {
                    var matchId = matcheNode.Attributes["id_str"].Value.Replace("pkp_scroll_", "").Replace("_ID", "");
                    matchIDs.Add(matchId);
                }
                catch
                {
                    //TODO LOG
                }
            }

            // Get matches
            foreach (var matchId in matchIDs)
            {
                try
                {
                    var matchResponse = _getter.GetMatchOdds(matchId);
                    ParseOddsResponse(matchResponse);
                }
                catch
                {
                    //TODO LOG
                }
            }
        }

        private void ParseOddsResponse(string oddsResponse)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(oddsResponse);
            if (html.DocumentNode == null)
            {
                //Log
                return;
            }

            var oddNodes = html.DocumentNode.SelectNodes(".//ul//li[@class='dugme_kompletna_ponuda_LI']");

            bool flag = false;

            HouseMatchData? match = null;

            foreach (var oddNode in oddNodes)
            {
                if (!flag)
                {
                    match = TryCreateMatch(oddNode);
                    if (match == null) continue;
                    flag = true;
                }

                try
                {
                    var oddValue = double.Parse(oddNode.Attributes["kvotameca"].Value);
                    var oddName = oddNode.Attributes["oznigre"].Value.Trim();
                    var opis = oddNode.Attributes["opiszawebatr"].Value;

                    if (betGameFromString.ContainsKey(oddName))
                    {
                        var game = betGameFromString[oddName].Clone();
                        game.Value = oddValue;
                        match.AddBetGame(game);
                    }

                } catch (Exception e) 
                {
                    // log
                }

            }

            if (match != null)
            {
                _parsedData.Add(match);
            }
        }

        private HouseMatchData? TryCreateMatch(HtmlNode oddNode)
        {
            try
            {
                var teamsStr = oddNode.Attributes["parmeca"].Value;
                var teams = teamsStr.Split('-');

                var startTimeStr = oddNode.Attributes["vrememeca"].Value.Replace(" &amp;nbsp;", "");
                DateTime startTime = DateTime.ParseExact(startTimeStr, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);

                var sportId = int.Parse(oddNode.Attributes["idsporta"].Value);
                // Todo sportID constructor
                return new(House, Sport.Football, startTime, teams[0], teams[1]);

            } catch (Exception ex)
            {
                // log
                return null;
            }
        }

        /// <summary>
        /// Map bet game name from html response oznigre to BetGame
        /// </summary>
        static readonly Dictionary<string, BetGame> betGameFromString = new()
        {
            {"K1", new(BetGameType.WX1) },
            {"KX", new(BetGameType.WXX) },
            {"K2", new(BetGameType.WX2) },
            {"D12", new(BetGameType.D12) },
            {"D1X", new(BetGameType.D1X) },
            {"DX2", new(BetGameType.DX2) },
            {"P1", new(BetGameType.WX1, GamePeriod.H1) },
            {"PX", new(BetGameType.WXX, GamePeriod.H1) },
            {"P2", new(BetGameType.WX2, GamePeriod.H1) },
            {"P12", new(BetGameType.D12, GamePeriod.H1) },
            {"P1X", new(BetGameType.D1X, GamePeriod.H1) },
            {"PX2", new(BetGameType.DX2, GamePeriod.H1) },
            {"DP1", new(BetGameType.WX1, GamePeriod.H2) },
            {"DPX", new(BetGameType.WXX, GamePeriod.H2) },
            {"DP2", new(BetGameType.WX2, GamePeriod.H2) },
            {"DP12", new(BetGameType.D12, GamePeriod.H2) },
            {"DP1X", new(BetGameType.D1X, GamePeriod.H2) },
            {"DPX2", new(BetGameType.DX2, GamePeriod.H2) },
            {"GG", new(BetGameType.GG) },
            {"GN", new(BetGameType.NG) },
            {"GG1", new(BetGameType.GG, GamePeriod.H1) },
            {"GN1", new(BetGameType.NG, GamePeriod.H1) },
            {"GG2", new(BetGameType.GG, GamePeriod.H2) },
            {"GN2", new(BetGameType.NG, GamePeriod.H2) },

            {"G0-1", new(BetGameType.UNDER, thr: 1.5) },
            {"G0-2", new(BetGameType.UNDER, thr: 2.5) },
            {"G0-3", new(BetGameType.UNDER, thr: 3.5) },
            {"G0-4", new(BetGameType.UNDER, thr: 4.5) },
            {"G0-5", new(BetGameType.UNDER, thr : 5.5) },
            {"G0-6", new(BetGameType.UNDER, thr : 6.5) },
            {"G2+", new(BetGameType.OVER, thr: 1.5) },
            {"G3+", new(BetGameType.OVER, thr: 2.5) },
            {"G4+", new(BetGameType.OVER, thr: 3.5) },
            {"G5+", new(BetGameType.OVER, thr: 4.5) },
            {"G6+", new(BetGameType.OVER, thr: 5.5) },
            // TODO OVER UNDER per team and halftimes
        };
    }
}
