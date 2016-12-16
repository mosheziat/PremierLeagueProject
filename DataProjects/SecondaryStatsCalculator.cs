using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProjects
{
    public class SecondaryStatsCalculator
    {
        public static double? GetAverageTotalShotsPerTeam(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50, DateTime? endDate = null)
        {
            return GetAverageEventValue(db, teamId, competitionId, (int)DataObjects.EventType.TotalShots, gamesToTake, endDate);
        }
        public static double? GetAverageShotsOnTargetAgainstTeam(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50, DateTime? endDate = null)
        {
            return GetAverageEventValueAgainstTeam(db, teamId, competitionId, (int)DataObjects.EventType.TotalShots, gamesToTake, endDate);
        }

        public static Dictionary<int, List<DataObjects.AttributeType>> BuildAttributesDictionary(int competitionId, int gamesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            using (var db = new sakilaEntities4())
            {
                var foulsTeamsList = GetTeamsListPerEvent(db, competitionId, (int)DataObjects.EventType.Fouls, gamesToTake,
                    endDate);
                var toughTeams = foulsTeamsList.Take(4);
                var dictToReturn = toughTeams.ToDictionary(team => team, team => new List<DataObjects.AttributeType>
            {
                DataObjects.AttributeType.Tough
            });

                var softTeams = foulsTeamsList.TakeLast(4);
                foreach (var softTeam in softTeams)
                {
                    dictToReturn.Add(softTeam, new List<DataObjects.AttributeType>
                {
                    DataObjects.AttributeType.Soft
                });
                }

                var possessionList = GetTeamsListPerEvent(db, competitionId, (int)DataObjects.EventType.Possession, gamesToTake,
                    endDate);

                var dominantTeams = possessionList.Take(4);
                foreach (var team in dominantTeams)
                {
                    if (dictToReturn.ContainsKey(team))
                    {
                        dictToReturn.First(x => x.Key == team).Value.Add(DataObjects.AttributeType.Dominant);
                    }
                    else
                    {
                        dictToReturn.Add(team, new List<DataObjects.AttributeType>
                    {
                        DataObjects.AttributeType.Dominant
                    });
                    }
                }

                var passiveTeams = possessionList.TakeLast(4);
                foreach (var team in passiveTeams)
                {
                    if (dictToReturn.ContainsKey(team))
                    {
                        dictToReturn.First(x => x.Key == team).Value.Add(DataObjects.AttributeType.Passive);
                    }
                    else
                    {
                        dictToReturn.Add(team, new List<DataObjects.AttributeType>
                    {
                        DataObjects.AttributeType.Passive
                    });
                    }
                }

                var shotsOnTargetList = GetTeamsListPerEvent(db, competitionId, (int)DataObjects.EventType.ShotsOnTarget, gamesToTake,
                    endDate);

                var dangerousTeams = shotsOnTargetList.Take(4);
                foreach (var team in dangerousTeams)
                {
                    if (dictToReturn.ContainsKey(team))
                    {
                        dictToReturn.First(x => x.Key == team).Value.Add(DataObjects.AttributeType.Dangerous);
                    }
                    else
                    {
                        dictToReturn.Add(team, new List<DataObjects.AttributeType>
                    {
                        DataObjects.AttributeType.Dangerous
                    });
                    }
                }

                var anemicTeams = shotsOnTargetList.TakeLast(4);
                foreach (var team in anemicTeams)
                {
                    if (dictToReturn.ContainsKey(team))
                    {
                        dictToReturn.First(x => x.Key == team).Value.Add(DataObjects.AttributeType.Anemic);
                    }
                    else
                    {
                        dictToReturn.Add(team, new List<DataObjects.AttributeType>
                    {
                        DataObjects.AttributeType.Anemic
                    });
                    }
                }

                var accurateInFrontOfGoalList = GetTeamsListAccuracyInFrontOfGoal(db, competitionId, 50, endDate);

                var accurateTeams = accurateInFrontOfGoalList.TakeLast(4);
                foreach (var team in accurateTeams)
                {
                    if (dictToReturn.ContainsKey(team))
                    {
                        dictToReturn.First(x => x.Key == team).Value.Add(DataObjects.AttributeType.Accurate_In_Front_Of_Goal);
                    }
                    else
                    {
                        dictToReturn.Add(team, new List<DataObjects.AttributeType>
                    {
                        DataObjects.AttributeType.Accurate_In_Front_Of_Goal
                    });
                    }
                }

                var inaccurateTeams = accurateInFrontOfGoalList.Take(4);
                foreach (var team in inaccurateTeams)
                {
                    if (dictToReturn.ContainsKey(team))
                    {
                        dictToReturn.First(x => x.Key == team).Value.Add(DataObjects.AttributeType.Inaccurate_In_Front_Of_Goal);
                    }
                    else
                    {
                        dictToReturn.Add(team, new List<DataObjects.AttributeType>
                    {
                        DataObjects.AttributeType.Inaccurate_In_Front_Of_Goal
                    });
                    }
                }

                var accurateInFrontOfGoalAgainst = GetTeamsListAccuracyInFrontOfGoalAgainst(db, competitionId, 50, endDate);
                var goodGoalkeeperTeams = accurateInFrontOfGoalAgainst.Take(4);
                foreach (var team in goodGoalkeeperTeams)
                {
                    if (dictToReturn.ContainsKey(team))
                    {
                        dictToReturn.First(x => x.Key == team).Value.Add(DataObjects.AttributeType.Good_Goalkeeper);
                    }
                    else
                    {
                        dictToReturn.Add(team, new List<DataObjects.AttributeType>
                    {
                        DataObjects.AttributeType.Good_Goalkeeper
                    });
                    }
                }

                var poorGoalkeeperTeams = accurateInFrontOfGoalAgainst.TakeLast(4);
                foreach (var team in poorGoalkeeperTeams)
                {
                    if (dictToReturn.ContainsKey(team))
                    {
                        dictToReturn.First(x => x.Key == team).Value.Add(DataObjects.AttributeType.Poor_Keeper);
                    }
                    else
                    {
                        dictToReturn.Add(team, new List<DataObjects.AttributeType>
                    {
                        DataObjects.AttributeType.Poor_Keeper
                    });
                    }
                }

                return dictToReturn;
            }
           
        }
        public static List<int> GetTeamsListPerEvent(sakilaEntities4 db, int competitionId, int eventTypeId,
            int gamesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var allEvents = db.matchevent.Where(x => x.competitionmatch.CompetitionID == competitionId)
                .Where(x => x.EventTypeID == eventTypeId)
                .Where(x => x.competitionmatch.MatchDate < endDate.Value);

            var groupByTeams = allEvents.GroupBy(x => x.TeamID)
                .Select(x => new {TeamID = x.Key, Average = x.Average(y => y.eventvalue)})
                .OrderByDescending(x => x.Average)
                .Select(x => x.TeamID)
                .ToList();

            return groupByTeams;
        }

        public static List<int> GetTeamsListAccuracyInFrontOfGoal(sakilaEntities4 db, int competitionId,
            int gamesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var participatedTeams = Helper.GetParticipatedTeamList(db, competitionId);
            var averageEventList = new List<TeamAverageEvent>();

            //Fill Table
            foreach (var teamId in participatedTeams)
            {
                var shotsOnTargetAverage = GetAverageTotalShotsPerTeam(db, teamId, competitionId, 50, endDate);
                var goalScoringAverage = GoalsCalculator.GetGoalsScoringAverage(db, teamId, competitionId, 50, endDate);
                var rate = Math.Round((double)(shotsOnTargetAverage / goalScoringAverage.Average), 2);
                var newItem = new TeamAverageEvent
                {
                    TeamID = teamId,
                    Rate = rate
                };
                averageEventList.Add(newItem);
            }

            var orderedTeamList = averageEventList.OrderByDescending(x => x.Rate);
            return orderedTeamList.Select(x => x.TeamID).ToList();
        }

        public static List<TeamAverageEvent> GetTeamNamesListAccuracyInFrontOfGoal(sakilaEntities4 db, int competitionId,
            int gamesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var participatedTeams = Helper.GetParticipatedTeamList(db, competitionId);
            var averageEventList = new List<TeamAverageEvent>();

            //Fill Table

            foreach (var teamId in participatedTeams)
            {
                var shotsOnTargetAverage = GetAverageTotalShotsPerTeam(db, teamId, competitionId, 50, endDate);
                var goalScoringAverage = GoalsCalculator.GetGoalsScoringAverage(db, teamId, competitionId, 50, endDate);
                var rate = Math.Round((double)(shotsOnTargetAverage / goalScoringAverage.Average), 2);
                var teamName = db.team.First(x => x.TeamID == teamId).TeamName;
                var newItem = new TeamAverageEvent
                {
                    TeamID = teamId,
                    Rate = rate,
                    TeamName = teamName
                };
                averageEventList.Add(newItem);
            }

            var orderedTeamList = averageEventList.OrderByDescending(x => x.Rate).ToList();
            return orderedTeamList;
        }

        public static List<int> GetTeamsListAccuracyInFrontOfGoalAgainst(sakilaEntities4 db, int competitionId,
                    int gamesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var participatedTeams = Helper.GetParticipatedTeamList(db, competitionId);
            var averageEventList = new List<TeamAverageEvent>();

            //Fill Table
            foreach (var teamId in participatedTeams)
            {
                var shotsOnTargetAverageAgainstTeam = GetAverageShotsOnTargetAgainstTeam(db, teamId, competitionId, 50, endDate);
                var goalConcedingAverage = GoalsCalculator.GetGoalsConcededAverage(db, teamId, competitionId,50, endDate);
                var rate = Math.Round((double)(shotsOnTargetAverageAgainstTeam / goalConcedingAverage.Average), 2);
                var newItem = new TeamAverageEvent
                {
                    TeamID = teamId,
                    Rate = rate
                };
                averageEventList.Add(newItem);
            }

            var orderedTeamList = averageEventList.OrderByDescending(x => x.Rate);
            return orderedTeamList.Select(x => x.TeamID).ToList();
        }

        public static List<TeamAverageEvent> GetTeamNamesListAccuracyInFrontOfGoalAgainst(sakilaEntities4 db, int competitionId,
            int gamesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var participatedTeams = Helper.GetParticipatedTeamList(db, competitionId);
            var averageEventList = new List<TeamAverageEvent>();

            //Fill Table
            foreach (var teamId in participatedTeams)
            {
                var shotsOnTargetAverageAgainstTeam = GetAverageShotsOnTargetAgainstTeam(db, teamId, competitionId, 50, endDate);
                var goalConcedingAverage = GoalsCalculator.GetGoalsConcededAverage(db, teamId, competitionId, 50, endDate);
                var rate = Math.Round((double)(shotsOnTargetAverageAgainstTeam / goalConcedingAverage.Average), 2);
                var teamName = db.team.First(x => x.TeamID == teamId).TeamName;
                var newItem = new TeamAverageEvent
                {
                    TeamID = teamId,
                    Rate = rate,
                    TeamName = teamName
                };
                averageEventList.Add(newItem);
            }

            var orderedTeamList = averageEventList.OrderByDescending(x => x.Rate).ToList();
            return orderedTeamList;
        }

        public static double? GetAveragePossessionPerTeam(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50)
        {
            return GetAverageEventValue(db, teamId, competitionId, (int)DataObjects.EventType.Possession, gamesToTake);
        }
        public static double? GetAveragePossessionAgainstTeam(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50)
        {
            return GetAverageEventValueAgainstTeam(db, teamId, competitionId, (int)DataObjects.EventType.Possession, gamesToTake);
        }
        public static double? GetAverageEventValue(sakilaEntities4 db, int teamId, int competitionId, int eventTypeId, int gamesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var average =
                db.matchevent.Where(
                    x =>
                        x.EventTypeID == eventTypeId &&
                        x.TeamID == teamId &&
                        x.competitionmatch.CompetitionID == competitionId &&
                        x.eventvalue != null
                        && x.competitionmatch.MatchDate < endDate)
                        .OrderByDescending(x => x.competitionmatch.MatchDate)
                        .Take(gamesToTake)
                        .Select(x => x.eventvalue)
                        .Average();

            return average;
        }
        public static double? GetAverageEventValueAgainstTeam(sakilaEntities4 db, int teamId, int competitionId, int eventTypeId, int gamesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var matches = db.competitionmatch
                .Where(x => x.CompetitionID == competitionId)
                .Where(x => x.HomeTeamID == teamId || x.AwayTeamID == teamId)
                .Where(x => x.MatchDate < endDate)
                .OrderByDescending(x => x.MatchDate)
                .Take(gamesToTake)
                .Select(x => x.CompetitionMatchID);

            var average =
                db.matchevent.Where(
                    x =>
                        x.EventTypeID == eventTypeId &&
                        x.TeamID != teamId &&
                        x.competitionmatch.CompetitionID == competitionId &&
                        x.eventvalue != null &&
                        matches.Contains(x.MatchID))
                        .OrderByDescending(x => x.competitionmatch.MatchDate)
                        .Select(x => x.eventvalue)
                        .Average();

            return average;
        }

        public static double GetStdEventValuePerTeamMatches(sakilaEntities4 db, int teamId, int competitionId, int eventTypeId, int gamesToTake = 50)
        {

            var matchesIds = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, gamesToTake)
                            .Select(x => x.CompetitionMatchID)
                            .ToList();

            var values =
                db.matchevent.Where(
                    x =>
                        x.EventTypeID == eventTypeId &&
                        matchesIds.Contains(x.MatchID) &&
                        x.competitionmatch.CompetitionID == competitionId &&
                        x.eventvalue != null)
                    .OrderByDescending(x => x.competitionmatch.MatchDate)
                    .Take(gamesToTake)
                    .Select(x => (int)x.eventvalue)
                    .ToList();

            return Math.Round(MainCalculator.CalculateStdDev(values), 2);
        }
        public static double? GetAverageEventValuePerTeamMatches(sakilaEntities4 db, int teamId, int competitionId, int eventTypeId, int gamesToTake = 50)
        {

            var matchesIds = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, gamesToTake)
                            .Select(x => x.CompetitionMatchID)
                            .ToList();



            var average =
                db.matchevent.Where(
                    x =>
                        x.EventTypeID == eventTypeId &&
                        matchesIds.Contains(x.MatchID) &&
                        x.competitionmatch.CompetitionID == competitionId &&
                        x.eventvalue != null)
                        .OrderByDescending(x => x.competitionmatch.MatchDate)
                        .Take(gamesToTake)
                        .Select(x => x.eventvalue)
                        .Average() * 2;

            return average;
        }
        public static double GetStdValueForTeamInEvent(sakilaEntities4 db, int teamId, int competitionId, int eventTypeId,
            int gamesToTake = 50)
        {
            var values =
                db.matchevent.Where(
                    x =>
                        x.EventTypeID == eventTypeId &&
                        x.TeamID == teamId &&
                        x.competitionmatch.CompetitionID == competitionId &&
                        x.eventvalue != null)
                    .OrderByDescending(x => x.competitionmatch.MatchDate)
                    .Take(gamesToTake)
                    .Select(x => (int)x.eventvalue)
                    .ToList();

            return MainCalculator.CalculateStdDev(values);
        }
        public static double? GetAverageCornersPerTeam(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50)
        {
            return GetAverageEventValue(db, teamId, competitionId, (int)DataObjects.EventType.Corner, gamesToTake);
        }
        public static double GetStdCornersPerTeamsMatches(sakilaEntities4 db, int teamId, int competitionId,
            int gamesToTake = 50)
        {
            return GetStdEventValuePerTeamMatches(db, teamId, competitionId, (int)DataObjects.EventType.Corner, gamesToTake);
        }
        public static double? GetAverageFoulsPerTeam(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50)
        {
            return GetAverageEventValue(db, teamId, competitionId, (int)DataObjects.EventType.Fouls, gamesToTake);
        }

        public static void PrintTableOfAccuracyInFrontOfGoal(int competitionId)
        {
            var path = $@"C:\Users\user\Desktop\DataProjects\EventTableAccuracyInFrontOfGoal.tsv";
            using (var db = new sakilaEntities4())
            {
                var table = GetTeamNamesListAccuracyInFrontOfGoal(db, competitionId);
                var linesToWrite = table.Select(x => x.TeamName + "\t" + x.Rate).ToList();
                File.WriteAllLines(path, linesToWrite);
            }
        }

        public static void PrintTableOfAccuracyInFrontOfGoalAgainst(int competitionId)
        {
            var path = $@"C:\Users\user\Desktop\DataProjects\EventTableAccuracyInFrontOfGoalAgainst.tsv";
            using (var db = new sakilaEntities4())
            {
                var table = GetTeamNamesListAccuracyInFrontOfGoalAgainst(db, competitionId);
                var linesToWrite = table.Select(x => x.TeamName + "\t" + x.Rate).ToList();
                File.WriteAllLines(path, linesToWrite);
            }
        }

        public static void PrintTableOfEvent(int competitionId, DataObjects.EventType eventType, int gamesToTake = 50,
            DateTime? endDate = null)
        {
            var eventTypeId = (int) eventType;
            var path = $@"C:\Users\user\Desktop\DataProjects\EventTable{eventType}.tsv";
            if (!endDate.HasValue)
                endDate = DateTime.Now;
            using (var db = new sakilaEntities4())
            {
                var allEvents = db.matchevent.Where(x => x.competitionmatch.CompetitionID == competitionId)
                .Where(x => x.EventTypeID == eventTypeId)
                .Where(x => x.competitionmatch.MatchDate < endDate.Value);

                var groupByTeams = allEvents.GroupBy(x => x.team.TeamName)
                    .Select(x => new { TeamName = x.Key, Average = x.Average(y => y.eventvalue)})
                    .ToList()
                    .OrderByDescending(x => x.Average)
                    .Select(x => x.TeamName + "\t" + Math.Round((double) x.Average, 2))
                    .ToList();

                File.WriteAllLines(path, groupByTeams);
            }
        }
        public static double? AverageCornersOnTeamsMatches(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50)
        {
            return GetAverageEventValuePerTeamMatches(db, teamId, competitionId, (int)DataObjects.EventType.Corner,
                gamesToTake);
        }
        public static void GetAllStdDevForEvent(int eventTyepId, string eventName, int competitionId)
        {
            var l = new List<MainCalculator.TeamStdDevAndAverage>();
            using (var db = new sakilaEntities4())
            {
                var teams = db.team.Where(x => x.TeamTypeID == 1 && x.TeamName != "NA").ToList();
                foreach (var team in teams)
                {
                    var allValues =
                        db.matchevent.Where(
                            x =>
                                x.EventTypeID == eventTyepId &&
                                x.TeamID == team.TeamID &&
                                x.competitionmatch.CompetitionID == competitionId &&
                                x.eventvalue != null)
                                    .Select(x => x.eventvalue).ToList();
                    if (!allValues.Any())
                    {
                        continue;
                    }

                    var avg = (double)allValues.Average();
                    var result = MainCalculator.CalculateStdDev(allValues.Select(x => x.Value).ToList());
                    l.Add(new MainCalculator.TeamStdDevAndAverage { StdDev = result, TeamName = team.TeamName, Average = avg });
                }
            }

            var path = @"C:\Users\user\Desktop\DataProjects\AverageAndStdDev" + eventName + ".tsv";
            File.WriteAllLines(path, l.OrderByDescending(x => x.Average).Select(x => x.TeamName + "\t" + Math.Round(x.Average, 2) + "\t" + Math.Round(x.StdDev, 2)));
        }
        public class TeamAverageEvent
        {
            public int TeamID;
            public double Rate;
            public string TeamName;
        }
    }
}
