using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProjects
{
    public class MachineLearningProject
    {
        public class StatisticLine
        {
            public int CompetitionMatchId;

            public int CompetitionID;
            public decimal Round;
            public DateTime Date;
            public string HomeTeam;
            public int HomeTeamID;
            public int AwayTeamID;
            public int HomeTeamValue;
            public int AwayTeamValue;
            public string AwayTeam;
            public string Winner;
            public decimal HomePointsSeasonal;
            public decimal HomePointsLast3;
            public decimal HomePointsAtHome;
            public double HomeGoalsAvgSeasonal;
            public double HomeGoalsAvgLast3;
            public double HomeGoalsAvgAtHome;
            public double HomeGoalsExpected;
            public double HomeGoalsConcededAvgSeasonal;
            public double HomeGoalsConcededAvgLast3;
            public double HomeGoalsConcededAvgAtHome;
            public double HomeGoalsExpectedConcede;
            public decimal AwayPointsSeasonal;
            public decimal AwayPointsLast3;
            public decimal AwayPointsAtAway;
            public double AwayGoalsAvgSeasonal;
            public double AwayGoalsAvgLast3;
            public double AwayGoalsAvgAtAway;
            public double AwayGoalsExpected;
            public double AwayGoalsConcededAvgSeasonal;
            public double AwayGoalsConcededAvgLast3;
            public double AwayGoalsConcededAvgAtAway;
            public double AwayGoalsExpectedConcede;
            public double HomeGoalsExpectedVsTeam;
            public double AwayGoalsExpectedVsTeam;
            public string HomeLast3Letters;
            public string AwayLast3Letters;
            public double HomeTotalShotsAvg;
            public double AwayTotalShotsAvg;
            public double HomeShotsOnTargetAvg;
            public double AwayShotsOnTargetAvg;
            public double HomeTotalShotsAgainstAvg;
            public double AwayTotalShotsAgainstAvg;
            public double HomeShotsOnTargetAgainstAvg;
            public double AwayShotsOnTargetAgainstAvg;
            public double HomeTotalShotsExpectedVsTeam;
            public double AwayTotalShotsExpectedVsTeam;
            public double HomeShotsOnTargetExpectedVsTeam;
            public double AwayShotsOnTargetExpectedVsTeam;
            public double HomeFoulsAvg;
            public double AwayFoulsAvg;
            public double HomePossessionAvg;
            public double AwayPossessionAvg;
            public decimal HomeExpectedWin;
            public decimal AwayExpectedWin;
            public decimal HomeTrend;
            public decimal AwayTrend;
            public double HomePointsWeightedSeasonal;
            public double AwayPointsWeightedSeasonal;
            public double ChanceCreationHome;
            public double ChanceCreationAway;
            public double ShotsAccuracyHome;
            public double ShotsAccuracyAway;
            public double ScoringRateHome;
            public double ScoringRateAway;
            public double KeeperStrengthHome;
            public double KeeperStrengthAway;
            public decimal HomeOddsRatio;
            public decimal DrawOddsRatio;
            public decimal AwayOddsRatio;
            public decimal IsHomeFavorite;
            public double Over2_5;

            private static Dictionary<string, List<Helper.LetterDistribution>> LettersDict;
            public List<competitionmatch> HomeTeamMatches;
            public List<competitionmatch> AwayTeamMatches;
            public List<matchevent> HomeTotalShots;
            public List<matchevent> AwayTotalShots;
            public List<matchevent> HomeShotsOnTarget;
            public List<matchevent> AwayShotsOnTarget;
            public List<matchevent> HomeTotalShotsAgainst;
            public List<matchevent> AwayTotalShotsAgainst;
            public List<matchevent> HomeFouls;
            public List<matchevent> AwayFouls;
            public List<matchevent> HomePossession;
            public List<matchevent> AwayPossession;

            public string Print ()
            {
                if (Winner == null)
                {
                    return null;
                }

                return Math.Round((double)HomeTeamValue / 10000, 4) + ","
                    + Math.Round((double)AwayTeamValue / 10000, 4) + ","
                    + Math.Round(HomePointsSeasonal / 10, 2) + ","
                    + Math.Round(AwayPointsSeasonal/10, 2) + "," +
                    Math.Round(HomePointsLast3/10, 2) + "," +
                    Math.Round(AwayPointsLast3/10, 2) + "," +
                    Math.Round(HomePointsAtHome/10, 2) + "," +
                    Math.Round(AwayPointsAtAway/10,2) + "," +
                    Math.Round(HomeGoalsAvgSeasonal/10,2) + "," +
                    Math.Round(AwayGoalsAvgSeasonal/10, 2) + "," +
                    Math.Round(HomeGoalsAvgLast3/10, 2) + "," +
                    Math.Round(AwayGoalsAvgLast3/10, 2) + "," +
                    Math.Round(HomeGoalsAvgAtHome/10,2) + "," +
                    Math.Round(AwayGoalsAvgAtAway / 10, 2) + "," +
                    Math.Round(HomeGoalsExpected / 10, 2) + "," +
                    Math.Round(AwayGoalsExpected / 10, 2) + "," +
                    Math.Round(HomeGoalsConcededAvgSeasonal/10, 2) + ","+
                    Math.Round(AwayGoalsConcededAvgSeasonal/10, 2) + "," +
                    Math.Round(HomeGoalsConcededAvgLast3/10, 2) + "," +
                    Math.Round(AwayGoalsConcededAvgLast3/10, 2) + "," +
                    Math.Round(HomeGoalsConcededAvgAtHome/10, 2) + "," +
                    Math.Round(AwayGoalsConcededAvgAtAway / 10, 2) + "," +
                    Math.Round(HomeGoalsExpectedConcede / 10, 2) + "," +
                    Math.Round(AwayGoalsExpectedConcede / 10, 2) + "," +
                    Math.Round(HomeGoalsExpectedVsTeam / 10, 2) + "," +
                    Math.Round(AwayGoalsExpectedVsTeam / 10, 2) + "," +
                    Math.Round(HomeTotalShotsAvg/100, 2) + "," +
                    Math.Round(AwayTotalShotsAvg/100, 2) + "," +
                    Math.Round(HomeShotsOnTargetAvg/100, 2) + "," +
                    Math.Round(AwayShotsOnTargetAvg/100, 2) + "," +
                    Math.Round(HomeTotalShotsAgainstAvg/100, 2) + "," +
                    Math.Round(AwayTotalShotsAgainstAvg/100, 2) + "," +
                    Math.Round(HomeShotsOnTargetAgainstAvg/100, 2) + "," +
                    Math.Round(AwayShotsOnTargetAgainstAvg / 100, 2) + "," +
                    Math.Round(HomeTotalShotsExpectedVsTeam / 100, 2) + "," +
                    Math.Round(AwayTotalShotsExpectedVsTeam / 100, 2) + "," +
                    Math.Round(HomeShotsOnTargetExpectedVsTeam / 100, 2) + "," +
                    Math.Round(AwayShotsOnTargetExpectedVsTeam / 100, 2) + "," +
                    Math.Round(HomePossessionAvg/100, 2) + "," +
                    Math.Round(AwayPossessionAvg/100, 2) + "," + 
                    //HomeFoulsAvg + "," + 
                    //AwayFoulsAvg + "," + 
                    HomeExpectedWin + "," +
                    AwayExpectedWin + "," +
                    Math.Round(HomeTrend/100, 2) + "," +
                    Math.Round(AwayTrend/100, 2) + "," +
                    +Math.Round(HomePointsWeightedSeasonal / 10000, 4) + ","
                    + Math.Round(AwayPointsWeightedSeasonal / 10000, 4) + "," +
                    Math.Round(ChanceCreationHome / 100, 3) + "," +
                    Math.Round(ChanceCreationAway / 100, 3) + "," +
                    Math.Round(ShotsAccuracyHome / 100, 3) + "," +
                    Math.Round(ShotsAccuracyAway / 100, 3) + "," +
                    Math.Round(ScoringRateHome / 100, 3) + "," +
                    Math.Round(ScoringRateAway / 100, 3) + "," + 
                    Math.Round(KeeperStrengthHome / 100, 3) + "," +
                    Math.Round(KeeperStrengthAway / 100, 3) + "," +
                    Math.Round(Round / 100, 3) + "," +
                    Math.Round(HomeOddsRatio, 1) + "," +
                    Math.Round(DrawOddsRatio,1) + "," +
                    Math.Round(AwayOddsRatio, 1) + "," +
                    IsHomeFavorite + "," +
                    CompetitionID + "," +
                    Winner + "," +
                    Over2_5;
            }

            public static string PrintCsvTrainAttrs()
            {
                return StatisticLine.PrintAttrs().Replace("@relation PL\n", "")
                                                 .Replace("@ATTRIBUTE ", "")
                                                 .Replace(" NUMERIC", "")
                                                 .Replace(" {H, D, A}", "")
                                                 .Replace(" {True, False}", "")
                                                 .Replace("@data", "")
                                                 .Replace("\n", ",");
            }


            public static string PrintCsvTestAttrs()
            {
                return "Match," +  StatisticLine.PrintAttrs().Replace("@relation PL\n", "")
                                                 .Replace("@ATTRIBUTE ", "")
                                                 .Replace(" NUMERIC", "")
                                                 .Replace(" {H, D, A}", "")
                                                 .Replace(" {True, False}", "")
                                                 .Replace("@data", "")
                                                 .Replace("\n", ",");
            }

            public static string PrintAttrs()
            {
                
                return "@relation PL\n" +
                       "@ATTRIBUTE HomeValue NUMERIC\n" +
                       "@ATTRIBUTE AwayValue NUMERIC\n" +
                       "@ATTRIBUTE HomePtsSeas NUMERIC\n" +
                       "@ATTRIBUTE AwayPtsSeas NUMERIC\n" +
                       "@ATTRIBUTE HomePtsLast NUMERIC\n" +
                       "@ATTRIBUTE AwayPtsLast NUMERIC\n" +
                       "@ATTRIBUTE HomePtsHome NUMERIC\n" +
                       "@ATTRIBUTE AwayPtsAway NUMERIC\n" +
                        "@ATTRIBUTE HomeGoalsSeas NUMERIC\n" +
                       "@ATTRIBUTE AwayGoalsSeas NUMERIC\n" +
                       "@ATTRIBUTE HomeGoalsLast NUMERIC\n" +
                       "@ATTRIBUTE AwayGoalsLast NUMERIC\n" +
                       "@ATTRIBUTE HomeGoalsHome NUMERIC\n" +
                       "@ATTRIBUTE AwayGoalsAway NUMERIC\n" +
                       "@ATTRIBUTE HomeGoalsExpected NUMERIC\n" +
                       "@ATTRIBUTE AwayGoalsExpected NUMERIC\n" +
                       "@ATTRIBUTE HomeGoalsConcSeas NUMERIC\n" +
                       "@ATTRIBUTE AwayGoalsConcSeas NUMERIC\n" +
                       "@ATTRIBUTE HomeGoalsConcLast NUMERIC\n" +
                       "@ATTRIBUTE AwayGoalsConcLast NUMERIC\n" +
                       "@ATTRIBUTE HomeGoalsConcHome NUMERIC\n" +
                       "@ATTRIBUTE AwayGoalsConcAway NUMERIC\n" +
                       "@ATTRIBUTE HomeGoalsExpectedConc NUMERIC\n" +
                       "@ATTRIBUTE AwayGoalsExpectedConc NUMERIC\n" +
                       "@ATTRIBUTE HomeGoalsExpectedVsTeam NUMERIC\n" +
                       "@ATTRIBUTE AwayGoalsExpectedVsTeam NUMERIC\n" +
                       "@ATTRIBUTE HomeTotalShots NUMERIC\n" +
                       "@ATTRIBUTE AwayTotalShots NUMERIC\n" +
                       "@ATTRIBUTE HomeShotsTarget NUMERIC\n" +
                       "@ATTRIBUTE AwayShotsTarget NUMERIC\n" +
                       "@ATTRIBUTE HomeTotalShotsAgst NUMERIC\n" +
                       "@ATTRIBUTE AwayTotalShotsAgst NUMERIC\n" +
                       "@ATTRIBUTE HomeShotsTargetAgst NUMERIC\n" +
                       "@ATTRIBUTE AwayShotsTargetAgst NUMERIC\n" +
                       "@ATTRIBUTE HomeTotalShotsVsTeam NUMERIC\n" +
                       "@ATTRIBUTE AwayTotalShotsVsTeam NUMERIC\n" +
                       "@ATTRIBUTE HomeShotsTargetVsTeam NUMERIC\n" +
                       "@ATTRIBUTE AwayShotsTargetVsTeam NUMERIC\n" +
                       "@ATTRIBUTE HomePossession NUMERIC\n" +
                       "@ATTRIBUTE AwayPossession NUMERIC\n" +
                       //"@ATTRIBUTE HomeFouls NUMERIC\n" +
                      // "@ATTRIBUTE AwayFouls NUMERIC\n" +
                       "@ATTRIBUTE HomeExpectedWin NUMERIC\n" +
                       "@ATTRIBUTE AwayExpectedWin NUMERIC\n" +
                       "@ATTRIBUTE HomeTrend NUMERIC\n" +
                       "@ATTRIBUTE AwayTrend NUMERIC\n" +
                       "@ATTRIBUTE HomeWeightedPoints NUMERIC\n" +
                       "@ATTRIBUTE AwayWeightedPoints NUMERIC\n" +
                       "@ATTRIBUTE ChanceCreationHome NUMERIC\n" +
                       "@ATTRIBUTE ChanceCreationAway NUMERIC\n" +
                       "@ATTRIBUTE ShotsAccuracyHome NUMERIC\n" +
                       "@ATTRIBUTE ShotsAccuracyAway NUMERIC\n" +
                       "@ATTRIBUTE ScoringRateHome NUMERIC\n" +
                       "@ATTRIBUTE ScoringRateAway NUMERIC\n" +
                       "@ATTRIBUTE KeeperStrengthHome NUMERIC\n" +
                       "@ATTRIBUTE KeeperStrengthAway NUMERIC\n" +
                       "@ATTRIBUTE Round NUMERIC\n" +
                       "@ATTRIBUTE HomeTeamOdds NUMERIC\n" +
                       "@ATTRIBUTE DrawOdds NUMERIC\n" +
                       "@ATTRIBUTE AwayTeamOdds NUMERIC\n" +
                       "@ATTRIBUTE IsHomeFavorite NUMERIC\n" +
                       "@ATTRIBUTE CompetitionID NUMERIC\n" +
                       "@ATTRIBUTE Winner {H, D, A}\n" +
                       "@ATTRIBUTE Over2.5 {True, False}\n" +
                       "@data"
                       ;
            }

            public static void initDict()
            {
                LettersDict = LettersSequenceCalculator.GetCombinedLettersDictionary();
            }

            public void init(int homeTeamid, int awayTeamId, DateTime date, int competitionId)
            {
                    Date = date;
                    CompetitionID = competitionId;
                    HomeTeamID = homeTeamid;
                    AwayTeamID = awayTeamId;
            }

            public void init(string homeTeam, string awayTeam, DateTime date, int competitionId)
            {
                Date = date;
                CompetitionID = competitionId;

                using (var db = new sakilaEntities4())
                {
                    var homeTeamObj = db.team.First(x => x.TeamName == homeTeam);
                    var awayTeamObj = db.team.First(x => x.TeamName == awayTeam);

                    HomeTeamID = homeTeamObj.TeamID;
                    AwayTeamID = awayTeamObj.TeamID;
                }
            }

            public void BuildTrainingLine()
            {
                using (var db = new sakilaEntities4())
                {
                    var match = db.competitionmatch.First(x => x.HomeTeamID == HomeTeamID && x.AwayTeamID == AwayTeamID && x.CompetitionID == CompetitionID);
                    CompetitionMatchId = match.CompetitionMatchID;
                    
                    if (!AggregateStats(db))
                    {
                        return;
                    }

                    var over2_5 = false;
                    if (match.HomeGoals + match.AwayGoals > 2.5)
                    {
                        over2_5 = true;
                    }

                    Over2_5 = over2_5 ? 1 : 0;

                    var winner = "D";
                    if (match.HomeGoals > match.AwayGoals)
                    {
                        winner = "H";
                    }

                    else if (match.AwayGoals > match.HomeGoals)
                    {
                        winner = "A";
                    }

                    Winner = winner;
                }

            }

            public void BuildTestLine()
            {
                using (var db = new sakilaEntities4())
                {
                    if (AggregateStats(db, isTest: true))
                    {
                        var winner = "D";
                        Winner = winner;
                        Over2_5 = 0;
                    }
                }
            }

            public bool AggregateStats(sakilaEntities4 db, bool isTest = false)
            {              
                var homeTeam = db.team.First(x => x.TeamID == HomeTeamID);
                var awayTeam = db.team.First(x => x.TeamID == AwayTeamID);
                HomeTeamValue = homeTeam.MarketValue.Value;
                AwayTeamValue = awayTeam.MarketValue.Value;
                HomeTeam = homeTeam.TeamName;
                AwayTeam = awayTeam.TeamName;

                HomeTeamMatches = MainCalculator.GetTeamLatesMatches(db, HomeTeamID, CompetitionID, 50, Date);
                AwayTeamMatches = MainCalculator.GetTeamLatesMatches(db, AwayTeamID, CompetitionID, 50, Date);

                if (HomeTeamMatches.Count < 4 || HomeTeamMatches.Count > 34 || AwayTeamMatches.Count < 4 || AwayTeamMatches.Count > 34)
                {
                    return false;
                }

                HomeTotalShotsAvg = SecondaryStatsCalculator.GetAverageEventValue(db, HomeTeamID, CompetitionID, (int)DataObjects.EventType.TotalShots).Value;
                AwayTotalShotsAvg = SecondaryStatsCalculator.GetAverageEventValue(db, AwayTeamID, CompetitionID, (int)DataObjects.EventType.TotalShots).Value;

                HomeShotsOnTargetAvg = SecondaryStatsCalculator.GetAverageEventValue(db, HomeTeamID, CompetitionID, (int)DataObjects.EventType.ShotsOnTarget).Value;
                AwayShotsOnTargetAvg = SecondaryStatsCalculator.GetAverageEventValue(db, AwayTeamID, CompetitionID, (int)DataObjects.EventType.ShotsOnTarget).Value;

                HomeTotalShotsAgainstAvg = SecondaryStatsCalculator.GetAverageEventValueAgainstTeam(db, HomeTeamID, CompetitionID, (int)DataObjects.EventType.TotalShots).Value;
                AwayTotalShotsAgainstAvg = SecondaryStatsCalculator.GetAverageEventValueAgainstTeam(db, AwayTeamID, CompetitionID, (int)DataObjects.EventType.TotalShots).Value;

                HomeShotsOnTargetAgainstAvg = SecondaryStatsCalculator.GetAverageEventValueAgainstTeam(db, HomeTeamID, CompetitionID, (int)DataObjects.EventType.ShotsOnTarget).Value;
                AwayShotsOnTargetAgainstAvg = SecondaryStatsCalculator.GetAverageEventValueAgainstTeam(db, AwayTeamID, CompetitionID, (int)DataObjects.EventType.ShotsOnTarget).Value;

                HomeTotalShotsExpectedVsTeam = (HomeTotalShotsAvg + AwayTotalShotsAgainstAvg) / 2;
                AwayTotalShotsExpectedVsTeam = (AwayTotalShotsAvg + HomeTotalShotsAgainstAvg) / 2;

                HomeShotsOnTargetExpectedVsTeam = (HomeShotsOnTargetAvg + AwayShotsOnTargetAgainstAvg) / 2;
                AwayShotsOnTargetExpectedVsTeam = (AwayShotsOnTargetAvg + HomeShotsOnTargetAgainstAvg) / 2;

                HomePossessionAvg = SecondaryStatsCalculator.GetAverageEventValue(db, HomeTeamID, CompetitionID, (int)DataObjects.EventType.Possession).Value;
                AwayPossessionAvg = SecondaryStatsCalculator.GetAverageEventValue(db, AwayTeamID, CompetitionID, (int)DataObjects.EventType.Possession).Value;

                HomeFoulsAvg = SecondaryStatsCalculator.GetAverageEventValue(db, HomeTeamID, CompetitionID, (int)DataObjects.EventType.Fouls).Value;
                AwayFoulsAvg = SecondaryStatsCalculator.GetAverageEventValue(db, AwayTeamID, CompetitionID, (int)DataObjects.EventType.Fouls).Value;

                HomePointsSeasonal = PointsCalculator.CalculatePointPace(PointsCalculator.GetTeamBalance(HomeTeamMatches, HomeTeamID, CompetitionID));
                AwayPointsSeasonal = PointsCalculator.CalculatePointPace(PointsCalculator.GetTeamBalance(AwayTeamMatches, AwayTeamID, CompetitionID));

                HomePointsLast3 = PointsCalculator.CalculatePointPace(PointsCalculator.GetTeamBalance(HomeTeamMatches, HomeTeamID, CompetitionID, 3));
                AwayPointsLast3 = PointsCalculator.CalculatePointPace(PointsCalculator.GetTeamBalance(AwayTeamMatches, AwayTeamID, CompetitionID, 3));

                HomePointsAtHome = PointsCalculator.CalculatePointPace(PointsCalculator.GetTeamBalanceHome(HomeTeamMatches, HomeTeamID, CompetitionID));
                AwayPointsAtAway = PointsCalculator.CalculatePointPace(PointsCalculator.GetTeamBalanceAway(AwayTeamMatches, AwayTeamID, CompetitionID));

                HomeGoalsAvgSeasonal = GoalsCalculator.GetGoalsScoringAverage(HomeTeamMatches, HomeTeamID, CompetitionID).Average;
                AwayGoalsAvgSeasonal = GoalsCalculator.GetGoalsScoringAverage(AwayTeamMatches, AwayTeamID, CompetitionID).Average;

                HomeGoalsAvgLast3 = GoalsCalculator.GetGoalsScoringAverage(HomeTeamMatches, HomeTeamID, CompetitionID, 3).Average;
                AwayGoalsAvgLast3 = GoalsCalculator.GetGoalsScoringAverage(AwayTeamMatches, AwayTeamID, CompetitionID, 3).Average;

                HomeGoalsAvgAtHome = GoalsCalculator.GetGoalsScoringAverageAtHome(HomeTeamMatches, HomeTeamID, CompetitionID).Average;
                AwayGoalsAvgAtAway = GoalsCalculator.GetGoalsScoringAverageAtAway(AwayTeamMatches, AwayTeamID, CompetitionID).Average;

                HomeGoalsExpected = (HomeGoalsAvgSeasonal + HomeGoalsAvgLast3 + HomeGoalsAvgAtHome) / 3;
                AwayGoalsExpected = (AwayGoalsAvgSeasonal + AwayGoalsAvgLast3 + AwayGoalsAvgAtAway) / 3;

                HomeGoalsConcededAvgSeasonal = GoalsCalculator.GetGoalsConcededAverage(HomeTeamMatches, HomeTeamID, CompetitionID).Average;
                AwayGoalsConcededAvgSeasonal = GoalsCalculator.GetGoalsConcededAverage(AwayTeamMatches, AwayTeamID, CompetitionID).Average;

                HomeGoalsConcededAvgLast3 = GoalsCalculator.GetGoalsConcededAverage(HomeTeamMatches, HomeTeamID, CompetitionID, 3).Average;
                AwayGoalsConcededAvgLast3 = GoalsCalculator.GetGoalsConcededAverage(AwayTeamMatches, AwayTeamID, CompetitionID, 3).Average;

                HomeGoalsConcededAvgAtHome = GoalsCalculator.GetGoalsConcededAverageAtHome(HomeTeamMatches, HomeTeamID, CompetitionID).Average;
                AwayGoalsConcededAvgAtAway = GoalsCalculator.GetGoalsConcededAverageAtAway(AwayTeamMatches, AwayTeamID, CompetitionID).Average;

                HomeGoalsExpectedConcede = (HomeGoalsConcededAvgSeasonal + HomeGoalsConcededAvgLast3 + HomeGoalsConcededAvgAtHome) / 3;
                AwayGoalsExpectedConcede = (AwayGoalsConcededAvgSeasonal + AwayGoalsConcededAvgLast3 + AwayGoalsConcededAvgAtAway) / 3;

                HomeGoalsExpectedVsTeam = (HomeGoalsExpected + AwayGoalsExpectedConcede) / 2;
                AwayGoalsExpectedVsTeam = (AwayGoalsExpected + HomeGoalsExpectedConcede) / 2;

                HomeTrend = LettersSequenceCalculator.GetTeamLetterScore(HomeTeamMatches, HomeTeamID, CompetitionID);
                AwayTrend = LettersSequenceCalculator.GetTeamLetterScore(AwayTeamMatches, AwayTeamID, CompetitionID);

                HomePointsWeightedSeasonal = PointsCalculator.GetWeightedBalance(db, HomeTeamMatches, HomeTeamID, CompetitionID);
                AwayPointsWeightedSeasonal = (PointsCalculator.GetWeightedBalance(db, AwayTeamMatches, AwayTeamID, CompetitionID));

                ChanceCreationHome = HomePossessionAvg / HomeTotalShotsAvg;
                ChanceCreationAway = AwayPossessionAvg / AwayTotalShotsAvg;

                ShotsAccuracyHome = HomeTotalShotsAvg == 0 || HomeShotsOnTargetAvg == 0 ? 0 : HomeTotalShotsAvg / HomeShotsOnTargetAvg;
                ShotsAccuracyAway = AwayTotalShotsAvg == 0 || AwayShotsOnTargetAvg == 0 ? 0 : AwayTotalShotsAvg / AwayShotsOnTargetAvg;

                ScoringRateHome = HomeShotsOnTargetAvg == 0 || HomeGoalsAvgSeasonal == 0 ? 0 : HomeShotsOnTargetAvg / HomeGoalsAvgSeasonal;
                ScoringRateAway = AwayShotsOnTargetAvg == 0 || AwayGoalsAvgSeasonal == 0 ? 0 : AwayShotsOnTargetAvg / AwayGoalsAvgSeasonal;

                KeeperStrengthHome = HomeShotsOnTargetAgainstAvg == 0 || HomeGoalsConcededAvgSeasonal == 0 ? 0 : HomeShotsOnTargetAgainstAvg / HomeGoalsConcededAvgSeasonal;
                KeeperStrengthAway = AwayShotsOnTargetAgainstAvg == 0 || AwayGoalsConcededAvgSeasonal == 0 ? 0 : AwayShotsOnTargetAgainstAvg / AwayGoalsConcededAvgSeasonal;
                
                if (!isTest)
                {
                    var relevantMatchOdds = db.matchodds.FirstOrDefault(x => x.MatchID == CompetitionMatchId);
                    if (relevantMatchOdds != null)
                    {
                        HomeOddsRatio = relevantMatchOdds.HomeTeamOdds;
                        DrawOddsRatio = relevantMatchOdds.DrawOdds;
                        AwayOddsRatio = relevantMatchOdds.AwayTeamOdds;
                        IsHomeFavorite = 0;
                        if (HomeOddsRatio < AwayOddsRatio)
                        {
                            IsHomeFavorite = 1;
                        }
                    }

                    else
                    {
                        Console.WriteLine("Failed to fing odds for match: " + HomeTeamID + " VS. " + AwayTeamID);
                        HomeOddsRatio = 0.00M;
                        DrawOddsRatio = 0.00M;
                        AwayOddsRatio = 0.00M;
                        IsHomeFavorite = 0;
                    }

                }

                else
                {
                    HomeOddsRatio = 0.00M;
                    DrawOddsRatio = 0.00M;
                    AwayOddsRatio = 0.00M;
                    IsHomeFavorite = 0;
                }
                

                var homeTeamSeq = LettersSequenceCalculator.GetTeamLetterSequence(db, HomeTeamID, CompetitionID, 3);
                var homeSeqString = string.Join("", homeTeamSeq);
                var homeTeamExpected =  LettersDict.First(x => x.Key.Equals(homeSeqString)).Value;
                HomeExpectedWin = Math.Round(homeTeamExpected.First(x => x.Letter == "W").Percent,2);

                var awayTeamSeq = LettersSequenceCalculator.GetTeamLetterSequence(db, AwayTeamID, CompetitionID, 3);
                var awaySeqString = string.Join("", awayTeamSeq);
                var awayTeamExpected = LettersDict.First(x => x.Key.Equals(awaySeqString)).Value;
                AwayExpectedWin = Math.Round(awayTeamExpected.First(x => x.Letter == "W").Percent, 2);

                Round = (decimal)(HomeTeamMatches.Count + AwayTeamMatches.Count)/2;

                if (AwayPossessionAvg == 0.0 || HomePossessionAvg == 0.0)
                {
                    return false;
                }
                
                return true;
            }
        }

        public static void PrintTrainingFile()
        {
            StatisticLine.initDict();
            var competitions = new List<int> {2, 3, 4, 8, 9, 10, 11};
            var linesToWrite = new List<string>();
            linesToWrite.Add(StatisticLine.PrintCsvTrainAttrs());
            int cnt = 0;
            using (var db = new sakilaEntities4())
            {
                foreach (var competition in competitions)
                {
                    Console.WriteLine(competition);
                    var matches = db.competitionmatch.Where(x => x.CompetitionID == competition).ToList();
                    foreach (var match in matches)
                    {
                        var sl = new StatisticLine();
                        sl.init(match.HomeTeamID, match.AwayTeamID, match.MatchDate, match.CompetitionID);
                        sl.BuildTrainingLine();
                        var line = sl.Print();
                        if (line != null)
                        {
                            linesToWrite.Add(line);
                            cnt = cnt + 1;
                            Console.WriteLine(cnt);
                        }
                    }
                }
            }

            //File.WriteAllLines(@"C:\Users\user\Desktop\DataProjects\training.arff", linesToWrite);
            File.WriteAllLines(@"C:\Users\user\Desktop\DataProjects\PredictionsModel\train.csv", linesToWrite);
        }

        public static void PrintTestFile(int daysToGet, int competitionId)
        {
            StatisticLine.initDict();
            var matchesToWrite = new List<string>();
            var linesToWrite = new List<string>();
            linesToWrite.Add(StatisticLine.PrintCsvTestAttrs());
            var matches = PremierLeagueMainProject.GetNextMatches(daysToGet);
            foreach (var match in matches)
            {
                var matchLine = match.HomeTeam + " VS. " + match.AwayTeam;
                Console.WriteLine(match.HomeTeam + " VS. " + match.AwayTeam);
                matchesToWrite.Add(match.HomeTeam + " VS. " + match.AwayTeam);
                var sl = new StatisticLine();
                sl.init(match.HomeTeam, match.AwayTeam, match.Date, competitionId);
                sl.BuildTestLine();
                var line = matchLine + "," + sl.Print();
                if (line != null)
                {
                    linesToWrite.Add(line);
                }
            }

            File.WriteAllLines(@"C:\Users\user\Desktop\DataProjects\PredictionsModel\test.csv", linesToWrite);
            //File.WriteAllLines(@"C:\Users\user\Desktop\DataProjects\testlines.csv", matchesToWrite);

        }

    }
}
