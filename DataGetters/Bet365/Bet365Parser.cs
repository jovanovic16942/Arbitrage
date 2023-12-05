using Arbitrage.General;
using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Bet365
{
    internal class Bet365Parser : Parser
    {
        private readonly Bet365Getter _getter = new();

        public Bet365Parser() : base(BettingHouse.Bet365)
        {
        }

        protected override void UpdateData()
        {
            var jsonResp = _getter.GetLeagues();

            if (jsonResp == null || jsonResp.Count == 0) { return; }

            foreach (var jsonSport in jsonResp) 
            {
                ParseJsonSport(jsonSport);
            }
        }

        private void ParseJsonSport(JsonSport jsonSport)
        {
            if (jsonSport == null || jsonSport.leagues == null) { return; }
            if (jsonSport.sportType != "S" || jsonSport.leagues.Count == 0) { return; }

            foreach (var jsonLeague in jsonSport.leagues)
            {
                ParseJsonLeague(jsonLeague);
            }
        }

        private void ParseJsonLeague(JsonLeague jsonLeague)
        {
            if (jsonLeague == null || jsonLeague.numOfMatches == 0) { return; }

            var leagueId = jsonLeague.betLeagueId;

            try
            {
                var leagueMatchesResp = _getter.GetMatchesInLeague(leagueId);
                
                foreach (var jsonMatch in leagueMatchesResp.matchList)
                {
                    TryParseJsonMatch(jsonMatch);
                }

            } catch (Exception e)
            {
                // log
            }
        }

        private void TryParseJsonMatch(JsonMatch jsonMatch)
        {
            try
            {
                if (jsonMatch.sport.Trim() != "S") { return; }

                DateTime startTime = DateTimeConverter.DateTimeFromLong(jsonMatch.kickOffTime, 1);

                Participant p1 = new(jsonMatch.home.Trim());
                Participant p2 = new(jsonMatch.away.Trim());

                Match match = new(startTime, p1, p2);

                // Add odds
                var matchId = jsonMatch.id;
                var jsonMatchResp = _getter.GetMatchResponse(matchId);

                foreach (var jsonBetGroup in jsonMatchResp.odBetPickGroups)
                {
                    TryParseBetGroup(match, jsonBetGroup);
                }

                _data.Insert(match);

            }
            catch
            {
                // log
                return;
            }
        }

        private void TryParseBetGroup(Match match, JsonBetGroup jsonBetGroup)
        {
            if (jsonBetGroup.tipTypes == null || !jsonBetGroup.tipTypes.Any()) { return; } // log

            foreach (var jsonBet in jsonBetGroup.tipTypes)
            {
                try
                {
                    if (betGameFromString.ContainsKey(jsonBet.name))
                    {
                        match.TryAddBetGame(betGameFromString[jsonBet.name],  jsonBet.value);
                    }
                } catch
                {
                    continue; // log
                }
            }
        }

        /// <summary>
        /// Map bet game name from json response name to BettingGames enum
        /// </summary>
        static readonly Dictionary<string, BettingGames> betGameFromString = new()
        {
            {"1", BettingGames._1 },
            {"X", BettingGames._X },
            {"2", BettingGames._2 },
            {"12", BettingGames._12 },
            {"1X", BettingGames._1X },
            {"X2",BettingGames._X2 },
            {"1P1", BettingGames._1_I },
            {"1PX", BettingGames._X_I },
            {"1P2", BettingGames._2_I },
            {"1P 12", BettingGames._12_I },
            {"1P 1X", BettingGames._1X_I },
            {"1P X2", BettingGames._X2_I },
            {"2P1", BettingGames._1_II },
            {"2PX", BettingGames._X_II },
            {"2P2", BettingGames._2_II },
            {"2P 12", BettingGames._12_II },
            {"2P 1X", BettingGames._1X_II },
            {"2P X2", BettingGames._X2_II },
            {"GG", BettingGames._GG },
            {"NG", BettingGames._NG },
            {"1P GG", BettingGames._GG_I },
            {"1P NG", BettingGames._NG_I },
            {"2P GG", BettingGames._GG_II },
            {"2P NG", BettingGames._NG_II },
            {"0-1", BettingGames._UG_0_1 },
            {"0-2", BettingGames._UG_0_2 },
            {"0-3", BettingGames._UG_0_3 },
            {"0-4", BettingGames._UG_0_4 },
            {"0-5", BettingGames._UG_0_5 },
            {"0-6", BettingGames._UG_0_6 },
            {"1-2", BettingGames._UG_1_2 },
            {"1-3", BettingGames._UG_1_3 },
            {"1-4", BettingGames._UG_1_4 },
            {"1-5", BettingGames._UG_1_5 },
            {"1-6", BettingGames._UG_1_6 },
            {"2-3", BettingGames._UG_2_3 },
            {"2-4", BettingGames._UG_2_4 },
            {"2-5", BettingGames._UG_2_5 },
            {"2-6", BettingGames._UG_2_6 },
            {"3-4", BettingGames._UG_3_4 },
            {"3-5", BettingGames._UG_3_5 },
            {"3-6", BettingGames._UG_3_6 },
            {"4-5", BettingGames._UG_4_5 },
            {"4-6", BettingGames._UG_4_6 },
            {"5-6", BettingGames._UG_5_6 },
            {"2+", BettingGames._UG_2_PLUS },
            {"3+", BettingGames._UG_3_PLUS },
            {"4+", BettingGames._UG_4_PLUS },
            {"5+", BettingGames._UG_5_PLUS },
            {"6+", BettingGames._UG_6_PLUS },
        };
    }
}
