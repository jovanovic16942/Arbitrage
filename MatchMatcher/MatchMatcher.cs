using Arbitrage.General;
using Arbitrage.Utils;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.MatchMatcher
{
    public class MatchMatcher
    {
        public static List<EventData> MatchMatches(List<MatchesData> datas)
        {
            var matchedMatches = new List<EventData>();

            foreach (var houseData in datas)
            {
                var betHouse = houseData.bettingHouse;
                var matches = houseData.GetMatches();

                foreach (var match in matches)
                {
                    MatchMatch(matchedMatches, match, betHouse);
                }
            }

            return matchedMatches;
        }

        private static void MatchMatch(List<EventData> matched, Match match, BettingHouses house) 
        {
            // Create EventData from match

            List<Participant> participants = new List<Participant>
            {
                match.Team1,
                match.Team2
            };

            EventData ev = new EventData(match.StartTime, participants);

            HouseOdds odds = new HouseOdds(house);

            foreach (var (betGame, value) in match.BetGames)
            {
                odds.AddOdds(betGame, value);
            }

            ev.odds.Add(odds);

            // Find matching event

            bool foundMatch = false;

            // Filter by start time
            var filteredMatches = matched.Where(x => x.startTime.Equals(ev.startTime)).ToList();

            foreach (var potentialMatch in filteredMatches) 
            {
                if (CompareEvents(ev, potentialMatch))
                {
                    File.AppendAllText("..\\..\\..\\Temp\\MatchMatcherMatched.txt", ev.ToString() + Environment.NewLine);
                    File.AppendAllText("..\\..\\..\\Temp\\MatchMatcherMatched.txt", potentialMatch.ToString() + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                    potentialMatch.odds.Add(odds);
                    foundMatch = true;
                    break;
                }
            }

            if (!foundMatch)
            {
                matched.Add(ev);
            }

        }

        private static bool CompareEvents(EventData eventA, EventData eventB)
        {
            int score = 0;
            int minScore = 0;

            for (int i = 0; i < eventA.teams.Count; i++)
            {
                var teamA = eventA.teams[i].Name;
                var tokensA = TokenizeString(teamA);

                var teamB = eventB.teams[i].Name;
                var tokensB = TokenizeString(teamB);

                minScore += Math.Min(tokensA.Length, tokensB.Length);

                foreach (var tokenA in tokensA)
                {
                    foreach (var tokenB in tokensB)
                    {
                        if (MatchTokens(tokenA, tokenB))
                        {
                            score++;
                            break;
                        }
                    }
                }
            }

            return score >= minScore;
        }

        private static string[] TokenizeString(string str, char[]? separators = null)
        {
            separators ??= new char[] { ' ', '.' };
            return str.Split(separators).Where(x => x.Length > 0).ToArray();
        }

        private static bool MatchTokens(string A, string B)
        {
            bool matching = false;

            //A = new string(A.Where(char.IsLetter).ToArray());
            //B = new string(B.Where(char.IsLetter).ToArray());

            if (A == B)
            {
                matching = true;
            }
            else if ((A.Length > B.Length) && A.Contains(B))
            {
                matching = true;
            }
            else if ((A.Length < B.Length) && B.Contains(A))
            {
                matching = true;
            }

            return matching;
        }
    }
}
