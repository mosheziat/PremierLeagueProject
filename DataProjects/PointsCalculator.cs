using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProjects
{
    public class PointsCalculator
    {
        public static decimal GetTeamFormInLatestMatches(sakilaEntities4 db, int teamId, int competitionId, int matchesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var latestMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, matchesToTake, endDate);
            var pointsAggregated = CalculatePointsForTeamInMatches(latestMatches, teamId);
            return Math.Round((decimal)pointsAggregated / latestMatches.Count, 2);
        }
        public static TeamBalance GetTeamBalance(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var latestMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, gamesToTake, endDate);
            var wins = latestMatches.Count(x => x.WinnerTeamID == teamId);
            var losts = latestMatches.Count(x => x.WinnerTeamID != null && x.WinnerTeamID != teamId);
            var draws = latestMatches.Count(x => x.WinnerTeamID == null);

            return new TeamBalance
            {
                Win = wins,
                Draw = draws,
                Lost = losts
            };
        }

        public static TeamBalance GetTeamBalance(List<competitionmatch> latestMatches, int teamId, int competitionId, int gamesToTake = 50, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            latestMatches = latestMatches.Take(gamesToTake).ToList();
            var wins = latestMatches.Count(x => x.WinnerTeamID == teamId);
            var losts = latestMatches.Count(x => x.WinnerTeamID != null && x.WinnerTeamID != teamId);
            var draws = latestMatches.Count(x => x.WinnerTeamID == null);

            return new TeamBalance
            {
                Win = wins,
                Draw = draws,
                Lost = losts
            };
        }

        public static TeamBalance GetTeamBalanceHome(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50, DateTime? endDate = null)
        {
            var latestMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, gamesToTake, endDate)
                .Where(x => x.HomeTeamID == teamId)
                .ToList();
            var wins = latestMatches.Count(x => x.WinnerTeamID == teamId);
            var losts = latestMatches.Count(x => x.WinnerTeamID != null && x.WinnerTeamID != teamId);
            var draws = latestMatches.Count(x => x.WinnerTeamID == null);

            return new TeamBalance
            {
                Win = wins,
                Draw = draws,
                Lost = losts
            };
        }

        public static TeamBalance GetTeamBalanceHome(List<competitionmatch> latestMatches, int teamId, int competitionId, int gamesToTake = 50, DateTime? endDate = null)
        {
            latestMatches = latestMatches.Where(x => x.HomeTeamID == teamId)
                .ToList();
            var wins = latestMatches.Count(x => x.WinnerTeamID == teamId);
            var losts = latestMatches.Count(x => x.WinnerTeamID != null && x.WinnerTeamID != teamId);
            var draws = latestMatches.Count(x => x.WinnerTeamID == null);

            return new TeamBalance
            {
                Win = wins,
                Draw = draws,
                Lost = losts
            };
        }

        public static TeamBalance GetTeamBalanceAway(sakilaEntities4 db, int teamId, int competitionId, int gamesToTake = 50, DateTime? endDate = null)
        {
            var latestMatches = MainCalculator.GetTeamLatesMatches(db, teamId, competitionId, gamesToTake, endDate)
                .Where(x => x.AwayTeamID == teamId)
                .ToList();
            var wins = latestMatches.Count(x => x.WinnerTeamID == teamId);
            var losts = latestMatches.Count(x => x.WinnerTeamID != null && x.WinnerTeamID != teamId);
            var draws = latestMatches.Count(x => x.WinnerTeamID == null);

            return new TeamBalance
            {
                Win = wins,
                Draw = draws,
                Lost = losts
            };
        }

        public static TeamBalance GetTeamBalanceAway(List<competitionmatch> latestMatches, int teamId, int competitionId, int gamesToTake = 50, DateTime? endDate = null)
        {
            latestMatches = latestMatches
                .Where(x => x.AwayTeamID == teamId)
                .ToList();
            var wins = latestMatches.Count(x => x.WinnerTeamID == teamId);
            var losts = latestMatches.Count(x => x.WinnerTeamID != null && x.WinnerTeamID != teamId);
            var draws = latestMatches.Count(x => x.WinnerTeamID == null);

            return new TeamBalance
            {
                Win = wins,
                Draw = draws,
                Lost = losts
            };
        }

        public static int CalculatePointsForTeamInMatches(List<competitionmatch> matches, int teamId)
        {
            var result = 0;
            var matchedOWon = matches.Where(x => x.WinnerTeamID == teamId).ToList();
            if (matchedOWon.Any())
            {
                result = matchedOWon.Count * 3;
            }

            var draws = matches.Where(x => x.WinnerTeamID == null).ToList();
            if (draws.Any())
            {
                result += draws.Count;
            }

            return result;
        }

        public static decimal CalculatePointPace(TeamBalance balance)
        {
            var games = balance.Win + balance.Lost + balance.Draw;
            var totalPoints = balance.Win * 3 + balance.Draw;
            if (games == 0)
            {
                return 0;
            }
            return Math.Round((decimal)totalPoints / games, 2);
        }

    }
    public class TeamBalance
    {
        public int Win;
        public int Lost;
        public int Draw;
    }

}
