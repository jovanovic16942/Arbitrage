using Arbitrage.General;
using Arbitrage.Utils;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Olimp
{
    internal class OlimpParser : Parser
    {
        private readonly OlimpGetter _getter = new();

        public OlimpParser() : base(BettingHouse.Olimp)
        {
        }

        protected override void UpdateData()
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

            Match? match = null;

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
                        match!.AddBetGame(betGameFromString[oddName], oddValue);
                    }

                } catch (Exception e) 
                {
                    // log
                }

            }

            if (match != null)
            {
                _data.Insert(match);
            }
        }

        private Match? TryCreateMatch(HtmlNode oddNode)
        {
            try
            {
                var teamsStr = oddNode.Attributes["parmeca"].Value;
                var teams = teamsStr.Split('-');
                Participant p1 = new(teams[0].Trim());
                Participant p2 = new(teams[1].Trim());

                var startTimeStr = oddNode.Attributes["vrememeca"].Value.Replace(" &amp;nbsp;", "");
                DateTime startTime = DateTime.ParseExact(startTimeStr, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);

                var sportId = int.Parse(oddNode.Attributes["idsporta"].Value);
                // Todo sportID constructor
                return new(startTime, p1, p2);

            } catch (Exception ex)
            {
                // log
                return null;
            }
        }

        /// <summary>
        /// Map bet game name from html response oznigre to BettingGames enum
        /// </summary>
        static readonly Dictionary<string, BettingGames> betGameFromString = new()
        {
            {"K1", BettingGames._1 },
            {"KX", BettingGames._X },
            {"K2", BettingGames._2 },
            {"D12", BettingGames._12 },
            {"D1X", BettingGames._1X },
            {"DX2",BettingGames._X2 },
            {"P1", BettingGames._1_I },
            {"PX", BettingGames._X_I },
            {"P2", BettingGames._2_I },
            {"P12", BettingGames._12_I },
            {"P1X", BettingGames._1X_I },
            {"PX2", BettingGames._X2_I },
            {"DP1", BettingGames._1_II },
            {"DPX", BettingGames._X_II },
            {"DP2", BettingGames._2_II },
            {"DP12", BettingGames._12_II },
            {"DP1X", BettingGames._1X_II },
            {"DPX2", BettingGames._X2_II },
            {"GG", BettingGames._GG },
            {"GN", BettingGames._NG },
            {"GG1", BettingGames._GG_I },
            {"GN1", BettingGames._NG_I },
            {"GG2", BettingGames._GG_II },
            {"GN2", BettingGames._NG_II },
            {"G0-1", BettingGames._UG_0_1 },
            {"G0-2", BettingGames._UG_0_2 },
            {"G0-3", BettingGames._UG_0_3 },
            {"G0-4", BettingGames._UG_0_4 },
            {"G0-5", BettingGames._UG_0_5 },
            {"G0-6", BettingGames._UG_0_6 },
            {"G1-2", BettingGames._UG_1_2 },
            {"G1-3", BettingGames._UG_1_3 },
            {"G1-4", BettingGames._UG_1_4 },
            {"G1-5", BettingGames._UG_1_5 },
            {"G1-6", BettingGames._UG_1_6 },
            {"G2-3", BettingGames._UG_2_3 },
            {"G2-4", BettingGames._UG_2_4 },
            {"G2-5", BettingGames._UG_2_5 },
            {"G2-6", BettingGames._UG_2_6 },
            {"G3-4", BettingGames._UG_3_4 },
            {"G3-5", BettingGames._UG_3_5 },
            {"G3-6", BettingGames._UG_3_6 },
            {"G4-5", BettingGames._UG_4_5 },
            {"G4-6", BettingGames._UG_4_6 },
            {"G5-6", BettingGames._UG_5_6 },
            {"G2+", BettingGames._UG_2_PLUS },
            {"G3+", BettingGames._UG_3_PLUS },
            {"G4+", BettingGames._UG_4_PLUS },
            {"G5+", BettingGames._UG_5_PLUS },
            {"G6+", BettingGames._UG_6_PLUS },
        };
    }
}
