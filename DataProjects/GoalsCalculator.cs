using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProjects
{
    public class GoalsCalculator
    {
        public static List<int> GetTeamGoals(int teamId)
        {
            using (var db = new sakilaEntities4())
            {
                var allHomeMtaches = db.competitionmatch.Where(x => x.HomeTeamID == teamId).Select(x => x.HomeGoals).ToList();
                var allAwayMatches = db.competitionmatch.Where(x => x.AwayTeamID == teamId).Select(x => x.AwayGoals).ToList();
                allHomeMtaches.AddRange(allAwayMatches);
                return allHomeMtaches;
            }
        }
        public static MainCalculator.TeamStdDevAndAverage GetGoalsStdDevForTeam(team team)
        {
            var allValues = GetTeamGoals(team.TeamID);
            var avg = (double)allValues.Average();
            var result = MainCalculator.CalculateStdDev(allValues.ToList());
            return new MainCalculator.TeamStdDevAndAverage { StdDev = result, TeamName = team.TeamName, Average = avg };
        }
        public static void GetAllGoalsStdDev()
        {
            var l = new List<MainCalculator.TeamStdDevAndAverage>();
            using (var db = new sakilaEntities4())
            {
                var teams = db.team.Where(x => x.TeamTypeID == 2 && x.TeamName != "NA").ToList();
                foreach (var team in teams)
                {
                    l.Add(GetGoalsStdDevForTeam(team));
                }
            }

            var path = @"C:\Users\user\Desktop\DataProjects\AverageAndStdDevGoals.tsv";
            File.WriteAllLines(path, l.OrderByDescending(x => x.Average).Select(x => x.TeamName + "\t" + Math.Round(x.Average, 2) + "\t" + Math.Round(x.StdDev, 2)));
        }
        public static decimal GetAverageGoalForCompetition(int competitionId)
        {
            using (var db = new sakilaEntities4())
            {
                var matches = MainCalculator.GetAllMatchesForCompetition(competitionId, db);
                var sumHomeGoals = matches.Sum(x => x.HomeGoals);
                var sumAwayGoals = matches.Sum(x => x.AwayGoals);
                return (decimal)(sumAwayGoals + sumHomeGoals) / matches.Count;
            }
        }
        public static MainCalculator.TeamStdDevAndAverage GetGoalsScoringAverage(sakilaEntities4 db, int teamId,
                    int competitionId, int matchesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var relevantMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, matchesToTake, endDate);
            var homeGoalsValues = relevantMatches.Where(x => x.HomeTeamID == teamId).Select(x => x.HomeGoals).ToList();
            var awayGoalsValues = relevantMatches.Where(x => x.AwayTeamID == teamId).Select(x => x.AwayGoals).ToList();
            var allValues = homeGoalsValues.Concat(awayGoalsValues).ToList();
            var avg = Math.Round(allValues.Average(), 2);
            var result = Math.Round(MainCalculator.CalculateStdDev(allValues.ToList()), 2);
            return new MainCalculator.TeamStdDevAndAverage { StdDev = result, Average = avg };
        }
        public static MainCalculator.TeamStdDevAndAverage GetGoalsScoringAverageAtHome(sakilaEntities4 db, int teamId,
            int competitionId, int matchesToTake = 50)
        {

            var relevantMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, matchesToTake).Where(x => x.HomeTeamID == teamId).ToList();
            var homeGoalsValues = relevantMatches.Where(x => x.HomeTeamID == teamId).Select(x => x.HomeGoals).ToList();
            var awayGoalsValues = relevantMatches.Where(x => x.AwayTeamID == teamId).Select(x => x.AwayGoals).ToList();
            var allValues = homeGoalsValues.Concat(awayGoalsValues).ToList();
            var avg = Math.Round(allValues.Average(), 2);
            var result = Math.Round(MainCalculator.CalculateStdDev(allValues.ToList()), 2);
            return new MainCalculator.TeamStdDevAndAverage { StdDev = result, Average = avg };
        }
        public static MainCalculator.TeamStdDevAndAverage GetGoalsScoringAverageAtAway(sakilaEntities4 db, int teamId,
                int competitionId, int matchesToTake = 50)
        {

            var relevantMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, matchesToTake).Where(x => x.AwayTeamID == teamId).ToList();
            var homeGoalsValues = relevantMatches.Where(x => x.HomeTeamID == teamId).Select(x => x.HomeGoals).ToList();
            var awayGoalsValues = relevantMatches.Where(x => x.AwayTeamID == teamId).Select(x => x.AwayGoals).ToList();
            var allValues = homeGoalsValues.Concat(awayGoalsValues).ToList();
            var avg = Math.Round(allValues.Average(), 2);
            var result = Math.Round(MainCalculator.CalculateStdDev(allValues.ToList()), 2);
            return new MainCalculator.TeamStdDevAndAverage { StdDev = result, Average = avg };
        }
        public static MainCalculator.TeamStdDevAndAverage GetGoalsConcededAverage(sakilaEntities4 db, int teamId,
                int competitionId, int matchesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var relevantMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, matchesToTake, endDate);
            var homeGoalsValues = relevantMatches.Where(x => x.HomeTeamID == teamId).Select(x => x.AwayGoals).ToList();
            var awayGoalsValues = relevantMatches.Where(x => x.AwayTeamID == teamId).Select(x => x.HomeGoals).ToList();
            var allValues = homeGoalsValues.Concat(awayGoalsValues).ToList();
            var avg = Math.Round(allValues.Average(), 2);
            var result = Math.Round(MainCalculator.CalculateStdDev(allValues.ToList()), 2);
            return new MainCalculator.TeamStdDevAndAverage { StdDev = result, Average = avg };
        }
        public static MainCalculator.TeamStdDevAndAverage GetGoalsConcededAverageAtHome(sakilaEntities4 db, int teamId,
        int competitionId, int matchesToTake = 50)
        {
            var relevantMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, matchesToTake);
            var homeGoalsValues = relevantMatches.Where(x => x.HomeTeamID == teamId).Select(x => x.AwayGoals).ToList();
            var avg = Math.Round(homeGoalsValues.Average(), 2);
            var result = Math.Round(MainCalculator.CalculateStdDev(homeGoalsValues.ToList()), 2);
            return new MainCalculator.TeamStdDevAndAverage { StdDev = result, Average = avg };
        }
        public static MainCalculator.TeamStdDevAndAverage GetGoalsConcededAverageAtAway(sakilaEntities4 db, int teamId,
                    int competitionId, int matchesToTake = 50)
        {
            var relevantMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, matchesToTake);
            var awayGoalsValues = relevantMatches.Where(x => x.AwayTeamID == teamId).Select(x => x.HomeGoals).ToList();
            var avg = Math.Round(awayGoalsValues.Average(), 2);
            var result = Math.Round(MainCalculator.CalculateStdDev(awayGoalsValues.ToList()), 2);
            return new MainCalculator.TeamStdDevAndAverage { StdDev = result, Average = avg };
        }

        public static MainCalculator.TeamStdDevAndAverage CalculateExpectedGoals(MainCalculator.TeamStdDevAndAverage sesonalGoals, MainCalculator.TeamStdDevAndAverage latestGoal,
                MainCalculator.TeamStdDevAndAverage seasonalGoalConceded, MainCalculator.TeamStdDevAndAverage latestGoalConceded,
                MainCalculator.TeamStdDevAndAverage goalScoredAtHomeOrAway, MainCalculator.TeamStdDevAndAverage goalsConcededAtAwayOrHome)
        {
            var goalScoring = sesonalGoals.Average * 0.2 + latestGoal.Average * 0.4 + goalScoredAtHomeOrAway.Average * 0.4;
            var goalConceded = seasonalGoalConceded.Average * 0.2 + latestGoalConceded.Average * 0.4 + goalsConcededAtAwayOrHome.Average * 0.4;

            var av = goalScoring*0.6 + goalConceded*0.4;
            var std = (sesonalGoals.StdDev + latestGoal.StdDev + goalScoredAtHomeOrAway.StdDev
                       + seasonalGoalConceded.StdDev + latestGoalConceded.StdDev + goalsConcededAtAwayOrHome.StdDev)/6;

            return new MainCalculator.TeamStdDevAndAverage
            {
                Average = av,
                StdDev = std
            };
        }
        public static string GetTeamTopScorersAgainsePosition(sakilaEntities4 db, int teamId, int competitionId,
                                                              int gamesToTake = 50)
        {
            var latestMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, gamesToTake);
            var matchesIds = latestMatches.Select(x => x.CompetitionMatchID);

            var teamGoalsConceded = db.matchgoal
                .Where(x => matchesIds.Contains(x.MatchID) && x.TeamID != teamId)
                .ToList();

            var groupByPosition = teamGoalsConceded
                .GroupBy(x => x.player.PositionID)
                .OrderByDescending(x => x.Count())
                .Select(x => x.First().player.PositionID)
                .First();

            var positionName = db.playerposition.First(x => x.PlayerPositionID == groupByPosition).PlayerPositionName;
            var percent = (decimal)teamGoalsConceded.Where(x => x.player.PositionID == groupByPosition).Count()/
                          teamGoalsConceded.Count;
            return positionName + " (" + Math.Round(percent * 100, 2) + ")";

        }

        public static void PrintGoalScroingTable(int competitionId, int gamesToTake = 50, DateTime? endDate = null)
        {
            var path = @"C:\Users\user\Desktop\DataProjects\EventTableGoalScoring.tsv";
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var aggregatedGoals = new List<Tuple<string, int>>();
            using (var db = new sakilaEntities4())
            {
                var allTeamIds = db.competitionmatch.Where(x => x.CompetitionID == competitionId)
                    .Select(x => x.HomeTeamID).Distinct().ToList();

                foreach (var teamId in allTeamIds)
                {
                    var teamScoredAtHome = db.competitionmatch
                        .Where(x => x.CompetitionID == competitionId)
                        .Where(x => x.HomeTeamID == teamId)
                        .Sum(x => x.HomeGoals);
                    var teamScoredAway = db.competitionmatch
                        .Where(x => x.CompetitionID == competitionId)
                        .Where(x => x.AwayTeamID == teamId)
                        .Sum(x => x.AwayGoals);

                    var allGoals = teamScoredAway + teamScoredAtHome;
                    var teamName = db.team.First(x => x.TeamID == teamId).TeamName;
                    var newItem = new Tuple<string, int>(teamName, allGoals);
                    aggregatedGoals.Add(newItem);
                }
            }

            var linesToWrite = aggregatedGoals.OrderByDescending(x => x.Item2)
                .Select(x => x.Item1 + "\t" + x.Item2);

            File.WriteAllLines(path, linesToWrite);
        }


        public static void PrintGoalConcededTable(int competitionId, int gamesToTake = 50, DateTime? endDate = null)
        {
            var path = @"C:\Users\user\Desktop\DataProjects\EventTableGoalConceding.tsv";
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var aggregatedGoals = new List<Tuple<string, int>>();
            using (var db = new sakilaEntities4())
            {
                var allTeamIds = db.competitionmatch.Where(x => x.CompetitionID == competitionId)
                    .Select(x => x.HomeTeamID).Distinct().ToList();

                foreach (var teamId in allTeamIds)
                {
                    var teamScoredAtHome = db.competitionmatch
                        .Where(x => x.CompetitionID == competitionId)
                        .Where(x => x.HomeTeamID == teamId)
                        .Sum(x => x.AwayGoals);
                    var teamScoredAway = db.competitionmatch
                        .Where(x => x.CompetitionID == competitionId)
                        .Where(x => x.AwayTeamID == teamId)
                        .Sum(x => x.HomeGoals);

                    var allGoals = teamScoredAway + teamScoredAtHome;
                    var teamName = db.team.First(x => x.TeamID == teamId).TeamName;
                    var newItem = new Tuple<string, int>(teamName, allGoals);
                    aggregatedGoals.Add(newItem);
                }
            }

            var linesToWrite = aggregatedGoals.OrderBy(x => x.Item2)
                .Select(x => x.Item1 + "\t" + x.Item2);

            File.WriteAllLines(path, linesToWrite);
        }



        public static TopScorer GetTeamTopScorer(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50)
        {
            var latestMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, gamesToTake);
            var matchesIds = latestMatches.Select(x => x.CompetitionMatchID);

            var teamGoals = db.matchgoal
                .Where(x => matchesIds.Contains(x.MatchID) && x.TeamID == teamId)
                .ToList();

            var mostGoals = teamGoals
                .GroupBy(x => x.ScorerID)
                .OrderByDescending(x => x.Count());

            var topScorerID = mostGoals
                .Select(x => x.First().ScorerID)
                .First();

            var player = db.player.First(x => x.PlayerID == topScorerID);

            var positionName = db.playerposition.First(x => x.PlayerPositionID == player.PositionID).PlayerPositionName;

            return new TopScorer
            {
                Name = player.PlayerName,
                Goals = mostGoals.First().Count(),
                Position = positionName
            };
        }

        public class TopScorer
        {
            public string Name;
            public string Position;
            public int Goals;
        }
    }
}
