using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

namespace DataProjects
{
    class Program
    {
        static void Main(string[] args)


        {

            //Helper.PrintUpdatedFullExpectedPrediction();
            //GoalsCalculator.PrintGoalScroingTable(3);
            //GoalsCalculator.PrintGoalConcededTable(3);
            //MainCalculator.RichestTeamsWins();
            //SecondaryStatsCalculator.PrintTableOfEvent(3, DataObjects.EventType.Fouls);
            //SecondaryStatsCalculator.PrintTableOfEvent(3, DataObjects.EventType.ShotsOnTarget);
            //SecondaryStatsCalculator.PrintTableOfEvent(3, DataObjects.EventType.Possession);
            //SecondaryStatsCalculator.PrintTableOfAccuracyInFrontOfGoalAgainst(3);
            //SecondaryStatsCalculator.PrintTableOfAccuracyInFrontOfGoal(3);
            //MainCalculator.PrineSimiliarTeamFile();
            //PremierLeagueMainProject.MainHandler();
            MatchReporter.PrintReportForNextDays(15);
            Console.WriteLine("Hello world");
            Console.Read();

            /*
             * Average goals per game:
             * SELECT        (SUM(HomeGoals) + SUM(AwayGoals)) / COUNT(*) AS Expr1
               FROM            competitionmatch cm
               WHERE        (CompetitionID = 3)
             */

            /*
             Corners:
             select teamName, sum(eventvalue) as s, count(*) as c
             from matchevent me
             JOIN Team t on t.teamID = me.teamID
             where EventTypeID = 1
             group by teamName
             order by 2 desc 


            Top Scorers:

            select PlayerName, count(*)
            from matchgoal mg
            join player p on p.playerID = mg.scorerID
            JOIN competitionmatch cm on cm.competitionMatchID = mg.matchID
            where cm.competitionID = 1 and playerID != 553
            group by PlayerName
            order by 2 desc

            Most common score:

            select HomeGoals, AwayGoals, count(*)
            from competitionmatch
            group by HomeGoals, AwayGoals
            order by 3 desc

            Scored against team:
            select PlayerName, count(*)
            from competitionmatch cm
            JOIN matchgoal mg on cm.competitionmatchID = mg.MatchID
            JOIN player p on p.playerID = mg.ScorerID
            JOIN team t on t.teamID = cm.hometeamID or t.teamID = cm.awayTeamID
            Join team te on te.teamID = mg.teamID
            where t.teamName = 'Italy' and te.teamName != 'Italy'
            group by playerName

            Player team, national team and position
            select p.PlayerName, pp.playerpositionName, t.TeamName, nt.TeamName
            from player p 
            JOIN team t on t.teamID = p.teamID
            JOIN team nt on nt.teamID = p.nationalteamID
            JOIN PlayerPosition pp on pp.playerPositionID = p.positionID

            Premier League Top Scorer:

            select PlayerName, count(*)
            from matchgoal mg
            Join competitionmatch cm on cm.competition
            JOIN competition c on c.competitionID = cm.competitionID
            join player p on p.playerID = mg.scorerID
            where c.competitionID = 2
            group by PlayerName
            order by 2 desc
             
            Premier League order by wins:
            
            select teamName, count(*)
            from competitionmatch cm
            JOIN competition c on c.competitionID = cm.competitionID
            JOIN team t on t.teamID = cm.WinnerteamID
            where c.competitionID = 2
            Group by teamName
            order by 2 desc


            Leader in premier league in event:

            select TeamName, sum(eventvalue), count(*)
            from matchevent ev
            JOIN team t on t.teamID = ev.teamID
            Join eventtype et on et.eventTypeID = ev.eventTypeID
            JOIN competitionmatch cm on cm.competitionMatchId = ev.matchID
            JOIN competition c on c.competitionId = cm.competitionID
            where eventName = 'Corner' and c.competitionID = 2
            group by TeamName
            order by 2 desc

            Total of event:

            select  sum(eventvalue), count(*)
            from matchevent ev
            JOIN team t on t.teamID = ev.teamID
            Join eventtype et on et.eventTypeID = ev.eventTypeID
            JOIN competitionmatch cm on cm.competitionMatchId = ev.matchID
            JOIN competition c on c.competitionId = cm.competitionID
            where eventName = 'Corner' and c.competitionID = 2

            Sum of event for a winner:

            select  sum(eventvalue), count(*)
            from matchevent ev
            Join eventtype et on et.eventTypeID = ev.eventTypeID
            JOIN competitionmatch cm on cm.competitionMatchId = ev.matchID
            JOIN team t on t.teamID = cm.winnerteamID and t.teamID = ev.teamID
            JOIN competition c on c.competitionId = cm.competitionID
            where eventName = 'Corner' and c.competitionID = 2

            Scorer by Position:

            select PlayerPositionName, count(*)
            from matchgoal mg
            JOIN player p on p.playerID = mg.scorerID
            JOIN competitionmatch cm on cm.competitionMatchID = mg.matchID
            JOIN playerposition pp on pp.playerPositionID = p.PositionID
            where cm.competitionID = 2 and playerID != 553
            group by PlayerPositionName
            order by 2 desc

             */
        }
    }
}
