using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProjects
{
    public class MatchReporter
    {
        public static void CreateMatchReport(Dictionary<string, List<Helper.LetterDistribution>> lettersDict,
            Dictionary<int, List<DataObjects.AttributeType>> attributesDict,
            List<MainCalculator.AttributesMatch> attributeClashingMap,
            string homeTeam, string awayTeam, int competitionId)
        {
            var linesToWrite = new List<string>
            {
                "Home Team: " + homeTeam,
                "Away Team: " + awayTeam + "\n" 
            };
            var path = @"C:\Users\user\Desktop\DataProjects\" + homeTeam + "VS" + awayTeam + ".tsv";
            using (var db = new sakilaEntities4())
            {
                var homeTeamObj = db.team.First(x => x.TeamName == homeTeam);
                var awayTeamObj = db.team.First(x => x.TeamName == awayTeam);

                linesToWrite.Add(homeTeam + " Market Value: " + homeTeamObj.MarketValue);
                linesToWrite.Add(awayTeam + " Market Value: " + awayTeamObj.MarketValue + "\n");

                var homeTeamBalance = PointsCalculator.GetTeamBalance(db, homeTeamObj.TeamID, competitionId);
                var awayTeamBalance = PointsCalculator.GetTeamBalance(db, awayTeamObj.TeamID, competitionId);

                var homeTeamBalanceLast3 = PointsCalculator.GetTeamBalance(db, homeTeamObj.TeamID, competitionId, 3);
                var awayTeamBalanceLast3 = PointsCalculator.GetTeamBalance(db, awayTeamObj.TeamID, competitionId, 3);

                var teamBalanceAtHome = PointsCalculator.GetTeamBalanceHome(db, homeTeamObj.TeamID, competitionId);
                var teamBalanceAwayFromHome = PointsCalculator.GetTeamBalanceAway(db, awayTeamObj.TeamID, competitionId);

                var homeTeamSeasonForm = PointsCalculator.CalculatePointPace(homeTeamBalance);
                var awayTeamSeasonForm = PointsCalculator.CalculatePointPace(awayTeamBalance);

                var homeTeamLatest3Form = PointsCalculator.CalculatePointPace(homeTeamBalanceLast3);
                var awayTeamLatest3Form = PointsCalculator.CalculatePointPace(awayTeamBalanceLast3);

                var homeTeamSeasonFormAtHome = PointsCalculator.CalculatePointPace(teamBalanceAtHome);
                var awayTeamSeasonFormAway = PointsCalculator.CalculatePointPace(teamBalanceAwayFromHome);

                var foulsAverageHome = SecondaryStatsCalculator.GetAverageFoulsPerTeam(db, homeTeamObj.TeamID, competitionId);
                var foulsAverageAway = SecondaryStatsCalculator.GetAverageFoulsPerTeam(db, awayTeamObj.TeamID, competitionId);

                var cornersAverageHome = SecondaryStatsCalculator.GetAverageCornersPerTeam(db, homeTeamObj.TeamID, competitionId);
                var cornersAverageAway = SecondaryStatsCalculator.GetAverageCornersPerTeam(db, awayTeamObj.TeamID, competitionId);
                var cornersAverageOnHomeTeamMatches = SecondaryStatsCalculator.AverageCornersOnTeamsMatches(db, homeTeamObj.TeamID,
                    competitionId);
                var cornersAverageOnAwayTeamMatches = SecondaryStatsCalculator.AverageCornersOnTeamsMatches(db, awayTeamObj.TeamID,
                        competitionId);
                var cornersStdOnHomeTeamMatches = SecondaryStatsCalculator.GetStdCornersPerTeamsMatches(db, homeTeamObj.TeamID,
                            competitionId);
                var cornersStdOnAwayTeamMatches = SecondaryStatsCalculator.GetStdCornersPerTeamsMatches(db, awayTeamObj.TeamID,
                        competitionId);

                var shotsOnTargetAverageHome = SecondaryStatsCalculator.GetAverageTotalShotsPerTeam(db, homeTeamObj.TeamID, competitionId);
                var shotsOnTargetAverageAway = SecondaryStatsCalculator.GetAverageTotalShotsPerTeam(db, awayTeamObj.TeamID, competitionId);

                var shotsOnTargetAverageAgainstHome = SecondaryStatsCalculator.GetAverageShotsOnTargetAgainstTeam(db, homeTeamObj.TeamID, competitionId);
                var shotsOnTargetAverageAgainstAway = SecondaryStatsCalculator.GetAverageShotsOnTargetAgainstTeam(db, awayTeamObj.TeamID, competitionId);

                var possessionAverageHome = SecondaryStatsCalculator.GetAveragePossessionPerTeam(db, homeTeamObj.TeamID, competitionId);
                var possessionAverageAway = SecondaryStatsCalculator.GetAveragePossessionPerTeam(db, awayTeamObj.TeamID, competitionId);

                var possessionAverageAgainstHome = SecondaryStatsCalculator.GetAveragePossessionAgainstTeam(db, homeTeamObj.TeamID, competitionId);
                var possessionAverageAgainstAway = SecondaryStatsCalculator.GetAveragePossessionAgainstTeam(db, awayTeamObj.TeamID, competitionId);

                var possToShotsRateHome = Math.Round((double) (possessionAverageHome/shotsOnTargetAverageHome), 2);
                var possToShotsRateAway = Math.Round((double) (possessionAverageAway/shotsOnTargetAverageAway), 2);

                var possToShotsRateAgainstHome = Math.Round((double)(possessionAverageAgainstHome / shotsOnTargetAverageAgainstHome), 2);
                var possToShotsRateAgainstAway = Math.Round((double)(possessionAverageAgainstAway / shotsOnTargetAverageAgainstAway), 2);

                var goalScoredHomeSeasonal = GoalsCalculator.GetGoalsScoringAverage(db, homeTeamObj.TeamID, competitionId);
                var goalScoredHomeLatest3 = GoalsCalculator.GetGoalsScoringAverage(db, homeTeamObj.TeamID, competitionId, 3);
                var goalScoredHomeAtHome = GoalsCalculator.GetGoalsScoringAverageAtHome(db, homeTeamObj.TeamID,
                    competitionId);

                var goalScoredAwaySeasonal = GoalsCalculator.GetGoalsScoringAverage(db, awayTeamObj.TeamID, competitionId);
                var goalScoredAwayLatest3 = GoalsCalculator.GetGoalsScoringAverage(db, awayTeamObj.TeamID, competitionId, 3);
                var goalScoredAwayAtAway = GoalsCalculator.GetGoalsScoringAverageAtAway(db, awayTeamObj.TeamID,
                    competitionId);

                var shotsToGoalsRateHome = Math.Round((double) (shotsOnTargetAverageHome/goalScoredHomeSeasonal.Average), 2);
                var shotsToGoalRateAway = Math.Round((double) (shotsOnTargetAverageAway/goalScoredAwaySeasonal.Average), 2);

                var homeTeamTopScorer = GoalsCalculator.GetTeamTopScorer(db, homeTeamObj.TeamID, competitionId);
                var awayTeamTopScorer = GoalsCalculator.GetTeamTopScorer(db, awayTeamObj.TeamID, competitionId);

                var goalConcededHomeSeasonal = GoalsCalculator.GetGoalsConcededAverage(db, homeTeamObj.TeamID, competitionId);
                var goalConcededHomeLatest3 = GoalsCalculator.GetGoalsConcededAverage(db, homeTeamObj.TeamID, competitionId, 3);
                var goalConcededHomeAtHome = GoalsCalculator.GetGoalsConcededAverageAtHome(db, homeTeamObj.TeamID, competitionId);

                var goalConcededAwaySeasonal = GoalsCalculator.GetGoalsConcededAverage(db, awayTeamObj.TeamID, competitionId);
                var goalConcededAwayLatest3 = GoalsCalculator.GetGoalsConcededAverage(db, awayTeamObj.TeamID, competitionId, 3);
                var goalConcededAwayAtAway = GoalsCalculator.GetGoalsConcededAverageAtAway(db, awayTeamObj.TeamID, competitionId);

                var shotsToGoalsRateAgainstHome = Math.Round((double)(shotsOnTargetAverageAgainstHome / goalConcededHomeSeasonal.Average), 2);
                var shotsToGoalRateAgainstAway = Math.Round((double)(shotsOnTargetAverageAgainstAway / goalConcededAwaySeasonal.Average), 2);

                var expectShotsHome = Math.Round((double)(shotsOnTargetAverageAgainstAway + shotsOnTargetAverageHome)/2, 2);
                var expectedShotsAway = Math.Round((double) (shotsOnTargetAverageAgainstHome + shotsOnTargetAverageAway)/2, 2);

                var homeExpectedConversionRate = Math.Round((decimal)(shotsToGoalRateAgainstAway + shotsToGoalsRateHome) / 2, 2);
                var awayExpectedConversionRate = Math.Round((decimal)(shotsToGoalRateAway + shotsToGoalsRateAgainstHome) / 2, 2);

                var homeExpectedGoalsByConversionRate = expectShotsHome/(double) homeExpectedConversionRate;
                var awayExpectedGoalsByConversionRate = expectedShotsAway/(double) awayExpectedConversionRate;

                var expectedWinnerByConversion = MainCalculator.GetExpectedWinner(homeExpectedGoalsByConversionRate, awayExpectedGoalsByConversionRate, 1, 0.5d);

                var conversionExpectedWinner = MainCalculator.CalculateConversionWinner(homeExpectedConversionRate,
                    awayExpectedConversionRate);

                var goalHomeConcededPosition = GoalsCalculator.GetTeamTopScorersAgainsePosition(db, homeTeamObj.TeamID,
                    competitionId);

                var goalAwayConcededPosition = GoalsCalculator.GetTeamTopScorersAgainsePosition(db, awayTeamObj.TeamID,
                 competitionId);

                var homeTeamFormLetters = LettersSequenceCalculator.GetTeamLatestSequence(db, homeTeamObj.TeamID, competitionId);
                var awayTeamFormLetters = LettersSequenceCalculator.GetTeamLatestSequence(db, awayTeamObj.TeamID, competitionId);

                var homeTeamStrength = MainCalculator.CalculateTeamStrength((int)homeTeamObj.MarketValue, homeTeamSeasonForm, homeTeamLatest3Form, homeTeamSeasonFormAtHome);
                var awayTeamStrength = MainCalculator.CalculateTeamStrength((int)awayTeamObj.MarketValue, awayTeamSeasonForm, awayTeamLatest3Form, awayTeamSeasonFormAway);

                var homeTeamExpectedGoals = GoalsCalculator.CalculateExpectedGoals(goalScoredHomeSeasonal,
                    goalScoredHomeLatest3, goalConcededAwaySeasonal, goalConcededAwayLatest3,
                    goalScoredHomeAtHome, goalConcededAwayAtAway);

                var awayTeamExpectedGoals = GoalsCalculator.CalculateExpectedGoals(goalScoredAwaySeasonal,
                    goalScoredAwayLatest3, goalConcededHomeSeasonal, goalConcededHomeLatest3,
                    goalScoredAwayAtAway, goalConcededHomeAtHome);

                var homeTeamStengthDiff = MainCalculator.StrengthDiffCaculator(homeTeamStrength, awayTeamStrength);
                var awayTeamStengthDiff = MainCalculator.StrengthDiffCaculator(awayTeamStrength, homeTeamStrength);

                string homeTeamSeq;
                var homeSequenceExpectation = LettersSequenceCalculator.BuildSequenceStringExpectation(db, homeTeamObj.TeamID,
                    competitionId, lettersDict, out homeTeamSeq);

                var homeSequenceStringExpectation =
                    LettersSequenceCalculator.BuildSequenceStringExpectation(homeSequenceExpectation, homeTeamSeq);

                string awayTeanSeq;
                var awaySequenceExpectation = LettersSequenceCalculator.BuildSequenceStringExpectation(db, awayTeamObj.TeamID,
                    competitionId, lettersDict, out awayTeanSeq);

                var awaySequenceStringExpectation =
                  LettersSequenceCalculator.BuildSequenceStringExpectation(awaySequenceExpectation, awayTeanSeq);

                var homeCalculatedGoals = Math.Round(homeTeamExpectedGoals.Average + homeTeamStengthDiff, 2);
                var awayCalculatedGoals = Math.Round(awayTeamExpectedGoals.Average + awayTeamStengthDiff, 2);
                var expectedWinner = MainCalculator.GetExpectedWinner(homeCalculatedGoals, awayCalculatedGoals);
                var lettersExpectedWinner = MainCalculator.GetLettersExpectedWinners(homeSequenceExpectation,
                    awaySequenceExpectation);

                var homeTeamAttributes = Helper.GetTeamAttributesList((int) homeTeamObj.TeamID, attributesDict);
                var awayTeamAttributes = Helper.GetTeamAttributesList((int)awayTeamObj.TeamID, attributesDict);

                linesToWrite.Add(homeTeam + " Strength: " + homeTeamStrength);
                linesToWrite.Add(awayTeam + " Strength: " + awayTeamStrength + "\n");

                linesToWrite.Add("Strength Expected Winner: " + expectedWinner.Winner + " (" + Math.Min(expectedWinner.Confidence * 100, 100) +  "%)");
                linesToWrite.Add("Expected Goals: " + Math.Round(homeTeamExpectedGoals.Average + awayTeamExpectedGoals.Average, 2) + " (Average Std: " + Math.Round((homeTeamExpectedGoals.StdDev + awayTeamExpectedGoals.StdDev) / 2, 1) + ")");
                linesToWrite.Add("Expected Result: " + homeCalculatedGoals + ":" + awayCalculatedGoals + "\n");

                linesToWrite.Add(homeTeam + " Expected Result According to Form: " + homeSequenceStringExpectation);
                linesToWrite.Add(awayTeam + " Expected Result According to Form: " + awaySequenceStringExpectation + "\n" + "\n");
                linesToWrite.Add("Letters Expected Results: " + lettersExpectedWinner);

                if (homeTeamAttributes.Any() || awayTeamAttributes.Any())

                {
                    linesToWrite.Add("---------Tips---------");

                    if (homeTeamAttributes.Any())
                    {
                        linesToWrite.Add(homeTeam + ":");
                        linesToWrite.AddRange(homeTeamAttributes);
                        linesToWrite.Add("");
                    }

                    if (awayTeamAttributes.Any())
                    {
                        linesToWrite.Add(awayTeam + ":");
                        linesToWrite.AddRange(awayTeamAttributes);
                        linesToWrite.Add("");

                    }
                    linesToWrite.Add("");

                    var homeTeamAttrs = Helper.GetTeamAttributeFromDict(homeTeamObj.TeamID, attributesDict);
                    var awayTeamAttrs = Helper.GetTeamAttributeFromDict(awayTeamObj.TeamID, attributesDict);
                    var attributeAdventage = MainCalculator.GetAttributeAdventageList(homeTeamAttrs,
                        awayTeamAttrs, attributeClashingMap);
                    if (attributeAdventage.Any())
                    {
                        linesToWrite.Add("Atributes Adventages: ");
                        linesToWrite.Add("");
                    }
                    linesToWrite.AddRange(attributeAdventage);
                    linesToWrite.Add("");
                }

                linesToWrite.Add("---------Balance & Points---------");
                linesToWrite.Add(homeTeam + " Balance: " + homeTeamBalance.Win + "-" + homeTeamBalance.Draw + "-" + homeTeamBalance.Lost + "(Pace: " + homeTeamSeasonForm + ")");
                linesToWrite.Add(homeTeam + " Balance at Home: " + teamBalanceAtHome.Win + "-" + teamBalanceAtHome.Draw + "-" + teamBalanceAtHome.Lost + "(Pace: " + homeTeamSeasonFormAtHome + ")");
                linesToWrite.Add(homeTeam + " Balance Latest 3: " + homeTeamBalanceLast3.Win + "-" + homeTeamBalanceLast3.Draw + "-" + homeTeamBalanceLast3.Lost + "(Pace: " + homeTeamLatest3Form + ")");
                linesToWrite.Add(homeTeam + " Form: " + homeTeamFormLetters + "\n");

                linesToWrite.Add(awayTeam + " Balance: " + awayTeamBalance.Win + "-" + awayTeamBalance.Draw + "-" + awayTeamBalance.Lost + "(Pace:" + awayTeamSeasonForm + ")");
                linesToWrite.Add(awayTeam + " Balance Away: " + teamBalanceAwayFromHome.Win + "-" + teamBalanceAwayFromHome.Draw + "-" + teamBalanceAwayFromHome.Lost + "(Pace: " + awayTeamSeasonFormAway + ")");
                linesToWrite.Add(awayTeam + " Balance Latest 3: " + awayTeamBalanceLast3.Win + "-" + awayTeamBalanceLast3.Draw + "-" + awayTeamBalanceLast3.Lost + "(Pace:" + awayTeamLatest3Form + ")");
                linesToWrite.Add(awayTeam + " Form: " + awayTeamFormLetters + "\n");

                linesToWrite.Add("---------Goals---------");
                linesToWrite.Add(homeTeam + " Goal Scored Average: " + goalScoredHomeSeasonal.Average + "(Std: " + goalScoredHomeSeasonal.StdDev + ")");
                linesToWrite.Add(homeTeam + " Goal Scored Latest 3 Average: " + goalScoredHomeLatest3.Average + "(Std: " + goalScoredHomeLatest3.StdDev + ")");
                linesToWrite.Add(homeTeam + " Goal Scored At Home Matchs: " + goalScoredHomeAtHome.Average + "(Std: " + goalScoredHomeAtHome.StdDev + ")" + "\n");


                linesToWrite.Add(awayTeam + " Goal Scored Average: " + goalScoredAwaySeasonal.Average + "(Std: " + goalScoredAwaySeasonal.StdDev + ")");
                linesToWrite.Add(awayTeam + " Goal Scored Latest 3 Average: " + goalScoredAwayLatest3.Average + "(Std: " + goalScoredAwayLatest3.StdDev + ")");
                linesToWrite.Add(awayTeam + " Goal Scored At Away Matches: " + goalScoredAwayAtAway.Average + "(Std: " + goalScoredAwayAtAway.StdDev + ")" + "\n");

                linesToWrite.Add(homeTeam + " Top Scorer: " + homeTeamTopScorer.Name + " (" + homeTeamTopScorer.Goals + ")");
                linesToWrite.Add(awayTeam + " Top Scorer: " + awayTeamTopScorer.Name + " (" + awayTeamTopScorer.Goals + ")" + "\n");

                linesToWrite.Add(homeTeam + " Goal Conceded Average: " + goalConcededHomeSeasonal.Average + "(Std: " + goalConcededHomeSeasonal.StdDev + ")");
                linesToWrite.Add(homeTeam + " Goal Conceded Latest 3 Average: " + goalConcededHomeLatest3.Average + "(Std: " + goalConcededHomeLatest3.StdDev + ")");
                linesToWrite.Add(homeTeam + " Goal Conceded At Home Matches: " + goalConcededHomeAtHome.Average + "(Std: " + goalConcededHomeAtHome.StdDev + ")" + "\n");

                linesToWrite.Add(awayTeam + " Goal Conceded Average: " + goalConcededAwaySeasonal.Average + "(Std: " + goalConcededAwaySeasonal.StdDev + ")");
                linesToWrite.Add(awayTeam + " Goal Conceded Latest 3 Average: " + goalConcededAwayLatest3.Average + "(Std: " + goalConcededAwayLatest3.StdDev + ")");
                linesToWrite.Add(awayTeam + " Goal Conceded At Away Matches: " + goalConcededAwayAtAway.Average + "(Std: " + goalConcededAwayAtAway.StdDev + ")" + "\n");

                linesToWrite.Add(homeTeam + " Conceded Mostly from: " + goalHomeConcededPosition);
                linesToWrite.Add(awayTeam + " Conceded Mostly from: " + goalAwayConcededPosition + "\n" + "\n");

                linesToWrite.Add("---------Corners---------");
                linesToWrite.Add(homeTeam + " Corners Average: " + cornersAverageHome);
                linesToWrite.Add("Average Corners on " + homeTeam + " Matches: " + cornersAverageOnHomeTeamMatches + "(Std:" + cornersStdOnHomeTeamMatches + ")" + "\n");
                linesToWrite.Add(awayTeam + " Corners Average: " + cornersAverageAway);
                linesToWrite.Add("Average Corners on " + awayTeam + " Matches: " + cornersAverageOnAwayTeamMatches + "(Std:" + cornersStdOnAwayTeamMatches + ")" + "\n" + "\n");

                linesToWrite.Add("---------Other Stats---------");
                linesToWrite.Add(homeTeam + " Fouls Average: " + foulsAverageHome);
                linesToWrite.Add(awayTeam + " Fouls Average: " + foulsAverageAway + "\n");

                linesToWrite.Add(homeTeam + " Possession Average: " + possessionAverageHome + "%");
                linesToWrite.Add(awayTeam + " Possession Average: " + possessionAverageAway + "%" + "\n");

                linesToWrite.Add(homeTeam + " Shots on Goal Average: " + shotsOnTargetAverageHome);
                linesToWrite.Add(awayTeam + " Shots on Goal Average: " + shotsOnTargetAverageAway + "\n");

                linesToWrite.Add("---------Funnels (Possession:Shots:Goals)---------");
                linesToWrite.Add(homeTeam + " Funnel: (" +((double)possessionAverageHome) + ") : (" + ((double)shotsOnTargetAverageHome) + ") : (" + (goalScoredHomeSeasonal.Average) + ")");
                linesToWrite.Add(homeTeam + " Possession to Shots Conversion: " + possToShotsRateHome);
                linesToWrite.Add(homeTeam + " Shots to Goals Conversion: " + shotsToGoalsRateHome + "\n");

                linesToWrite.Add(homeTeam + " Opponents Funnel: (" + ((double)possessionAverageAgainstHome) + ") : (" + ((double)shotsOnTargetAverageAgainstHome) + ") : (" + (goalConcededHomeSeasonal.Average) + ")");
                linesToWrite.Add(homeTeam + " Possession to Shots Conversion Against " + homeTeam + ": " + possToShotsRateAgainstHome);
                linesToWrite.Add(homeTeam + " Shots to Goals Conversion Against " + homeTeam + ": " + shotsToGoalsRateAgainstHome + "\n");

                linesToWrite.Add(awayTeam + " Funnel: (" + ((double) possessionAverageAway) + ") : (" + ((double) shotsOnTargetAverageAway) + ") : (" + (goalScoredAwaySeasonal.Average) + ")");
                linesToWrite.Add(awayTeam + " Possession to Shots Conversion: " + possToShotsRateAway);
                linesToWrite.Add(awayTeam + " Shots to Goals Conversion: " + shotsToGoalRateAway + "\n");

                linesToWrite.Add(awayTeam + " Oponents Funnel: (" + ((double)possessionAverageAgainstAway) + ") : (" + ((double)shotsOnTargetAverageAgainstAway) + ") : (" + (goalConcededAwaySeasonal.Average) + ")");
                linesToWrite.Add(awayTeam + " Possession to Shots Conversion Against " + awayTeam + ": " + possToShotsRateAgainstAway);
                linesToWrite.Add(awayTeam + " Shots to Goals Conversion Against " + awayTeam + ": " + shotsToGoalRateAgainstAway);
                linesToWrite.Add("");
                linesToWrite.Add(homeTeam + " Expected Conversion: " + homeExpectedConversionRate);
                linesToWrite.Add(awayTeam + " Expected Conversion: " + awayExpectedConversionRate + "\n");
                linesToWrite.Add("Expected Average Conversion: " + Math.Round((awayExpectedConversionRate + homeExpectedConversionRate) / 2, 2));
                linesToWrite.Add("");
                linesToWrite.Add(conversionExpectedWinner);
                linesToWrite.Add("Shots Conversion Winner: " + expectedWinnerByConversion.Winner + " (" + Math.Min(expectedWinnerByConversion.Confidence * 100, 100) + "%)");
            
            }

            File.WriteAllLines(path, linesToWrite);
        }
        public static void PrintReportForNextDays(int daysToGet)
        {
            var matches = PremierLeagueMainProject.GetNextMatches(daysToGet);
            var combinedLetterDict = LettersSequenceCalculator.GetCombinedLettersDictionary();
            var attributeDict = SecondaryStatsCalculator.BuildAttributesDictionary(3);
            var attributeClashingMap = MainCalculator.BuildAttributeMatchMap();
            foreach (var match in matches)
            {
                Console.WriteLine(match.HomeTeam + " Vs. " + match.AwayTeam);
                CreateMatchReport(combinedLetterDict, attributeDict, attributeClashingMap, match.HomeTeam, match.AwayTeam, 3);
            }
        }

        public static void PrintReportForNextDaysNewVersion(int daysToGet, int competitionId, bool withAttr = false)
        {
            Console.WriteLine($"Started: {DateTime.Now.Hour}:{DateTime.Now.Minute}");
            var matches = PremierLeagueMainProject.GetNextMatches(daysToGet);
            Console.WriteLine($"Got {matches.Count} new matches");
            var combinedLetterDict = LettersSequenceCalculator.GetCombinedLettersDictionary();
            var attributeClashingMap = new List<MainCalculator.AttributesMatch>();
            var attributeDict = new Dictionary<int, List<DataObjects.AttributeType>>();
            if (withAttr)
            {
                Console.WriteLine("Starting building attributes dictionary");
                attributeDict = SecondaryStatsCalculator.BuildAttributesDictionary(competitionId);
                Console.WriteLine("Finished building attributes dictionary");
                attributeClashingMap = MainCalculator.BuildAttributeMatchMap();
                Console.WriteLine($"Ended build: {DateTime.Now.Hour}:{DateTime.Now.Minute}");
            }
        
            var recs = new List<MainCalculator.Recommendation>();
            var recsPath = @"C:\Users\user\Desktop\DataProjects\2018\RecommendationsFile.tsv";
            foreach (var match in matches)
            {
                var path = @"C:\Users\user\Desktop\DataProjects\2018\" + match.HomeTeam + "VS" + match.AwayTeam + ".tsv";
                Console.WriteLine(match.HomeTeam + " Vs. " + match.AwayTeam);
                var reportObj = new ReportObject();
                reportObj.Init(combinedLetterDict, attributeDict, attributeClashingMap, competitionId);
                reportObj.Build(match.HomeTeam, match.AwayTeam);

                reportObj.CalculateExpectedWinnerByStrength();
                reportObj.CalculateExpectedWinnerByLetters();
                //reportObj.CalculateAttributesExpectedWinner();

                reportObj.FindRecommendations();
                recs.AddRange(reportObj.MatchRecommendations);

                var linesToWrite = reportObj.GetLinesToWrite();
                File.WriteAllLines(path, linesToWrite);
            }

            var recsToWrite = recs.Where(x => x.Result != null)
                .Where(x => x.Confidence >= 60m)
                .OrderByDescending(x => x.Confidence)
                .Select(x => $"{x.HomeTeam} VS. {x.AwayTeam} ({x.Type}): {x.Result} ({x.Confidence}%) (Ratio: {Math.Round(100/x.Confidence, 2)})");
            File.WriteAllLines(recsPath, recsToWrite);
        }
    }
}
