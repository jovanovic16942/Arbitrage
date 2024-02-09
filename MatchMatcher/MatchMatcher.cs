using Arbitrage.General;
using Arbitrage.Utils;
using NLog;

namespace Arbitrage.MatchMatcher
{
    // TODO
    /*
     * 
     * [2024-02-01 20:39:06.9204] [INFO] [MatchMatcher.cs:74] Matched: {U.A.N.L.- TIGRES U23}{U.N.A.M.- PUMAS U23}[OktagonBet] with: <03/02/2024 16:00:00>{ANNAN}{ALLOA}[PinnBet]
     * 
     * */
    public class MatchMatcher
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly double MIN_SCORE_THR = 0.5;
        private static readonly double BEST_SCORE_THR = 0.95;

        public static List<EventData> MatchMatches(List<List<HouseMatchData>> datas)
        {
            var matchedMatches = new List<EventData>();

            foreach (var houseData in datas)
            {
                foreach (var matchData in houseData)
                {
                    MatchMatch(matchedMatches, matchData);
                }
            }

            var matched = matchedMatches.Where(x => x.data.Count > 1).ToList();
            var unmatched = matchedMatches.Where(x => x.data.Count == 1).ToList();

            // Second pass for unmatched matches
            foreach (var match in unmatched)
            {
                MatchMatch(matched, match.data.First(), true);
            }

            return matched;
        }

        private static void MatchMatch(List<EventData> matched, HouseMatchData match, bool filterHouses = false)
        {
            bool matchFilter(EventData candidate)
            {
                List<bool> conditions = new()
                {
                    candidate.startTime.Equals(match.startTime),
                    candidate.sport.Equals(match.sport),
                };

                if (filterHouses)
                {
                    conditions.Add(!candidate.data.Any(x => x.house == match.house));
                }

                return conditions.All(x => x);
            }

            // Filter matches
            var filteredMatches = matched.Where(x => matchFilter(x)).ToList();

            if (!filteredMatches.Any()) 
            {
                log.Debug("No potential candidates for match: " + match);
                matched.Add(new(match));
                return;
            }

            EventData? bestMatch = null;
            var bestMatchScore = 0.0;

            foreach (var potentialMatch in filteredMatches)
            {
                var score = CompareMatchToEvent(match, potentialMatch);

                if (score > bestMatchScore)
                {
                    bestMatch = potentialMatch;
                    bestMatchScore = score;

                    if (score > BEST_SCORE_THR)
                    {
                        break;
                    }
                }
            }

            if (bestMatch == null)
            {
                log.Info("Found: " + filteredMatches.Count + "candidates. Unable to match: " + match);
                matched.Add(new(match));
                return;
            }

            // Check if a match was already made from same house as bestMatch and choose better match
            var prevMatch = bestMatch.data.FirstOrDefault(x => x.house == match.house);
            if (prevMatch == null)
            {
                log.Info("Matched: " + match + " with: " + bestMatch);
                bestMatch.data.Add(match);
                return;
            }

            var prevScore = CompareMatchToEvent(prevMatch, bestMatch);
            if (prevScore > bestMatchScore)
            {
                log.Info(string.Format("Matched: {0} with: {1}, but a better match was present: {2}", match, bestMatch, prevMatch));
                matched.Add(new(match));
            }
            else if (prevScore == bestMatchScore)
            {
                log.Info(string.Format("Matched: {0} with: {1}, but found equal match: {2}, keeping both", match, bestMatch, prevMatch));
                bestMatch.data.Add(match);
            }
            else // if (bestMatchScore > prevScore)
             {
                log.Info(string.Format("Matched: {0} with: {1}, replacing worse match: {2}", match, bestMatch, prevMatch));
                bestMatch.data.Remove(prevMatch);
                bestMatch.data.Add(match);
                matched.Add(new(prevMatch));
            }
        }

        private static double CompareMatchToEvent(HouseMatchData match, EventData potentialMatch)
        {
            var totalScore = 0.0;
            var cnt = potentialMatch.data.Count;

            foreach (var houseData in potentialMatch.data)
            {
                // Do not compare to data from same house
                if (houseData.house == match.house) 
                {
                    cnt--;
                    continue; 
                }

                var score = CompareMatches(match, houseData);
                
                if (score < MIN_SCORE_THR)
                {
                    return 0;
                }

                totalScore += score;
            }

            return totalScore / cnt;
        }

        /// <summary>
        /// Calculate match similarity score [0,1] based on team names 
        /// TODO Add league information in getters to improve matching
        /// </summary>
        /// <param name="matchA"></param>
        /// <param name="matchB"></param>
        /// <returns></returns>
        private static double CompareMatches(HouseMatchData matchA, HouseMatchData matchB)
        {
            var s1 = CompareStrings(matchA.team1, matchB.team1);
            if (s1 < MIN_SCORE_THR) return 0;

            var s2 = CompareStrings(matchA.team2, matchB.team2);
            if (s2 < MIN_SCORE_THR) return 0;

            return (s1 + s2) / 2;
        }

        /// <summary>
        /// Return string similarity score in a range [0,1]
        /// </summary>
        /// <param name="name1">Name of the first team (including separators)</param>
        /// <param name="name2">Name of the second team (including separators)</param>
        /// <returns></returns>
        private static double CompareStrings(string name1, string name2)
        {
            if (name1 == name2) return 1.0;
            if (name1.Length > name2.Length && name1.Contains(name2)) return 1.0;
            if (name1.Length < name2.Length && name2.Contains(name1)) return 1.0;

            var tokensA = TokenizeString(name1);
            var tokensB = TokenizeString(name2);

            string[] tokens1 = tokensA;
            string[] tokens2 = tokensB;

            if (tokensB.Length > tokensA.Length)
            {
                tokens1 = tokensB;
                tokens2 = tokensA;
            }

            var score = 0.0;
            var totalLength = 0.0;

            foreach (var t in tokens2)
            {
                var tokenScore = 0.0;
                var mod = 0.0;

                foreach (var t2 in tokens1)
                {
                    var tmpScore = MatchTokens(t, t2);
                    tokenScore = Math.Max(tokenScore, tmpScore);
                    mod = t2.Length;

                    if (tokenScore > BEST_SCORE_THR)
                    {
                        break;
                    }
                }

                mod += t.Length;
                totalLength += mod;
                score += tokenScore * mod;
            }

            return score / totalLength;
        }

        private static string[] TokenizeString(string str, char[]? separators = null)
        {
            separators ??= new char[] { ' ', '.', '-' };
            return str.Split(separators).Where(x => x.Length > 0).ToArray();
        }
        
        /// <summary>
        /// Return similarity score between two tokens
        /// </summary>
        /// <param name="A">Token A</param>
        /// <param name="B">Token B</param>
        /// <returns></returns>
        private static double MatchTokens(string A, string B)
        {
            if (A == B)
            {
                return 1.0;
            }
            
            if ((B.Length > A.Length))
            {
                var tmp = B;
                B = A;
                A = tmp;
            }

            if (A.StartsWith(B))
            {
                return 0.5 * (1.0 + (double)B.Length / A.Length);
            }

            if (B[0] == A[0])
            {
                var idxB = 1;
                for (var idxA = 1; idxA < A.Length && idxB < B.Length; idxA++)
                {
                    if (A[idxA] == B[idxB])
                    {
                        idxB++;
                    }
                }

                if (idxB == B.Length)
                {
                    return 0.5 * (1.0 + (double)B.Length / A.Length);
                }
            }

            return 0.0;
        }
    }
}
