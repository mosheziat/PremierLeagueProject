using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProjects
{
    public class LettersSequenceCalculator
    {
        public static string GetCompetitionMatchResultLetter(competitionmatch match, int teamId)
        {
            var latest = "";
            if (match.WinnerTeamID == teamId)
            {
                latest = "W";
            }

            else if (match.WinnerTeamID == null)
            {
                latest = "D";
            }

            else
            {
                latest = "L";
            }

            return latest;
        }
        public static string GetTeamLatestSequence(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50)
        {
            var latestMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, gamesToTake)
                .OrderBy(x => x.MatchDate)
                .Reverse()
                .Select(x => GetCompetitionMatchResultLetter(x, teamId))
                .ToList();
            var firstLetter = latestMatches.First();
            var sequence = firstLetter;
            var curLetter = latestMatches[1];
            var i = 1;
            while (curLetter == firstLetter && i < latestMatches.Count - 1)
            {
                sequence += " " + curLetter;
                i++;
                curLetter = latestMatches[i];
            }

            return sequence;
        }
        public static List<string> GetTeamLetterSequence(sakilaEntities4 db, int teamId, int competitionId,
            int gamesToTake = 50)
        {
            var latestMatchesLetters = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, gamesToTake)
                                .OrderBy(x => x.MatchDate)
                                .Select(x => GetCompetitionMatchResultLetter(x, teamId))
                                .ToList();

            return latestMatchesLetters;
        }

        public static string GetTeamLetterSequence(List<competitionmatch> matches, int teamId, int competitionId,
    int gamesToTake = 50)
        {
            var latestMatches = matches
                .OrderBy(x => x.MatchDate)
                .Reverse()
                .Select(x => GetCompetitionMatchResultLetter(x, teamId))
                .ToList();
            var firstLetter = latestMatches.First();
            var sequence = firstLetter;
            var curLetter = latestMatches[1];
            var i = 1;
            while (curLetter == firstLetter && i < latestMatches.Count - 1)
            {
                sequence += " " + curLetter;
                i++;
                curLetter = latestMatches[i];
            }

            return sequence;
        }

        public static int GetTeamLetterScore(List<competitionmatch> matches, int teamId, int competitionId,
                int gamesToTake = 50)
        {
            var latestMatches = matches
                .OrderBy(x => x.MatchDate)
                .Reverse()
                .Select(x => GetCompetitionMatchResultLetter(x, teamId))
                .ToList();

            var firstLetter = latestMatches.First();
            var curLetter = latestMatches[1];
            var i = 1;
            var sequence = 1;
            while (i < latestMatches.Count)
            {
                curLetter = latestMatches[i];
                if (curLetter != firstLetter)
                    break;
                i++;

            }

            var factor = 0;
            if (firstLetter.Equals("W"))
            {
                factor = 1;
            }
            else if (firstLetter.Equals("L"))
            {
                factor = -1;
            }

            return factor * i;
        }

        public static List<List<string>> GetAllTeamsLettersSequence(sakilaEntities4 db, int competitionId, int gamesToTake = 50)
        {
            var toReturn = new List<List<string>>();
            var allTeamIds = db.competitionmatch
                .Where(x => x.CompetitionID == competitionId)
                .Select(x => x.HomeTeamID)
                .Distinct()
                .ToList();

            foreach (var teamId in allTeamIds)
            {
                var sequenceLetters = GetTeamLetterSequence(db, teamId, competitionId, gamesToTake);
                toReturn.Add(sequenceLetters);
            }

            return toReturn;
        }
        public static Dictionary<string, List<Helper.LetterDistribution>> GetFullSequenceDictionary(List<int> competitionIds, int gamesToTake = 50, int sequenceAmount = 3)

        {
            using (var db = new sakilaEntities4())
            {

                var allSequences = new List<List<string>>();
                foreach (var competitionId in competitionIds)
                {
                    Console.WriteLine($"Building Sequence Dictionary for competition ID: {competitionId}");
                    allSequences.AddRange(GetAllTeamsLettersSequence(db, competitionId, gamesToTake));
                }

                return Helper.BuildSequencesDict(allSequences, sequenceAmount);
            }

        }
        public static Helper.LetterDistribution BuildSequenceStringExpectation(sakilaEntities4 db, int teamId, int competitionId,
             Dictionary<string, List<Helper.LetterDistribution>> lettersDict, out string letterSeq, int gamesToTake = 50, int lettersAmount = 3)
        {
            var teamLatestSequence = GetTeamLetterSequence(db, teamId, competitionId, lettersAmount);
            letterSeq = string.Join("", teamLatestSequence);
            var seq = letterSeq;
            var relevantList = lettersDict.First(x => x.Key.Equals(seq)).Value;
            var ordered = relevantList
                    .Where(x => x.Count > 0)
                    .OrderByDescending(x => x.Percent);

            if (!ordered.Any())
            {
                return null;
            }

            var relevant = ordered.First();
            return relevant;
        }

        public static string BuildSequenceStringExpectation(Helper.LetterDistribution relevant, string letterSeq)
        {
           return relevant.Letter + " (" + Math.Round(relevant.Percent*100, 2) + "%, Count: " + relevant.Count + " " + letterSeq + ")";
        }

        public static Dictionary<string, List<Helper.LetterDistribution>> GetCombinedLettersDictionary(int sequenceAmount = 3)
        {
            var competitionId = 8;
            var competitionLastYearId = 3;
            var competitionTwoYearsAgo = 2;
            var competitionThreeYearsAgo = 4;
            var competitionFourYearsAgo = 5;
            var competitionFiveYearsAgo = 6;
            var competitionSixYearsAgo = 7;

            var competitionIds = new List<int>
            {
                competitionId, competitionLastYearId,
                competitionTwoYearsAgo, competitionThreeYearsAgo,
                competitionFourYearsAgo,competitionFiveYearsAgo,
                competitionSixYearsAgo
            };
            var combinedLetterDict = GetFullSequenceDictionary(competitionIds, 50, sequenceAmount);

            return combinedLetterDict;
        }
        public static decimal GetTeamFormInLatestMatchesHomeAway(sakilaEntities4 db, int teamId, int competitionId, string homeOrAway, int matchesToTake = 50)
        {
            var latestMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, matchesToTake);
            if (homeOrAway == "Home")
            {
                latestMatches = latestMatches.Where(x => x.HomeTeamID == teamId).ToList();
            }
            if (homeOrAway == "Away")
            {
                latestMatches = latestMatches.Where(x => x.AwayTeamID == teamId).ToList();
            }
            var pointsAggregated = PointsCalculator.CalculatePointsForTeamInMatches(latestMatches, teamId);
            return Math.Round((decimal)pointsAggregated / latestMatches.Count, 2);
        }

    }
}
