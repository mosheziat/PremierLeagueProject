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
            //PremierLeagueMainProject.AddAllPlayersToDb();
            //var ml = new MachineLearningProject.StatisticLine();
            //ml.Init(4404);
            //ml.AggregateStats();
            //var s = ml.Print();
            //PremierLeagueMainProject.SolvePossessionIssue();
            PremierLeagueMainProject.MainUpdatorFromEspn(12, 9);
            PremierLeagueMainProject.UpdateBetsOdds("2018-2019", 9);
            MachineLearningProject.PrintTrainingFile();
            MachineLearningProject.PrintTestFile(7, 9);
            //PremierLeagueMainProject.MainUpdatorFromEspn(2014,5,20,300,5);
            //MatchReporter.PrintReportForNextDaysNewVersion(daysToGet:7, competitionId: 8);
            Console.WriteLine("Hello world");
            Console.Read();


            /*
             * missing: home team = 40 away team 26
             * palace vs leicster
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


            away matches of a team
            SELECT        competitionmatch.HomeGoals, competitionmatch.AwayGoals, competitionmatch.WinnerTeamID
            FROM            competitionmatch INNER JOIN
                         team ON team.TeamID = competitionmatch.AwayTeamID
            WHERE        (competitionmatch.CompetitionID = 8) AND (team.TeamName = 'Tottenham Hotspur')
            ORDER BY competitionmatch.MatchDate DESC

            first half goals of a team
            SELECT        mg.MatchGoalID, mg.MatchID, mg.TeamID, mg.ScorerID, mg.AssistantID, mg.ScoringMinute
            FROM            matchgoal mg INNER JOIN
                         competitionmatch cm ON mg.MatchID = cm.CompetitionMatchID INNER JOIN
                         team t ON t.TeamID = mg.TeamID
            WHERE        (cm.CompetitionID = 8) AND (t.TeamName = 'Tottenham Hotspur') AND (mg.ScoringMinute <= 45)


            goals by minute of a team in away matches
            SELECT        mg.MatchGoalID, mg.MatchID, mg.TeamID, mg.ScorerID, mg.AssistantID, mg.ScoringMinute
FROM            matchgoal mg INNER JOIN
                         competitionmatch cm ON mg.MatchID = cm.CompetitionMatchID INNER JOIN
                         team t ON t.TeamID = mg.TeamID INNER JOIN
                         team t1 ON t1.TeamID = cm.HomeTeamID INNER JOIN
                         team t2 ON t2.TeamID = cm.AwayTeamID
WHERE        (cm.CompetitionID = 8) AND (t.TeamName = 'Crystal Palace') AND (t2.TeamName = 'Crystal Palace')
ORDER BY mg.ScoringMinute

            scorer

            SELECT        mg.MatchGoalID, mg.MatchID, mg.TeamID, mg.ScorerID, mg.AssistantID, mg.ScoringMinute
FROM            matchgoal mg INNER JOIN
                         competitionmatch cm ON mg.MatchID = cm.CompetitionMatchID INNER JOIN
                         team t ON t.TeamID = mg.TeamID
WHERE        (cm.CompetitionID = 8) AND (t.TeamName = 'Tottenham Hotspur') AND (cm.AwayTeamID = 28) AND (mg.ScorerID = 126)








            SELECT        cm.MatchDate, T1.TeamName, t2.TeamName AS Expr1, t3.TeamName AS Expr2, et.EventName, me1.eventvalue
FROM            matchevent me1 INNER JOIN
                         competitionmatch cm ON cm.CompetitionMatchID = me1.MatchID INNER JOIN
                         team T1 ON T1.TeamID = cm.HomeTeamID INNER JOIN
                         team t2 ON t2.TeamID = cm.AwayTeamID INNER JOIN
                         team t3 ON t3.TeamID = me1.TeamID INNER JOIN
                         eventtype et ON et.EventTypeID = me1.EventTypeID
WHERE        (me1.MatchID IN
                             (SELECT        me.MatchID
                               FROM            matchevent me INNER JOIN
                                                         competitionmatch cm ON cm.CompetitionMatchID = me.MatchID
                               WHERE        (me.EventTypeID = 7) AND (cm.CompetitionID IN (2, 3, 8)) AND (me.eventvalue = 0))) AND (me1.eventvalue = 0) AND (me1.EventTypeID IN (5, 7, 10, 17))
ORDER BY cm.MatchDate

             */
        }
    }
}
