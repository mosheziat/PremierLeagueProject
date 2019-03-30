using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery.ExtensionMethods.Internal;

namespace DataProjects
{
    public class ReportObject
    {
        public string HomeTeamName;
        public string AwayTeamName;

        public TeamReportStats HomeTeamStats;
        public TeamReportStats AwayTeamStats;

        public MainCalculator.TeamStdDevAndAverage HomeExpectedGoals;
        public MainCalculator.TeamStdDevAndAverage AwayExpectedGoals;

        public MainCalculator.ResultConfidence StrengthExpectedWinner;
        private int StrengthFactor = 10;

        public MainCalculator.LettersWinner LettersExpectedWinner;
        private int LettersFactor = 7;

        public MainCalculator.ClashingResults ClashingAttributesExpectedWinner;
        private int AttributesFactor = 5;

        public Dictionary<string, List<Helper.LetterDistribution>> LettersDict;
        public Dictionary<int, List<DataObjects.AttributeType>> AttributesDict;
        public List<MainCalculator.AttributesMatch> AttributeClashingMap;
        public List<MainCalculator.Recommendation> MatchRecommendations;
        public int CompetitionId;


        public void Init(Dictionary<string, List<Helper.LetterDistribution>> lettersDict,
            Dictionary<int, List<DataObjects.AttributeType>> attributesDict,
            List<MainCalculator.AttributesMatch> attributeClashingMap,
            int competitionId)
        {
            LettersDict = lettersDict;
            AttributeClashingMap = attributeClashingMap;
            AttributesDict = attributesDict;
            CompetitionId = competitionId;
        }

        public void Build(string homeTeamName, string awayTeamName)
        {
            HomeTeamName = homeTeamName;
            AwayTeamName = awayTeamName;
            using (var db = new sakilaEntities4())
            {
                HomeTeamStats = new TeamReportStats(LettersDict, AttributesDict, CompetitionId, homeTeamName, db, isHomeTeam: true);
                AwayTeamStats = new TeamReportStats(LettersDict, AttributesDict, CompetitionId, awayTeamName, db, isHomeTeam: false);

                HomeExpectedGoals = GoalsCalculator.CalculateExpectedGoals(HomeTeamStats.TeamGoalsStats.TeamSeasonalGoalsScored,
                    HomeTeamStats.TeamGoalsStats.TeamLastGamesGoalsScored, AwayTeamStats.TeamGoalsStats.TeamSeasonalGoalsConceded, AwayTeamStats.TeamGoalsStats.TeamLastGamesGoalsConceded,
                    HomeTeamStats.TeamGoalsStats.TeamHomeOrAwayGoalsScored, AwayTeamStats.TeamGoalsStats.TeamHomeOrAwayGoalsConceded);


                AwayExpectedGoals = GoalsCalculator.CalculateExpectedGoals(AwayTeamStats.TeamGoalsStats.TeamSeasonalGoalsScored,
                    AwayTeamStats.TeamGoalsStats.TeamLastGamesGoalsScored, HomeTeamStats.TeamGoalsStats.TeamSeasonalGoalsConceded, HomeTeamStats.TeamGoalsStats.TeamLastGamesGoalsConceded,
                    AwayTeamStats.TeamGoalsStats.TeamHomeOrAwayGoalsScored, HomeTeamStats.TeamGoalsStats.TeamHomeOrAwayGoalsConceded);
            }
        }
        public void CalculateExpectedWinnerByStrength()
        {
            var homeTeamStengthDiff = MainCalculator.StrengthDiffCaculator(HomeTeamStats.TeamStrength, AwayTeamStats.TeamStrength);
            var awayTeamStengthDiff = MainCalculator.StrengthDiffCaculator(AwayTeamStats.TeamStrength, HomeTeamStats.TeamStrength);

            var homeCalculatedGoals = Math.Round(HomeExpectedGoals.Average + homeTeamStengthDiff, 2);
            var awayCalculatedGoals = Math.Round(AwayExpectedGoals.Average + awayTeamStengthDiff, 2);
            StrengthExpectedWinner = MainCalculator.GetExpectedWinner(homeCalculatedGoals, awayCalculatedGoals);
        }
        public void CalculateExpectedWinnerByLetters()
        {
            LettersExpectedWinner = MainCalculator.GetLettersExpectedWinner(HomeTeamStats.LettersDistribution,
                        AwayTeamStats.LettersDistribution);
        }
        public void CalculateAttributesExpectedWinner()
        {
            ClashingAttributesExpectedWinner = MainCalculator.GetAttributeAdventageWinner(HomeTeamStats.TeamAttributes,
                AwayTeamStats.TeamAttributes, AttributeClashingMap);
        }

        public MainCalculator.Recommendation FindWinnerRecommendations()
        {
            var winnerRecommendation = new MainCalculator.Recommendation();
            var winnerSignals = new List<string>();
            if (StrengthExpectedWinner?.Confidence >= 0.95m)
            {
                winnerSignals.Add(StrengthExpectedWinner.Winner.Trim());
            }

            if (LettersExpectedWinner?.Percent >= 80)
            {
                winnerSignals.Add(LettersExpectedWinner.Winner.Trim());
            }

            /*
            if (ClashingAttributesExpectedWinner?.Percent >= 70)
            {
                winnerSignals.Add(ClashingAttributesExpectedWinner.Winner.Trim());
            }
            */
            if (winnerSignals.Any() && winnerSignals.Distinct().Count() == 1 && !winnerSignals.First().Trim().Equals("Draw"))
            {
                var recommendationConfidence = 50;
                if (StrengthExpectedWinner != null && StrengthExpectedWinner.Winner.Equals(winnerSignals.First()) && StrengthExpectedWinner?.Confidence >= 0.6m)
                {
                    recommendationConfidence += StrengthFactor;
                    if (StrengthExpectedWinner?.Confidence >= 0.8m)
                    {
                        //recommendationConfidence += StrengthFactor;
                        if (StrengthExpectedWinner.Confidence >= 0.95m)
                        {
                            recommendationConfidence += StrengthFactor * 2;
                        }
                    }
                }

                if (LettersExpectedWinner?.Percent >= 70)
                {
                    recommendationConfidence += LettersFactor;

                    if (LettersExpectedWinner?.Percent >= 80)
                    {
                        recommendationConfidence += LettersFactor;
                        if (LettersExpectedWinner.Percent >= 90)
                        {
                            recommendationConfidence += LettersFactor;
                        }
                    }
                }

                /*
                if (ClashingAttributesExpectedWinner?.Percent >= 70)
                {
                    recommendationConfidence += AttributesFactor;
                    if (ClashingAttributesExpectedWinner?.Percent >= 80)
                    {
                        recommendationConfidence += AttributesFactor;
                    }
                }
                */
                recommendationConfidence = (int) Math.Round((double) recommendationConfidence);
                recommendationConfidence = Math.Min(recommendationConfidence, 100);
                winnerRecommendation = new MainCalculator.Recommendation
                {
                    Confidence = recommendationConfidence,
                    Result = winnerSignals.Distinct().First(),
                    Type = "Winner",
                    HomeTeam = HomeTeamName,
                    AwayTeam = AwayTeamName
                };

            }

            return winnerRecommendation;
        }
        public MainCalculator.Recommendation FindGoalsRecommendations()
        {
            var expectedGoalsList = new List<MainCalculator.TeamStdDevAndAverage> {HomeExpectedGoals, AwayExpectedGoals};
            var totalGoals = expectedGoalsList.Sum(x => x.Average);
            var stdAverage = expectedGoalsList.Average(x => x.StdDev);
            var stdPunish = 0;
            if (stdAverage > 1.2)
            {
                stdPunish = (int) ((stdAverage - 1.2)*100);
            }

            if (totalGoals > 3)
            {
                var confidence = (totalGoals - 2.5) * 100 - stdPunish;
                return new MainCalculator.Recommendation
                {
                    Confidence = (decimal) confidence,
                    Result = "Over",
                    Type = "Goals",
                    HomeTeam = HomeTeamName,
                    AwayTeam = AwayTeamName
                };
            }

            if (totalGoals < 2)
            {
                var confidence = (2.5 -totalGoals) * 100 - stdPunish;
                return new MainCalculator.Recommendation
                {
                    Confidence = (decimal) confidence,
                    Result = "Under",
                    Type = "Goals",
                    HomeTeam = HomeTeamName,
                    AwayTeam = AwayTeamName
                };
            }

            return new MainCalculator.Recommendation();
        }
        public void FindRecommendations()
        {
            var matchRecommendations = FindWinnerRecommendations();
            var goalsRecommendations = FindGoalsRecommendations();
            MatchRecommendations = new List<MainCalculator.Recommendation>
            {
                matchRecommendations, goalsRecommendations
            };
        }

        public List<string> GetLinesToWrite()
        {
            var linesToWrite = new List<string>();

            linesToWrite.Add("Home Team: " + HomeTeamName);
            linesToWrite.Add("Away Team: " + AwayTeamName);
            linesToWrite.Add("");

            linesToWrite.Add($"Strength Expected Winner: {StrengthExpectedWinner.Winner} ({Math.Min(StrengthExpectedWinner.Confidence * 100, 100)}%)");
            linesToWrite.Add($"Expected Goals: {Math.Round(HomeExpectedGoals.Average + AwayExpectedGoals.Average, 2)} (Average Std: {Math.Round((HomeExpectedGoals.StdDev + AwayExpectedGoals.StdDev) / 2, 1)})");
            linesToWrite.Add("");

            linesToWrite.Add($"{HomeTeamName} Expected Result According to Form: {HomeTeamStats.LettersDistribution.Letter} ({Math.Round(HomeTeamStats.LettersDistribution.Percent, 2)}, Count: {HomeTeamStats.LettersDistribution.Count}, {HomeTeamStats.TeamLastThreeResults})");
            linesToWrite.Add($"{AwayTeamName} Expected Result According to Form: {AwayTeamStats.LettersDistribution.Letter} ({Math.Round(AwayTeamStats.LettersDistribution.Percent, 2)}, Count: {AwayTeamStats.LettersDistribution.Count}, {AwayTeamStats.TeamLastThreeResults})");
            linesToWrite.Add($"Letters Expected Results: {LettersExpectedWinner.Winner} ({LettersExpectedWinner.Percent}%)");
            linesToWrite.Add("");
            linesToWrite.Add("-----------------------");
            linesToWrite.Add("");

            /*
             * 
            linesToWrite.Add("Home team attributes: ");
            linesToWrite.AddRange(HomeTeamStats.TeamAttributesString);
            linesToWrite.Add("");

            linesToWrite.Add("Away Team attributes: ");
            linesToWrite.AddRange(AwayTeamStats.TeamAttributesString);
            linesToWrite.Add("");

            linesToWrite.Add($"Attributes Clashing Winner: {ClashingAttributesExpectedWinner.Winner} ({ClashingAttributesExpectedWinner.Percent}%)");
            linesToWrite.Add("");
            linesToWrite.Add("-----------------------");
            linesToWrite.Add("");
            */
            linesToWrite.Add($"{HomeTeamName} Top Scorer: {HomeTeamStats.TeamGoalsStats.TopScorer.Name} ({HomeTeamStats.TeamGoalsStats.TopScorer.Goals})");
            linesToWrite.Add($"{AwayTeamName} Top Scorer: {AwayTeamStats.TeamGoalsStats.TopScorer.Name} ({AwayTeamStats.TeamGoalsStats.TopScorer.Goals})");
            linesToWrite.Add("");
            linesToWrite.Add("-----------------------");
            linesToWrite.Add("");
            var homeTeamScoringPositions =
                HomeTeamStats.TeamGoalsStats.GoalsScoreDistribution.Select(x => x.Position + " => " + x.Goals).ToList();

            var awayTeamScoringPositions =
                AwayTeamStats.TeamGoalsStats.GoalsScoreDistribution.Select(x => x.Position + " => " + x.Goals).ToList();

            linesToWrite.Add($"{HomeTeamName} Scoring positions: ");
            linesToWrite.AddRange(homeTeamScoringPositions);
            linesToWrite.Add("");

            linesToWrite.Add($"{AwayTeamName} Scoring positions: ");
            linesToWrite.AddRange(awayTeamScoringPositions);
            linesToWrite.Add("");

            var homeTeamConcededPositions =
                HomeTeamStats.TeamGoalsStats.GoalsConcedeDistribution.Select(x => x.Position + " => " + x.Goals).ToList();

            var awayTeamConcededPositions =
                AwayTeamStats.TeamGoalsStats.GoalsConcedeDistribution.Select(x => x.Position + " => " + x.Goals).ToList();

            linesToWrite.Add($"{HomeTeamName} Conceded positions: ");
            linesToWrite.AddRange(homeTeamConcededPositions);
            linesToWrite.Add("");

            linesToWrite.Add($"{AwayTeamName} Conceded positions: ");
            linesToWrite.AddRange(awayTeamConcededPositions);

            return linesToWrite;
        }
    }

    public class TeamReportStats
    {
        public team Team;
        public Helper.LetterDistribution LettersDistribution;
        public string TeamForm;
        public string TeamLastThreeResults;
        public List<string> TeamAttributesString;
        public List<DataObjects.AttributeType> TeamAttributes;
        public TeamOverallBalance TeamOverallBalance;
        public TeamGoalsStats TeamGoalsStats;
        public Decimal TeamStrength => MainCalculator.CalculateTeamStrength((int)Team.MarketValue, TeamOverallBalance.TeamSeasonalPace, TeamOverallBalance.TeamLastGamesPace, TeamOverallBalance.TeamHomeOrAwayPace);

        public TeamReportStats(Dictionary<string, List<Helper.LetterDistribution>> lettersDict,
            Dictionary<int, List<DataObjects.AttributeType>> attributesDict,
            int competitionId, string teamName, sakilaEntities4 db, bool isHomeTeam)
        {
            Team = db.team.First(x => x.TeamName == teamName);
            var relevantMatches = MainCalculator.GetTeamLatesMatches(db, Team.TeamID, competitionId);

            TeamForm = LettersSequenceCalculator.GetTeamLatestSequence(db, Team.TeamID, competitionId);
            TeamOverallBalance = new TeamOverallBalance(Team, competitionId, isHomeTeam, relevantMatches);
            TeamGoalsStats = new TeamGoalsStats(Team, competitionId, isHomeTeam, db, relevantMatches);
            TeamAttributes = Helper.GetTeamAttributeFromDict(Team.TeamID, attributesDict);
            TeamAttributesString = Helper.GetTeamAttributesList((int)Team.TeamID, attributesDict);
            string homeTeamSeq;
            LettersDistribution = LettersSequenceCalculator.BuildSequenceStringExpectation(db, Team.TeamID,
                    competitionId, lettersDict, out homeTeamSeq);
            TeamLastThreeResults = homeTeamSeq;
        }
    }

    public class TeamOverallBalance
    {
        public TeamBalance TeamSeasonalBalance;
        public TeamBalance TeamLastGamesBalance;
        public TeamBalance TeamHomeOrAwayBalance;
        public decimal TeamSeasonalPace => PointsCalculator.CalculatePointPace(TeamSeasonalBalance);
        public decimal TeamLastGamesPace => PointsCalculator.CalculatePointPace(TeamLastGamesBalance);
        public decimal TeamHomeOrAwayPace => PointsCalculator.CalculatePointPace(TeamHomeOrAwayBalance);
        public TeamOverallBalance(team team, int competitionId, bool isHometeam, List<competitionmatch> relevantMatches)
        {
            TeamSeasonalBalance = PointsCalculator.GetTeamBalance(relevantMatches, team.TeamID, competitionId);
            TeamLastGamesBalance = PointsCalculator.GetTeamBalance(relevantMatches, team.TeamID, competitionId, 3);
            if (isHometeam)
            {
                TeamHomeOrAwayBalance = PointsCalculator.GetTeamBalanceHome(relevantMatches, team.TeamID, competitionId);
            }
            else
            {
                TeamHomeOrAwayBalance = PointsCalculator.GetTeamBalanceAway(relevantMatches, team.TeamID, competitionId);
            }
        }
    }

    public class TeamGoalsStats
    {
        public MainCalculator.TeamStdDevAndAverage TeamSeasonalGoalsScored;
        public MainCalculator.TeamStdDevAndAverage TeamLastGamesGoalsScored;
        public MainCalculator.TeamStdDevAndAverage TeamHomeOrAwayGoalsScored;
        public MainCalculator.TeamStdDevAndAverage TeamSeasonalGoalsConceded;
        public MainCalculator.TeamStdDevAndAverage TeamLastGamesGoalsConceded;
        public MainCalculator.TeamStdDevAndAverage TeamHomeOrAwayGoalsConceded;
        public GoalsCalculator.TopScorer TopScorer;
        public List<MainCalculator.PositionGoals> GoalsScoreDistribution;
        public List<MainCalculator.PositionGoals> GoalsConcedeDistribution;
        public TeamGoalsStats(team team, int competitionId, bool isHometeam, sakilaEntities4 db, List<competitionmatch> relevantMatches)
        {
            TeamSeasonalGoalsScored = GoalsCalculator.GetGoalsScoringAverage(relevantMatches, team.TeamID, competitionId);
            TeamLastGamesGoalsScored = GoalsCalculator.GetGoalsScoringAverage(relevantMatches, team.TeamID, competitionId, 3);
            TeamSeasonalGoalsConceded = GoalsCalculator.GetGoalsConcededAverage(relevantMatches, team.TeamID, competitionId);
            TeamLastGamesGoalsConceded = GoalsCalculator.GetGoalsConcededAverage(relevantMatches, team.TeamID, competitionId, 3);

            if (isHometeam)
            {
                TeamHomeOrAwayGoalsScored = GoalsCalculator.GetGoalsScoringAverageAtHome(relevantMatches, team.TeamID,competitionId);
                TeamHomeOrAwayGoalsConceded = GoalsCalculator.GetGoalsConcededAverageAtHome(relevantMatches, team.TeamID, competitionId);
            }
            else
            {
                TeamHomeOrAwayGoalsScored = GoalsCalculator.GetGoalsScoringAverageAtAway(relevantMatches, team.TeamID, competitionId);
                TeamHomeOrAwayGoalsConceded = GoalsCalculator.GetGoalsConcededAverageAtAway(relevantMatches, team.TeamID, competitionId);
            }

            TopScorer = GoalsCalculator.GetTeamTopScorer(db, team.TeamID, competitionId);
            GoalsScoreDistribution = GoalsCalculator.GetTeamScorersByPosition(db, team.TeamID, competitionId);
            GoalsConcedeDistribution = GoalsCalculator.GetTeamScorersAgainstByPosition(db, team.TeamID, competitionId);
        }
    }
}
