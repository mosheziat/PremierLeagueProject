using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProjects
{
    public class DataObjects
    {
        public class FileTeamDetails
        {
            public string Name;
            public int Games;
            public int Points;
            public int TotalGoalsScored;
            public int AverageGoalsScored;
            public int FirstHalfGoalsScored;
            public int SecondHalfGoalsScored;
            public int TotalGoalsConceded;
            public int AverageGoalsConceded;
            public int FirstHalfGoalsConceded;
            public int SecondHalfGoalsConceded;
            public int OffsidesTotal;
            public int OffsidesAverage;
            public int OffsidesOf;
            public int OffsidesAgainst;
            public int CornersTotal;
            public int CornersAverage;
            public int CornersOf;
            public int CornersAgainst;
            public int TotalShots;
            public int ShotsOnTarget;
            public int TargetGoalsPercent;
            public int ShotsOffTarget;
            public int TargetOffTragetPercent;
            public int TotalShotsAgainst;
            public int ShotsOnTargetAgainst;
            public int TargetGoalsPercentAgainst;
            public int ShotsOffTargetAgainst;
            public int TargetOffTragetPercentAgainst;
        }
        public class TeamDetails
        {
            public int Type;
            public string Name;
            public int Goals;
            public List<Goal> GoalsDetails;
            public int Offsides;
            public int Corners;
            public int OnTarget;
            public int OffTarget;
            public int Possession;
            public int Assists;
            public int FreeKicks;
            public int Penalties;
            public int TotalShots;
            public int Crossses;
            public int ThrowIns;
            public int Saves;
            public int Blocks;
            public int Clearances;
            public int Handballs;
            public int Fouls;
            public int YellowCards;
            public int RedCards;
        }
        public class MatchDetails
        {
            public TeamDetails HomeTeam;
            public TeamDetails AwayTeam;
            public DateTime Date;
            public int MatchID;
        }
        internal enum TeamType
        {
            Home = 1,
            Away = 2
        }
        public enum EventType
        {
            Corner = 1,
            Offside = 2,
            RedCard = 3,
            YellowCard = 4,
            ShotsOnTarget = 5,
            ShotsOffTarget = 6,
            Possession = 7,
            FreeKick = 8,
            Penalties = 9,
            TotalShots = 10,
            Crosses = 11,
            ThrowIns = 12,
            Saves = 13,
            Blocks = 14,
            Clearances = 15,
            Handballs = 16,
            Fouls = 17
        }

        public enum AttributeType
        {
            Dangerous = 1,
            Anemic = 2,
            Tough = 3,
            Soft = 4,
            Dominant = 5,
            Passive = 6,
            Accurate_In_Front_Of_Goal = 7,
            Inaccurate_In_Front_Of_Goal = 8,
            Good_Goalkeeper = 9,
            Poor_Keeper = 10
        }
        public class Goal
        {
            public string Scorer;
            public string Assistant;
            public int Minute;
            public bool IsOwnGoal;
        }
        public class Player
        {
            public string Name;
            public string Team;
        }
    }
}
