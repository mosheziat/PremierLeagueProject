﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;

namespace DataProjects
{
    public class MainCalculator
    {
        public static List<TeamPoints> CalculateTableOnDate(string dateStr, int competitionId)
        {
            var date = DateTime.Parse(dateStr);
            var allTeamsPoints = new List<TeamPoints>();
            using (var db = new sakilaEntities4())
            {
                var allMatches = db.competitionmatch
                    .Where(x => x.CompetitionID == competitionId &&
                    x.MatchDate <= date).ToList();
                Console.WriteLine(allMatches.Count);
                var i = 0;
                foreach (var match in allMatches)
                {
                    Console.WriteLine(i++);
                    if (match.WinnerTeamID != null)
                    {
                        AddPointsToTeam(allTeamsPoints, match.WinnerTeamID.Value, 3);
                    }

                    else
                    {
                        AddPointsToTeam(allTeamsPoints, match.HomeTeamID, 1);
                        AddPointsToTeam(allTeamsPoints, match.AwayTeamID, 1);

                    }
                }
            }

            var path = @"C:\Users\user\Desktop\DataProjects\PremierLeagueTable";
            var ordered = allTeamsPoints.OrderByDescending(x => x.TeamPointsValue).ToList();
            File.WriteAllLines(path + dateStr + ".tsv", ordered.Select(x => x.TeamName + "\t" + x.TeamPointsValue));
            return ordered;

        }
        private static void AddPointsToTeam(List<TeamPoints> allTeamsPoints, int teamId, int pointsToAdd)
        {
            var team = allTeamsPoints.FirstOrDefault(x => x.TeamId == teamId);
            if (team != null)
            {
                team.TeamPointsValue = team.TeamPointsValue + pointsToAdd;
            }

            else
            {
                using (var db = new sakilaEntities4())
                {
                    var team2 = db.team.First(x => x.TeamID == teamId);
                    var newTeam = new TeamPoints();
                    newTeam.TeamName = team2.TeamName;
                    newTeam.TeamId = team2.TeamID;
                    newTeam.TeamPointsValue = pointsToAdd;
                    allTeamsPoints.Add(newTeam);
                }
            }
        }
        public static void DaysInFirstPlace(int competitionID)
        {
            var leaders = new List<Leader>();
            using (var db = new sakilaEntities4())
            {
                var allCompetitionMatches =
                    db.competitionmatch.Where(x => x.CompetitionID == competitionID)
                    .Select(x => x.MatchDate)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                for (int i = 0; i < allCompetitionMatches.Count - 1; i++)
                {
                    var d1 = allCompetitionMatches[i];
                    var d2 = allCompetitionMatches[i + 1];
                    var daysToAdd = (d2 - d1).Days;
                    var table = CalculateTableOnDate(d1.ToLongDateString(), competitionID);
                    var firstTeam = table.First().TeamName;
                    var l = leaders.FirstOrDefault(x => x.TeamName == firstTeam);
                    if (l != null)
                    {
                        l.Days = l.Days + daysToAdd;
                    }
                    else
                    {
                        var newLeader = new Leader();
                        newLeader.TeamName = firstTeam;
                        newLeader.Days = daysToAdd;
                        leaders.Add(newLeader);
                    }
                }
            }

            var toPrint = string.Join("\n",
                leaders.OrderByDescending(x => x.Days).Select(x => x.TeamName + ": " + x.Days));
            Console.Write(toPrint);
        }

        public static double CalculateStdDev(List<int> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
        public static List<competitionmatch> GetAllMatchesForCompetition(int competitionId, sakilaEntities4 db)
        {
            var matches = db.competitionmatch.Where(x => x.CompetitionID == competitionId).ToList();
            return matches;
        }
        public static List<competitionmatch> GetTeamLatesMatches(sakilaEntities4 db, int teamId, int competitionId, int matchesToTake = 50, DateTime? endDate = null)
        {

            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var latestMatches = db.competitionmatch
                        .Where(x => x.CompetitionID == competitionId)
                        .Where(x => x.AwayTeamID == teamId || x.HomeTeamID == teamId)
                        .Where(x => x.MatchDate < endDate.Value)
                        .OrderByDescending(x => x.MatchDate)
                        .Take(matchesToTake)
                        .ToList();

            return latestMatches;
        }
        public static decimal CalculateTeamStrength(int marketValue, decimal sesonalForm, decimal latestForm, decimal homeOrAwayForm)
        {
            return Math.Round(marketValue * (sesonalForm * 0.15m + latestForm * 0.35m + homeOrAwayForm * 0.5m) / 100, 1);
        }
        
        public static decimal CalculateTeamStrength(sakilaEntities4 db, int teamId, string homeOrAway, int competitionId, DateTime date)
        {
            var marketValue = db.team.First(x => x.TeamID == teamId).MarketValue;

            var seasonalBalance = PointsCalculator.GetTeamBalance(db, teamId, competitionId,50, date);
            var seasonalPointsPace = PointsCalculator.CalculatePointPace(seasonalBalance);

            var latestBalance = PointsCalculator.GetTeamBalance(db, teamId, competitionId, 3, date);
            var latestPointsPace = PointsCalculator.CalculatePointPace(latestBalance);


            var homeOrAwayBalance = new TeamBalance();
            if (homeOrAway == "Home")
            {
                homeOrAwayBalance = PointsCalculator.GetTeamBalanceHome(db, teamId, competitionId, 50, date);
            }
            else
            {
                homeOrAwayBalance = PointsCalculator.GetTeamBalanceAway(db, teamId, competitionId, 50, date);
            }

            var homeOrAwayPointsPace = PointsCalculator.CalculatePointPace(homeOrAwayBalance);

            return CalculateTeamStrength(marketValue.Value, seasonalPointsPace, latestPointsPace, homeOrAwayPointsPace);
        }
        public static double StrengthDiffCaculator(decimal teamStrength, decimal oponentStength)
        {
            if (teamStrength <= oponentStength)
            {
                return 0;
            }

            if (oponentStength == 0.0m)
            {
                oponentStength = 0.1m;
            }

            return (double) (teamStrength/oponentStength/5);
        }
        public static string GetExpectedWinnerAccordingToResult(double homeGoals, double awayGoals, out decimal percent, int devide = 2, double minimalGap = 2)
        {
            percent = 0;
            if (homeGoals - minimalGap > awayGoals)
            {
                percent = (decimal) Math.Round((homeGoals - awayGoals)/ devide, 2);
                return "Home";
            }

            if (awayGoals - minimalGap > homeGoals)
            {
                percent = (decimal) Math.Round((awayGoals - homeGoals) / devide, 2);
                return "Away";
            }

            if (homeGoals > awayGoals)
            {
                percent = (decimal) Math.Round(1 - (homeGoals - awayGoals), 2);
            }

            if (awayGoals > homeGoals)
            {
                percent = (decimal) Math.Round(1 - (awayGoals - homeGoals), 2);
            }

            return "Draw";
        }
        public static bool IsTheSameResult(string homeExpectedLetter, string awayExpectedLetter, out string winner)
        {
            winner = "NA";
            if (homeExpectedLetter == "L" && awayExpectedLetter == "W")
            {
                winner = "Away";
                return true;
            }

            if (homeExpectedLetter == "W" && awayExpectedLetter == "L")
            {
                winner = "Home";
                return true;
            }

            if (homeExpectedLetter == awayExpectedLetter)
            {
                winner = "Draw";
            }

            return false;
        }
        public static ResultConfidence GetExpectedResultByLetters(Helper.LetterDistribution homeLetters,
            Helper.LetterDistribution awayLetters)
        {
            string winner;
            if (IsTheSameResult(homeLetters.Letter, awayLetters.Letter, out winner))
            {
                if (homeLetters.Percent >= 0.43m && awayLetters.Percent >= 0.43m)
                {
                    return new ResultConfidence
                    {
                        Confidence = 1m,
                        Winner = winner
                    };
                }

                if (homeLetters.Percent >= 0.43m || awayLetters.Percent >= 0.43m)
                {
                    return new ResultConfidence
                    {
                        Confidence = 0.8m,
                        Winner = winner
                    };
                }

                return new ResultConfidence
                {
                    Confidence = 0.7m,
                    Winner = winner
                };
                
            }

            if (homeLetters.Percent > awayLetters.Percent)
            {
                var confidence = 0.4m;
                if (homeLetters.Percent >= 0.43m && awayLetters.Percent < 0.43m)
                {
                    confidence = 0.6m;
                }

                if (homeLetters.Letter == "W")
                {
                    return new ResultConfidence
                    {
                        Confidence = confidence,
                        Winner = "Home"
                    };
                }

                if (homeLetters.Letter == "L")
                {
                    return new ResultConfidence
                    {
                        Confidence = confidence,
                        Winner = "Away"
                    };
                }
                if (homeLetters.Letter == "D")
                {
                    return new ResultConfidence
                    {
                        Confidence = confidence,
                        Winner = "Draw"
                    };
                }
            }

            if (awayLetters.Percent > homeLetters.Percent)
            {
                var confidence = 0.4m;
                if (awayLetters.Percent >= 0.43m && homeLetters.Percent < 0.43m)
                {
                    confidence = 0.6m;
                }

                if (awayLetters.Letter == "W")
                {
                    return new ResultConfidence
                    {
                        Confidence = confidence,
                        Winner = "Away"
                    };
                }

                if (awayLetters.Letter == "L")
                {
                    return new ResultConfidence
                    {
                        Confidence = confidence,
                        Winner = "Home"
                    };
                }
                if (awayLetters.Letter == "D")
                {
                    return new ResultConfidence
                    {
                        Confidence = confidence,
                        Winner = "Draw"
                    };
                }
            }

            return new ResultConfidence
            {
                Confidence = 0.2m,
                Winner = "Draw"
            };
        }
        public static ResultConfidence GetExpectedWinner(double homeGoals, double awayGoals, int devide = 2, double gap = 1)
        {
            decimal conf;
            var expectedResultByGoals = GetExpectedWinnerAccordingToResult(homeGoals, awayGoals, out conf, devide, gap);
            return new ResultConfidence
            {
                Winner = expectedResultByGoals,
                Confidence = conf
            };
        }
        public static MatchStrengthDetails GetMatchStrengthDetails(sakilaEntities4 db, competitionmatch match, int competitionId)
        {
            var homeTeamStrength = CalculateTeamStrength(db, match.HomeTeamID, "Home", competitionId, match.MatchDate);
            var awayTeamStrength = CalculateTeamStrength(db, match.AwayTeamID, "Away", competitionId, match.MatchDate);
            var winner = "Draw";
            if (match.WinnerTeamID == match.HomeTeamID)
            {
                winner = "Home";
            }

            if (match.WinnerTeamID == match.AwayTeamID)
            {
                winner = "Away";
            }

            return new MatchStrengthDetails
            {
                HomeTeamStrength = homeTeamStrength,
                AwayTeamStength = awayTeamStrength,
                Winner = winner
            };
        }

        public static List<MatchStrengthDetails> GetAllMatchStengthDetailsForCompetition(sakilaEntities4 db,
            int competitionId)
        {
            var result = new List<MatchStrengthDetails>();
            var relevantMatches = db.competitionmatch.Where(x => x.CompetitionID == competitionId)
                .OrderBy(x => x.MatchDate)
                .ToList();
            var i = 0;
            foreach (var match in relevantMatches)
            {
                var matchDetails = GetMatchStrengthDetails(db, match, competitionId);
                Console.WriteLine(i++);
                result.Add(matchDetails);
            }

            return result;
        }


        public static List<StrengthDiff> GetMatchStrengthMapForCompetition(int competitionId)
        {
            var result = new List<StrengthDiff>();
            using (var db = new sakilaEntities4())
            {
                var allStrengthMatches = GetAllMatchStengthDetailsForCompetition(db, competitionId);
                var ordered = allStrengthMatches.GroupBy(x => x.HomeTeamStrength - x.AwayTeamStength)
                    .OrderByDescending(x => x.Count());

                foreach (var strengthBalance in ordered)
                {
                    var winnerGroup = strengthBalance.GroupBy(x => x.Winner);
                    var newItem = new StrengthDiff
                    {
                        Diff = strengthBalance.Key,
                        Winners = winnerGroup.Select(y => new StrengthWinner
                        {
                            Winner = y.Key,
                            Count = y.Count(),
                            Percent = Math.Round((decimal) y.Count()/strengthBalance.Count(), 2)
                        }).ToList()
                    };
                    result.Add(newItem);
                }
            }
            result = result.OrderByDescending(x => x.Winners.FirstOrDefault().Percent * x.Winners.FirstOrDefault().Count).ToList();

            return result;
        }

        public static void PrintMatchStengthMap(int competitionId)
        {
            var path = @"C:\Users\user\Desktop\DataProjects\StengthMap" + "CompetitionID " + competitionId + ".tsv";
            var stengthMap = GetMatchStrengthMapForCompetition(competitionId);
            var linesToWrite = stengthMap.Select(x => x.Diff + ":: " + x.Winners.First().Winner + " (" + x.Winners.Sum(z => z.Count) + ", " + +x.Winners.First().Percent + ")");
            File.WriteAllLines(path, linesToWrite);
        }
        public static AttributesMatch GetAttributeMatch(sakilaEntities4 db, competitionmatch match)
        {
            var attributesDict = SecondaryStatsCalculator.BuildAttributesDictionary(match.CompetitionID, db, 50, match.MatchDate);
            var homeTeamAttributes = Helper.GetTeamAttributeFromDict((int)match.HomeTeamID, attributesDict);
            var awayTeamAttributes = Helper.GetTeamAttributeFromDict((int)match.AwayTeamID, attributesDict);
            List<DataObjects.AttributeType> winnerAttributes = null;
            if (match.WinnerTeamID == match.HomeTeamID)
            {
                winnerAttributes = homeTeamAttributes;
            }
            else if (match.WinnerTeamID == match.AwayTeamID)
            {
                winnerAttributes = awayTeamAttributes;
            }

            return new AttributesMatch
            {
                HomeAttributes = homeTeamAttributes,
                AwayAttributes = awayTeamAttributes,
                WinnerAttributes = winnerAttributes
            };
        }
        public static List<AttributesMatch> GetAttributeMatchesForCompetition(int competitionId)
        {
            using (var db = new sakilaEntities4())
            {

                var allAttributeMatches = db.competitionmatch
                    .Where(x => x.CompetitionID == competitionId)
                    .OrderBy(x => x.MatchDate)
                    .Skip(20)
                    .ToList()
                    .Select(x => GetAttributeMatch(db, x))
                    .ToList();

                return allAttributeMatches;
            }
        }

        public static void PrintAttributesMapForCompetition(int competitioId)
        {
            var linesToWrite = new List<string>();
            var resultPath = @"C:\Users\user\Desktop\DataProjects\FullAttributeMap.tsv";
            var attributeMatches = GetAttributeMatchesForCompetition(competitioId);
            var groups = attributeMatches.GroupBy(x => new {HomeAttr = string.Join(", ", x.HomeAttributes), AwayAttr = string.Join(", ", x.AwayAttributes)})
                .OrderByDescending(x => x.Count())
                .ToList();

            foreach (var g in groups)
            {
                var homeAttrStr = string.Join(", ", g.First().HomeAttributes.Select(x => x.ToString()));
                if (string.IsNullOrWhiteSpace(homeAttrStr))
                {
                    homeAttrStr = "None";
                }
                var awayAttrStr = string.Join(", ", g.First().AwayAttributes.Select(x => x.ToString()));
                if (string.IsNullOrWhiteSpace(awayAttrStr))
                {
                    awayAttrStr = "None";
                }
                var winnerAttrStr = "Draw";
                var winnersGroup = g
                    .GroupBy(x => x.WinnerAttributes)
                    .OrderByDescending(x => x.Count())
                    .Select(x => x.First())
                    .ToList();
                if (winnersGroup.Any())
                {
                    if (winnersGroup.First().WinnerAttributes != null)
                    {
                        winnerAttrStr = string.Join(", ", winnersGroup.First().WinnerAttributes.Select(x => x.ToString()));
                        if (string.IsNullOrWhiteSpace(winnerAttrStr))
                        {
                            winnerAttrStr = "None";
                        }
                    }
                }

                linesToWrite.Add(homeAttrStr + "::: " + awayAttrStr + "::: " + winnerAttrStr + "(" + g.Count() + ")");
            }

            File.WriteAllLines(resultPath, linesToWrite);
        }

        public static List<AttributesMatch> BuildAttributeMatchMap(List<int> competitionIDs = null)
        {
            if (competitionIDs == null)
            {
                competitionIDs = new List<int>
                {
                    2,3,8
                };
            }
            var fullMap = new List<AttributesMatch>();
            foreach (var competitionId in competitionIDs)
            {
                Console.WriteLine($"Building attribute clash map for competitionID: {competitionId}");
                var attributeMatches = GetAttributeMatchesForCompetition(competitionId);
                fullMap.AddRange(attributeMatches);
            }

            return fullMap;
        }
        public static string GetAttributesClashResult(DataObjects.AttributeType homeAttr,
            DataObjects.AttributeType awayAttr, List<AttributesMatch> fullMap, out double percent, out int cnt)
        {
            var relevantLines =
                fullMap.Where(x => x.HomeAttributes.Contains(homeAttr) && x.AwayAttributes.Contains(awayAttr)).ToList();

            var homeAttrWinner = relevantLines.Where(x => x.WinnerAttributes != null).Count(x => x.WinnerAttributes.Contains(homeAttr));
            var awayAttrWinner = relevantLines.Where(x => x.WinnerAttributes != null).Count(x => x.WinnerAttributes.Contains(awayAttr));
            var draws = relevantLines.Count - homeAttrWinner - awayAttrWinner;
            cnt = relevantLines.Count;

            if ((draws > homeAttrWinner && draws > awayAttrWinner) || homeAttrWinner == awayAttrWinner)
            {
                percent = (double)draws/relevantLines.Count;
                percent = Math.Round(percent, 2);
                return "Draw";
            }

            if (homeAttrWinner > awayAttrWinner)
            {
                percent = (double)homeAttrWinner/relevantLines.Count;
                percent = Math.Round(percent, 2);
                return "Home";
            }

            if (awayAttrWinner > homeAttrWinner)
            {
                percent = (double)awayAttrWinner / relevantLines.Count;
                percent = Math.Round(percent, 2);
                return "Away";
            }

            percent = 0;
            return "None";
        }


        public static List<string> GetAttributeAdventageList(List<DataObjects.AttributeType> homeAttrs,
    List<DataObjects.AttributeType> awayAttrs, List<AttributesMatch> fullMap)
        {
            var linesToWrite = new List<string>();
            var clashingReaultList = new List<ClashingResults>();
            foreach (var attr in homeAttrs)
            {
                if (awayAttrs.Contains(attr))
                {
                    continue;
                }

                foreach (var awayAttr in awayAttrs)
                {
                    double percent;
                    int count;
                    var clashingResult = GetAttributesClashResult(attr, awayAttr, fullMap, out percent, out count);
                    if (percent > 0)
                    {
                        linesToWrite.Add(clashingResult + " has adventage due to clashing of home teams's "
                            + attr.ToString().Replace("_", " ") + " and away team's attr " +
                            awayAttr.ToString().Replace("_", " ") + " in significance of " + percent + "%" + " (" + count + ")");
                    }

                    if (percent >= 0.6)
                    {
                        var newItem = new ClashingResults
                        {
                            Percent = percent,
                            Winner = clashingResult
                        };

                        clashingReaultList.Add(newItem);
                    }
                }
            }

            if (clashingReaultList.Any())
            {
                var g = clashingReaultList.GroupBy(x => x.Winner);
                if (g.Count() == 1 && g.First().Count() > 1)
                {
                    linesToWrite.Add("Team Attributes Winner: " + clashingReaultList.First().Winner + " (" + clashingReaultList.OrderByDescending(x => x.Percent).First().Percent * 100 + "%)");
                }

                else
                {
                    linesToWrite.Add("Team Attributes Winner: None");
                }
            }

            else
            {
                linesToWrite.Add("Team Attributes Winner: None");
            }

            return linesToWrite;
        }

        public static ClashingResults GetAttributeAdventageWinner(List<DataObjects.AttributeType> homeAttrs,
            List<DataObjects.AttributeType> awayAttrs, List<AttributesMatch> fullMap)
        {
            var returnVal = new ClashingResults();
            var linesToWrite = new List<string>();
            var clashingReaultList = new List<ClashingResults>();
            foreach (var attr in homeAttrs)
            {
                if (awayAttrs.Contains(attr))
                {
                    continue;
                }

                foreach (var awayAttr in awayAttrs)
                {
                    double percent;
                    int count;
                    var clashingResult = GetAttributesClashResult(attr, awayAttr, fullMap, out percent, out count);
                    if (percent > 0)
                    {
                        linesToWrite.Add(clashingResult + " has adventage due to clashing of home teams's "
                            + attr.ToString().Replace("_", " ") + " and away team's attr " + 
                            awayAttr.ToString().Replace("_", " ") + " in significance of " + percent + "%" + " ("+ count +")");
                    }

                    if (percent >= 0.6)
                    {
                        var newItem = new ClashingResults
                        {
                            Percent = percent,
                            Winner = clashingResult
                        };

                        clashingReaultList.Add(newItem);
                    }
                }
            }

            if (clashingReaultList.Any())
            {
                var g = clashingReaultList.GroupBy(x => x.Winner);
                if (g.Count() == 1 && g.First().Count() > 1)
                {
                    returnVal = new ClashingResults
                    {
                        Winner = clashingReaultList.First().Winner,
                        Percent = clashingReaultList.OrderByDescending(x => x.Percent).First().Percent * 100
                    };
                }

                else
                {
                    linesToWrite.Add("Team Attributes Winner: None");
                }
            }

            else
            {
                linesToWrite.Add("Team Attributes Winner: None");
            }

            return returnVal;
        }
        public class AttributesMatch
        {
            public List<DataObjects.AttributeType> HomeAttributes;
            public List<DataObjects.AttributeType> AwayAttributes;
            public List<DataObjects.AttributeType> WinnerAttributes;

        }
        public class MatchStrengthDetails
        {
            public decimal HomeTeamStrength;
            public decimal AwayTeamStength;
            public string Winner;
        }
        public class StrengthDiff
        {
            public decimal Diff;
            public List<StrengthWinner> Winners;
        }
        public class ClashingResults
        {
            public string Winner;
            public double Percent;
        }
        public class StrengthWinner
        {
            public string Winner;
            public int Count;
            public decimal Percent;
        }
        public class LettersWinner
        {
            public string Winner;
            public double CountAverage;
            public decimal Percent;
        }
        public class PositionGoals
        {
            public string Position;
            public int Goals;
        }


        public class TeamPoints
        {
            public int TeamId;
            public string TeamName;
            public int TeamPointsValue;
        }
        public class Leader
        {
            public int Days;
            public string TeamName;
        }
        public class TeamStdDevAndAverage
        {
            public string TeamName;
            public double StdDev;
            public double Average;
        }
        public class ResultConfidence
        {
            public string Winner;
            public decimal Confidence;
        }
        public class Recommendation
        {
            public string Type;
            public string Result;
            public decimal Confidence;
            public string HomeTeam;
            public string AwayTeam;
        }

        public static LettersWinner GetLettersExpectedWinner(Helper.LetterDistribution homeLetters,
            Helper.LetterDistribution awayLetters)
        {
            var decimalList = new List<Helper.LetterDistribution>
            {
                awayLetters,
                homeLetters
            };

            var averageCount = decimalList.Select(x => x.Count).Average();
            string winner;

            if (IsTheSameResult(homeLetters.Letter, awayLetters.Letter, out winner))
            {
                var conf = Math.Round(decimalList.Select(x => x.Percent).Sum() * 100);
                return new LettersWinner
                {
                    CountAverage = averageCount,
                    Percent = conf,
                    Winner = winner
                };
            }

            var higherPercent = decimalList.OrderByDescending(x => x.Percent).First();
            if (higherPercent.Percent == homeLetters.Percent)
            {
                return new LettersWinner
                {
                    CountAverage = averageCount,
                    Percent = Math.Round(higherPercent.Percent * 100),
                    Winner = "Home"
                };
            }

            return new LettersWinner
            {
                CountAverage = averageCount,
                Percent = Math.Round(higherPercent.Percent * 100),
                Winner = "Away"
            };
        }

        public static string GetLettersExpectedWinners(Helper.LetterDistribution homeLetters, Helper.LetterDistribution awayLetters)
        {
            var decimalList = new List<Helper.LetterDistribution>
            {
                awayLetters,
                homeLetters
            };
            string winner;

            if (IsTheSameResult(homeLetters.Letter, awayLetters.Letter, out winner))
            {
                var conf = Math.Round(decimalList.Select(x => x.Percent).Sum() * 100);
                return winner + "(" + conf + "%)";
            }

            var higherPercent = decimalList.OrderByDescending(x => x.Percent).First();
            if (higherPercent.Percent == homeLetters.Percent)
            {
                return "Home (" + Math.Round(higherPercent.Percent * 100) + "%)";
            }

            return "Away (" + Math.Round(higherPercent.Percent * 100) + "%)";

            if (IsTheSameResult(homeLetters.Letter, awayLetters.Letter, out winner))
            {
                if (homeLetters.Percent >= 0.43m && awayLetters.Percent >= 0.43m)
                {
                    return winner + " (100%)";
                }

                if (homeLetters.Percent >= 0.43m || awayLetters.Percent >= 0.43m)
                {
                    return winner + " (80%)";

                }

                return winner + " (70%)";
            }

            if (homeLetters.Percent > awayLetters.Percent)
            {
                var confidence = 0.4m;
                if (homeLetters.Percent >= 0.43m && awayLetters.Percent < 0.43m)
                {
                    confidence = 0.6m;
                }

                if (homeLetters.Letter == "W")
                {

                    return "Home (" + confidence * 100 + "%)";
                }

                if (homeLetters.Letter == "L")
                {
                    return "Away (" + confidence * 100 + "%)";

                }
                if (homeLetters.Letter == "D")
                {
                    return "Draw (" + confidence * 100 + "%)";
                }
            }

            if (awayLetters.Percent > homeLetters.Percent)
            {
                var confidence = 0.4m;
                if (awayLetters.Percent >= 0.43m && homeLetters.Percent < 0.43m)
                {
                    confidence = 0.6m;
                }

                if (awayLetters.Letter == "W")
                {
                    return "Away (" + confidence * 100 + "%)";

                }

                if (awayLetters.Letter == "L")
                {
                    return "Home (" + confidence * 100 + "%)";

                }
                if (awayLetters.Letter == "D")
                {
                    return "Draw (" + confidence * 100 + "%)";

                }
            }

            return "None";
        }

        public static string CalculateConversionWinner(decimal homeExpectedConversionRate, decimal awayExpectedConversionRate)
        {
            if (homeExpectedConversionRate < awayExpectedConversionRate)
            {
                var diff = Math.Round(homeExpectedConversionRate - awayExpectedConversionRate, 2);
                return "Conversion Winner: Home (" + diff*30*-1 + ")";
            }

            if (awayExpectedConversionRate < homeExpectedConversionRate)
            {
                var diff = Math.Round(awayExpectedConversionRate- homeExpectedConversionRate, 2);
                return "Conversion Winner: Away (" + diff * 30*-1 + ")";
            }

            return "Conversion Winner: None";
        }

        public static Dictionary<string, List<string>> ExtractSimiliarTeamsDict()
        {
            var retVal = new Dictionary<string, List<string>>();

            var possessionPath = @"C:\Users\user\Desktop\DataProjects\EventTablePossession.tsv";
            var foulsPath = @"C:\Users\user\Desktop\DataProjects\EventTableFouls.tsv";
            var accurateInFrontOfGoalPath = @"C:\Users\user\Desktop\DataProjects\EventTableAccuracyInFrontOfGoal.tsv";
            var accurateInFrontOfGoalAgainstPath = @"C:\Users\user\Desktop\DataProjects\EventTableAccuracyInFrontOfGoalAgainst.tsv";
            var shotsToTargetPath = @"C:\Users\user\Desktop\DataProjects\EventTableShotsOnTarget.tsv";

            var possessionFile = File.ReadAllLines(possessionPath).ToNamesAndPlaces();
            var possessionOrdered = possessionFile.OrderBy(x => x.Value).Select(x => x.Key).ToList();

            var foulsFile = File.ReadAllLines(foulsPath).ToNamesAndPlaces();
            var foulsOrdered = foulsFile.OrderBy(x => x.Value).Select(x => x.Key).ToList();

            var accurateInFrontOfGoalFile = File.ReadAllLines(accurateInFrontOfGoalPath).ToNamesAndPlaces();
            var accurateInFrontOfGoalOrdered = accurateInFrontOfGoalFile.OrderBy(x => x.Value).Select(x => x.Key).ToList();

            var accurateInFrontOfGoalAgainstFile = File.ReadAllLines(accurateInFrontOfGoalAgainstPath).ToNamesAndPlaces();
            var accurateInFrontOfGoalAgainstOrdered = accurateInFrontOfGoalAgainstFile.OrderBy(x => x.Value).Select(x => x.Key).ToList();

            var shotsToTargetFile = File.ReadAllLines(shotsToTargetPath).ToNamesAndPlaces();
            var shotsToTargetOrdered = shotsToTargetFile.OrderBy(x => x.Value).Select(x => x.Key).ToList();
            var placesToTake = 6;

            foreach (var team in possessionFile)
            {
                var possessionCloseTeams = Helper.GetNearestItems(possessionOrdered, team.Value, placesToTake);

                var foulsPlace = foulsFile.First(x => x.Key == team.Key).Value;
                var foulsCloseTeams = Helper.GetNearestItems(foulsOrdered, foulsPlace, placesToTake);

                var accurateInFrontOfGoalPlace = accurateInFrontOfGoalFile.First(x => x.Key == team.Key).Value;
                var accurateInFrontOfGoalTeams = Helper.GetNearestItems(accurateInFrontOfGoalOrdered, accurateInFrontOfGoalPlace, placesToTake);

                var accurateInFrontOfGoalAgainstPlace = accurateInFrontOfGoalAgainstFile.First(x => x.Key == team.Key).Value;
                var accurateInFrontOfGoalAgainstTeams = Helper.GetNearestItems(accurateInFrontOfGoalAgainstOrdered, accurateInFrontOfGoalAgainstPlace, placesToTake);

                var shotsToTargetPlace = shotsToTargetFile.First(x => x.Key == team.Key).Value;
                var shotsToTargetTeams = Helper.GetNearestItems(shotsToTargetOrdered, shotsToTargetPlace, placesToTake);

                var allCloseTeams = possessionCloseTeams;
                allCloseTeams.AddRange(foulsCloseTeams);
                allCloseTeams.AddRange(accurateInFrontOfGoalTeams);
                allCloseTeams.AddRange(shotsToTargetTeams);
                allCloseTeams.AddRange(accurateInFrontOfGoalAgainstTeams);

                var closeTeams = allCloseTeams.GroupBy(x => x).Where(x => x.Count() > 2).Select(x => x.First()).Where(x => x != team.Key).ToList();

                retVal.Add(team.Key, closeTeams);
            }

            return retVal;
        }

        public static void PrineSimiliarTeamFile()
        {
            var resultFile = @"C:\Users\user\Desktop\DataProjects\SimiliarTeamsDict.tsv";
            var teamsDict = ExtractSimiliarTeamsDict();
            var linesToWrite = new List<string>();

            foreach (var team in teamsDict)
            {
                var similiarTeamsString = string.Join(",", team.Value);
                linesToWrite.Add(team.Key + "\t" + similiarTeamsString);
            }

            File.WriteAllLines(resultFile, linesToWrite);
        }

        public static void RichestTeamsWins()
        {
            var resultFile = @"C:\Users\user\Desktop\DataProjects\RichestTeamTest.tsv";
            using (var db = new sakilaEntities4())
            {
                var allMatches = db.competitionmatch.Where(x => x.CompetitionID == 3).ToList();
                var relevantMatches = allMatches.Where(x => x.WinnerTeamID != null);
                var cnt = 0;
                var notParticipated = 0;
                foreach (var match in relevantMatches)
                {
                    var winnerTeamID = match.WinnerTeamID;
                    var loserTeamID = 0;
                    if (match.HomeTeamID != winnerTeamID.Value)
                    {
                        loserTeamID = match.HomeTeamID;
                    }
                    else
                    {
                        loserTeamID = match.AwayTeamID;
                    }

                    var winnerTeam = db.team.First(x => x.TeamID == winnerTeamID.Value);
                    var loserTeam = db.team.First(x => x.TeamID == loserTeamID);

                    if (Math.Abs((int) (winnerTeam.MarketValue - loserTeam.MarketValue)) < 50)
                    {
                        notParticipated++;
                        continue;
                    }

                    if (winnerTeam.MarketValue > loserTeam.MarketValue + 50)
                    {
                        cnt++;
                    }
                }

                var percent = Math.Round((double) ((decimal)cnt/(allMatches.Count() - notParticipated)*100), 2);
                var stringToWrite =
                    $"Out of {allMatches.Count - notParticipated} so far, the reachest team won {cnt} out of them, which is {percent}%";
                File.WriteAllText(resultFile, stringToWrite);
            }
        }
    }
}
