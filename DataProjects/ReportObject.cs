using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProjects
{
    public class ReportObject
    {
        public static TeamReportStats HomeTeam;
        public static TeamReportStats AwayTeam;
        public Dictionary<string, List<Helper.LetterDistribution>> LettersDict;
        public Dictionary<int, List<DataObjects.AttributeType>> AttributesDict;
        public List<MainCalculator.AttributesMatch> AttributeClashingMap;
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
            using (var db = new sakilaEntities4())
            {
                var homeTeam = new TeamReportStats(LettersDict, AttributesDict, AttributeClashingMap, CompetitionId, homeTeamName, db, true);
                var awayTeam = new TeamReportStats(LettersDict, AttributesDict, AttributeClashingMap, CompetitionId, awayTeamName, db,false);
            }
        }
    }

    public class TeamReportStats
    {
        public static team Team;
        public static Helper.LetterDistribution LettersDistribution;
        public static string TeamForm;
        public static string TeamLastThreeResults;
        public static List<string> TeamAttributes;
        public static TeamOverallBalance TeamOverallBalance;
        public static TeamGoalsStats TeamGoalsStats;
        public static Decimal TeamStrength => MainCalculator.CalculateTeamStrength((int)Team.MarketValue, TeamOverallBalance.TeamSeasonalPace, TeamOverallBalance.TeamLastGamesPace, TeamOverallBalance.TeamHomeOrAwayPace);

        public TeamReportStats(Dictionary<string, List<Helper.LetterDistribution>> lettersDict,
            Dictionary<int, List<DataObjects.AttributeType>> attributesDict,
            List<MainCalculator.AttributesMatch> attributeClashingMap,
            int competitionId, string teamName, sakilaEntities4 db, bool isHomeTeam)
        {
            Team = db.team.First(x => x.TeamName == teamName);
            TeamForm = LettersSequenceCalculator.GetTeamLatestSequence(db, Team.TeamID, competitionId);
            TeamOverallBalance = new TeamOverallBalance(Team, competitionId, isHomeTeam, db);
            TeamGoalsStats = new TeamGoalsStats(Team, competitionId, isHomeTeam, db);
            TeamAttributes = Helper.GetTeamAttributesList((int)Team.TeamID, attributesDict);
            string homeTeamSeq;
            LettersDistribution = LettersSequenceCalculator.BuildSequenceStringExpectation(db, Team.TeamID,
                    competitionId, lettersDict, out homeTeamSeq);
            TeamLastThreeResults = homeTeamSeq;
        }
    }

    public class TeamOverallBalance
    {
        public static TeamBalance TeamSeasonalBalance;
        public static TeamBalance TeamLastGamesBalance;
        public static TeamBalance TeamHomeOrAwayBalance;
        public static decimal TeamSeasonalPace => PointsCalculator.CalculatePointPace(TeamSeasonalBalance);
        public static decimal TeamLastGamesPace => PointsCalculator.CalculatePointPace(TeamLastGamesBalance);
        public static decimal TeamHomeOrAwayPace => PointsCalculator.CalculatePointPace(TeamHomeOrAwayBalance);
        public TeamOverallBalance(team team, int competitionId, bool isHometeam, sakilaEntities4 db)
        {
            TeamSeasonalBalance = PointsCalculator.GetTeamBalance(db, team.TeamID, competitionId);
            TeamLastGamesBalance = PointsCalculator.GetTeamBalance(db, team.TeamID, competitionId);
            if (isHometeam)
            {
                TeamHomeOrAwayBalance = PointsCalculator.GetTeamBalanceHome(db, team.TeamID, competitionId);
            }
            else
            {
                TeamHomeOrAwayBalance = PointsCalculator.GetTeamBalanceAway(db, team.TeamID, competitionId);
            }
        }
    }

    public class TeamGoalsStats
    {
        public static MainCalculator.TeamStdDevAndAverage TeamSeasonalGoalsScored;
        public static MainCalculator.TeamStdDevAndAverage TeamLastGamesGoalsScored;
        public static MainCalculator.TeamStdDevAndAverage TeamHomeOrAwayGoalsScored;
        public static MainCalculator.TeamStdDevAndAverage TeamSeasonalGoalsConceded;
        public static MainCalculator.TeamStdDevAndAverage TeamLastGamesGoalsConceded;
        public static MainCalculator.TeamStdDevAndAverage TeamHomeOrAwayGoalsConceded;
        public static GoalsCalculator.TopScorer TopScorer;
        public static string MostGoalsConcededPosition;
        public TeamGoalsStats(team team, int competitionId, bool isHometeam, sakilaEntities4 db)
        {
            TeamSeasonalGoalsScored = GoalsCalculator.GetGoalsScoringAverage(db, team.TeamID, competitionId);
            TeamLastGamesGoalsScored = GoalsCalculator.GetGoalsScoringAverage(db, team.TeamID, competitionId, 3);
            if (isHometeam)
            {
                TeamHomeOrAwayGoalsScored = GoalsCalculator.GetGoalsScoringAverageAtHome(db, team.TeamID,competitionId);
            }
            else
            {
                TeamHomeOrAwayGoalsScored = GoalsCalculator.GetGoalsScoringAverageAtAway(db, team.TeamID, competitionId);
            }

            TeamSeasonalGoalsConceded = GoalsCalculator.GetGoalsConcededAverage(db, team.TeamID, competitionId);
            TeamLastGamesGoalsConceded = GoalsCalculator.GetGoalsConcededAverage(db, team.TeamID, competitionId, 3);
            if (isHometeam)
            {
                TeamHomeOrAwayGoalsConceded = GoalsCalculator.GetGoalsConcededAverageAtHome(db, team.TeamID, competitionId);
            }
            else
            {
                TeamHomeOrAwayGoalsConceded = GoalsCalculator.GetGoalsConcededAverageAtAway(db, team.TeamID, competitionId);
            }
        }

    }
}
