using Arbitrage.General;
using Arbitrage.Utils;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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

            /* Filter potential matches based on:
             * 1. Start time
             * 2. Whether potential match was already filled with data from current bet house
            */
            Func<EventData, bool> matchFilter = candidate =>
            {
                List<bool> conditions = new List<bool>()
                {
                    candidate.startTime.Equals(ev.startTime),
                    !candidate.odds.Any(x => x.House == house)
                };

                return conditions.All(x => x);
            };

            // Filter matches
            var filteredMatches = matched.Where(x => matchFilter(x)).ToList();

            EventData? bestMatch = null;
            int bestMatchScore = 0;

            foreach (var potentialMatch in filteredMatches) 
            {
                int score = CompareEvents(ev, potentialMatch);

                if (score > bestMatchScore)
                {
                    bestMatch = potentialMatch;
                    bestMatchScore = score;
                }
            }

            if (bestMatch != null)
            {
                //File.AppendAllText("..\\..\\..\\Temp\\MatchMatcherMatched.txt", ev.ToString() + Environment.NewLine);
                //File.AppendAllText("..\\..\\..\\Temp\\MatchMatcherMatched.txt", bestMatch.ToString() + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                bestMatch.odds.Add(odds);
            } else
            {
                matched.Add(ev);
            }

        }

        /// <summary>
        /// Return similarity score of two events, based on string similarity of their participant names
        /// It is assumed that in both events, participant lists are in the same order
        /// </summary>
        /// <param name="eventA">First event</param>
        /// <param name="eventB">Second event</param>
        /// <returns></returns>
        private static int CompareEvents(EventData eventA, EventData eventB)
        {
            int totalScore = 0;

            for (int i = 0; i < eventA.teams.Count; i++)
            {
                int score = 0;

                var teamA = eventA.teams[i].Name;
                var tokensA = TokenizeString(teamA);

                var teamB = eventB.teams[i].Name;
                var tokensB = TokenizeString(teamB);

                int minScore = Math.Min(tokensA.Length, tokensB.Length);

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

                if (score > 0)
                {
                    Console.WriteLine("");
                }

                if (score >= minScore)
                {
                    totalScore += score;
                } else
                {
                    return 0;
                }
            }

            return totalScore;
        }

        private static string[] TokenizeString(string str, char[]? separators = null)
        {
            separators ??= new char[] { ' ', '.', '-' };
            return str.Split(separators).Where(x => x.Length > 0).ToArray();
        }

        private static bool MatchTokens(string A, string B)
        {
            bool matching = false;

            if (A == B)
            {
                matching = true;
            }
            else if ((A.Length > B.Length) && A.StartsWith(B))
            {
                matching = true;
            }
            else if ((A.Length < B.Length) && B.StartsWith(A))
            {
                matching = true;
            }

            return matching;
        }
    }
}
