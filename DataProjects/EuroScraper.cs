using CsQuery;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace DataProjects
{
    public class EuroScraper
    {
        public static DataObjects.MatchDetails GetMatchStatistics(string url)
        {
            var awayDetails = new DataObjects.TeamDetails();
            var homeDetails = new DataObjects.TeamDetails();
            var matchDetails = new DataObjects.MatchDetails();
            matchDetails.Date = DateTime.Today;

            var basicQuery = "[data-bind=text: HorA.Event]";
            var attempsInitial = ".total-attempts--values-wrap ";

            var dom = CQ.CreateFromUrl(url.Replace("index.html", "statistics/index.html"));
            //ballPossession
            var homeTeamName = dom[".team-home  .team-name"].Last().Text().Trim().Trim('\n').Trim('\r').Trim('\t').Trim().Trim();
            var homeGoals = dom[basicQuery.Replace("HorA", "home").Replace("Event", "goalsScored")].Text();
            var homeoffsides = dom[basicQuery.Replace("HorA", "home").Replace("Event", "offside")].Text();
            var homePossession = dom[basicQuery.Replace("HorA", "home").Replace("Event", "ballPossession + '%'")].Text().Trim('%');
            var homeGoalsDetails = GetGoalsForTeamInMatch(dom, "home");
            Console.Write(homeTeamName + " ");

            var homeCorners = dom[basicQuery.Replace("HorA", "home").Replace("Event", "corner")].Text();
            var homeOnTarget = dom[attempsInitial + basicQuery.Replace("HorA", "home").Replace("Event", "attempsOn")].Text();
            var homeOffTarget = dom[attempsInitial + basicQuery.Replace("HorA", "home").Replace("Event", "attempsOff")].Text();

            homeDetails.Type = (int) DataObjects.TeamType.Home;
            homeDetails.Name = homeTeamName;
            homeDetails.Goals = int.Parse(homeGoals);
            homeDetails.Offsides = int.Parse(homeoffsides);
            homeDetails.Corners = int.Parse(homeCorners);
            homeDetails.OnTarget = int.Parse(homeOnTarget);
            homeDetails.OffTarget = int.Parse(homeOffTarget);
            homeDetails.Possession = int.Parse(homePossession);
            homeDetails.GoalsDetails = homeGoalsDetails;

            var awayTeamName =
                dom[".team-away  .team-name"].Last().Text().Trim().Trim('\n').Trim('\r').Trim('\t').Trim().Trim();
            var awayGoals = dom[basicQuery.Replace("HorA", "away").Replace("Event", "goalsScored")].Text();
            var awayoffsides = dom[basicQuery.Replace("HorA", "away").Replace("Event", "offside")].Text();
            var awayCorners = dom[basicQuery.Replace("HorA", "away").Replace("Event", "corner")].Text();
            var awayOnTarget = dom[attempsInitial + basicQuery.Replace("HorA", "away").Replace("Event", "attempsOn")].Text();
            var awayOffTarget = dom[attempsInitial + basicQuery.Replace("HorA", "away").Replace("Event", "attempsOff")].Text();
            var awayPossession = dom[basicQuery.Replace("HorA", "away").Replace("Event", "ballPossession + '%'")].Text().Trim('%');
            var awayGoalsDetails = GetGoalsForTeamInMatch(dom, "away");
            Console.Write(awayTeamName);
            Console.WriteLine();

            awayDetails.Type = (int)DataObjects.TeamType.Away;
            awayDetails.Name = awayTeamName;
            awayDetails.Goals = int.Parse(awayGoals);
            awayDetails.Offsides = int.Parse(awayoffsides);
            awayDetails.Corners = int.Parse(awayCorners);
            awayDetails.OnTarget = int.Parse(awayOnTarget);
            awayDetails.OffTarget = int.Parse(awayOffTarget);
            awayDetails.OffTarget = int.Parse(awayPossession);
            awayDetails.GoalsDetails = awayGoalsDetails;

            matchDetails.HomeTeam = homeDetails;
            matchDetails.AwayTeam = awayDetails;

            return matchDetails;
        }
        public static List<DataObjects.Goal> GetGoalsForTeamInMatch(CQ dom, string hOrA)
        {
            var toReturn = new List<DataObjects.Goal>();
            var query = $".js-{hOrA}-team--scorers .scorer";
            var goals = dom[query].Select(x => x.Cq().Text().Trim()).ToList();
            foreach (var goal in goals)
            {
                var g = NormalizeGoalDetails(goal);
                toReturn.AddRange(g);
            }

            return toReturn;
        }
        public static void ReadFile(string path)
        {
            var allLines = File.ReadAllLines(path).Skip(2).Select(tab => tab.Split('\t'))
                .Select(x => new DataObjects.FileTeamDetails
                {
                    Name = x[0], Games = int.Parse(x[1]), Points = int.Parse(x[2]),
                    TotalGoalsScored = int.Parse(x[3]), AverageGoalsScored = int.Parse(x[4]),
                    FirstHalfGoalsScored = int.Parse(x[5]), SecondHalfGoalsScored = int.Parse(x[5]),
                    TotalGoalsConceded = int.Parse(x[6]), AverageGoalsConceded = int.Parse(x[7]),
                    FirstHalfGoalsConceded = int.Parse(x[8]), SecondHalfGoalsConceded = int.Parse(x[9]),
                    OffsidesOf = int.Parse(x[10])
                });

        }
        public static List<DataObjects.Goal> NormalizeGoalDetails(string details)
        {
            var toReturn = new List<DataObjects.Goal>();
            var detailsList = details.Replace("ET","").Trim().Split().ToList().Where(x => !string.IsNullOrEmpty(x)).ToList();
            var isOwnGoal = detailsList.Last() == "og";

            int minute = 0;
            var minutes = new List<int>();
            var minutePlaces = 0;
            var addedFirstALready = false;
            for (var i = 0; i < detailsList.Count; i++)
            {
                var val = detailsList[i].Trim(',');
                if (val.Contains("+"))
                {
                    val = val.Split('+').First();
                }

                if (int.TryParse(val, out minute))
                {
                    minutes.Add(minute);
                    if (!addedFirstALready)
                    {
                        minutePlaces = i;
                        addedFirstALready = true;
                    }
                        
                }
            }

            var scorer = string.Join(" ", detailsList.GetRange(0, minutePlaces));
            foreach (var min in minutes)
            {
                var newGoal = new DataObjects.Goal();
                newGoal.Scorer = scorer;
                newGoal.Minute = min;
                newGoal.IsOwnGoal = isOwnGoal;
                toReturn.Add(newGoal);
            }

            return toReturn;
        }
        public static List<string> GetPlayersOfTeam(CQ dom)
        {
            var list = dom[".squad--stats tr a"].Select(x => x.GetAttribute("title").Trim().Replace("Ä\u0087", "c")).ToList();
            return list;
        }
        public static string GetTeamName(CQ dom)
        {
            return dom[".team-name"].Text();
        }
        public static List<string> GetAllTeamsLinks(string path)
        {
            var dom = CQ.CreateFromUrl(path);
            var links = dom[".teams--qualified a"].Select(x => "http://www.uefa.com" + x.GetAttribute("href").Replace("/index.html", "/squad/index.html")).ToList();

            return links;
        }
        public static List<DataObjects.Player> ExtractAllPlayers(string path)
        {
            var teamsLinks = GetAllTeamsLinks(path);
            var toReturn = new List<DataObjects.Player>();
            foreach (var teamLink in teamsLinks)
            {
                var dom = CQ.CreateFromUrl(teamLink);
                var teamName = GetTeamName(dom);
                var players = GetPlayersOfTeam(dom);

                foreach (var player in players)
                {
                    var pl = new DataObjects.Player();
                    pl.Name = player;
                    pl.Team = teamName;

                    toReturn.Add(pl);
                }
               
            }

            return toReturn;
        }
        public static void AddPlayersToDB()
        {
            var mainPath = "http://www.uefa.com/uefaeuro/season=2016/teams/index.html";
            var allPlayers = ExtractAllPlayers(mainPath);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            using (var db = new sakilaEntities4())
            {                
                foreach (var player in allPlayers)
                {
                    var nationalTeamID = db.team.First(x => x.TeamName == player.Team).TeamID;
                    var alreadyExist =
                        db.player.Where(x => x.PlayerName == player.Name && x.NationalTeamID == nationalTeamID).ToList();

                    if (alreadyExist.Any())
                    {
                        continue;
                    }

                    Console.WriteLine(player.Team + ": " + player.Name);

                    var p = new player();
                    p.NationalTeamID = nationalTeamID;
                    p.PlayerName = player.Name;
                    p.PositionID = 1; //NA

                    db.player.Add(p);
                    db.SaveChanges();
                }
            }
        }
        public static int GetPlayerId(string name, int nationalTeamID, sakilaEntities4 db)
        {
            var nameArr = name.Replace("Ä\u0087", "c").Split().ToList();
            var lastName = nameArr.Last();
            var p =
                db.player.FirstOrDefault(
                    x => x.NationalTeamID == nationalTeamID && x.PlayerName.EndsWith(lastName));

            if (p != null)
            {
                return p.PlayerID;
            }

            return 0;
        }
        public static List<string> ExtractMatchesLinks()
        {
            var toReturn = new List<string>();
            for (int i = 0; i < 6; i++)
            {
                var ids = new List<int>
                {
                    2002441,
                    2002442,
                    2002443,
                    2002444,
                    2004543,
                    2004544
                };
                var id = ids[i];

                var groupStagePath = $"http://www.uefa.com/uefaeuro/season=2016/standings/round=2000448/group={id}/index.html";
                var dom = CQ.CreateFromUrl(groupStagePath);
                var links = dom[".report a"].Select(x => "http://www.uefa.com" + x.GetAttribute("href")).Distinct().ToList();

                toReturn.AddRange(links);
            }

            var startAt = 2017996;
            for (var j = 0; j <= 7; j++)
            {
                var pagePath =
                    $"http://www.uefa.com/uefaeuro/season=2016/matches/round=2000744/match={startAt + j}/index.html";

                toReturn.Add(pagePath);
            }

            startAt = 2017901;
            for (int i = 0; i <= 3; i++)
            {
                var quarterPagePath = $"http://www.uefa.com/uefaeuro/season=2016/matches/round=2000449/match={startAt + i}/index.html";
                toReturn.Add(quarterPagePath);

            }

            startAt = 2017905;
            for (int i = 0; i < 2; i++)
            {
                var semiFinalPath =
                    $"http://www.uefa.com/uefaeuro/season=2016/matches/round=2000450/match={startAt + i}/index.html";
                toReturn.Add(semiFinalPath);

            }

            toReturn.Add("http://www.uefa.com/uefaeuro/season=2016/matches/round=2000451/match=2017907/index.html");

            return toReturn;
        }
        public static List<DataObjects.MatchDetails> GetAllMatchesDetails()
        {
            var allDetails = new List<DataObjects.MatchDetails>();
            var allMatchesLinks = ExtractMatchesLinks();
            foreach (var link in allMatchesLinks)
            {
                var details = GetMatchStatistics(link);
                allDetails.Add(details);
            }

            return allDetails;
        }
        public static int? GetWinnerTeamID(int homeTeamID, int homeTeamGoals, int awayTeamID, int awayTeamGoals)
        {
            if (homeTeamGoals > awayTeamGoals)
            {
                return homeTeamID;
            }

            if (awayTeamGoals > homeTeamGoals)
            {
                return awayTeamID;
            }

            return null;
        }
        public static void AddMatchDetailsToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID, int awayTeamID)
        {
            var newMatch = new competitionmatch();
            newMatch.HomeTeamID = homeTeamID;
            newMatch.AwayTeamID = awayTeamID;
            newMatch.HomeGoals = match.HomeTeam.Goals;
            newMatch.AwayGoals = match.AwayTeam.Goals;
            newMatch.WinnerTeamID = GetWinnerTeamID(homeTeamID, match.HomeTeam.Goals, awayTeamID,
                match.AwayTeam.Goals);
            newMatch.MatchDate = match.Date;
            newMatch.CompetitionID = 1;

            db.competitionmatch.Add(newMatch);
            db.SaveChanges();
            match.MatchID = newMatch.CompetitionMatchID;

        }
        public static void AddCornersDetailsToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID, int awayTeamID)
        {
            //1
            var homeEv = new matchevent();
            homeEv.EventTypeID = 1;
            homeEv.TeamID = homeTeamID;
            homeEv.eventvalue = match.HomeTeam.Corners;
            homeEv.MatchID = match.MatchID;

            var awayEv = new matchevent();
            awayEv.EventTypeID = 1;
            awayEv.TeamID = awayTeamID;
            awayEv.eventvalue = match.AwayTeam.Corners;
            awayEv.MatchID = match.MatchID;

            db.matchevent.Add(homeEv);
            db.matchevent.Add(awayEv);
            db.SaveChanges();
        }
        public static void AddPossessionDetailsToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID, int awayTeamID)
        {
            //7
            var homeEv = new matchevent();
            homeEv.EventTypeID = 7;
            homeEv.TeamID = homeTeamID;
            homeEv.eventvalue = match.HomeTeam.Possession;
            homeEv.MatchID = match.MatchID;

            var awayEv = new matchevent();
            awayEv.EventTypeID = 7;
            awayEv.TeamID = awayTeamID;
            awayEv.eventvalue = match.AwayTeam.Possession;
            awayEv.MatchID = match.MatchID;

            db.matchevent.Add(homeEv);
            db.matchevent.Add(awayEv);
            db.SaveChanges();
        }
        public static void AddOffsidesDetailsToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID, int awayTeamID)
        {
            //2
            var homeEv = new matchevent();
            homeEv.EventTypeID = 2;
            homeEv.TeamID = homeTeamID;
            homeEv.eventvalue = match.HomeTeam.Offsides;
            homeEv.MatchID = match.MatchID;

            var awayEv = new matchevent();
            awayEv.EventTypeID = 2;
            awayEv.TeamID = awayTeamID;
            awayEv.eventvalue = match.AwayTeam.Offsides;
            awayEv.MatchID = match.MatchID;

            db.matchevent.Add(homeEv);
            db.matchevent.Add(awayEv);
            db.SaveChanges();
        }
        public static void AddShotsDetailsToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID, int awayTeamID)
        {
            var homeEv = new matchevent();
            homeEv.EventTypeID = 5;
            homeEv.TeamID = homeTeamID;
            homeEv.eventvalue = match.HomeTeam.OnTarget;
            homeEv.MatchID = match.MatchID;

            var awayEv = new matchevent();
            awayEv.EventTypeID = 5;
            awayEv.TeamID = awayTeamID;
            awayEv.eventvalue = match.AwayTeam.OnTarget;
            awayEv.MatchID = match.MatchID;

            var homeEv2 = new matchevent();
            homeEv2.EventTypeID = 6;
            homeEv2.TeamID = homeTeamID;
            homeEv2.eventvalue = match.HomeTeam.OffTarget;
            homeEv2.MatchID = match.MatchID;

            var awayEv2 = new matchevent();
            awayEv2.EventTypeID = 6;
            awayEv2.TeamID = awayTeamID;
            awayEv2.eventvalue = match.AwayTeam.OffTarget;
            awayEv2.MatchID = match.MatchID;

            db.matchevent.Add(homeEv);
            db.matchevent.Add(homeEv2);
            db.matchevent.Add(homeEv);
            db.matchevent.Add(awayEv2);
            db.SaveChanges();
        }
        public static void AddGoalsDetailsToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID, int awayTeamID)
        {
            var ownGoalID = 553;
            foreach (var goal in match.HomeTeam.GoalsDetails)
            {
                var newGoal = new matchgoal();
                newGoal.MatchID = match.MatchID;
                newGoal.ScoringMinute = goal.Minute;
                newGoal.TeamID = homeTeamID;
                if (goal.IsOwnGoal)
                {
                    newGoal.ScorerID = ownGoalID;
                }
                else
                {
                    newGoal.ScorerID = GetPlayerId(goal.Scorer, homeTeamID, db);
                }

                db.matchgoal.Add(newGoal);
                db.SaveChanges();
            }

            foreach (var goal in match.AwayTeam.GoalsDetails)
            {
                var newGoal = new matchgoal();
                newGoal.MatchID = match.MatchID;
                newGoal.ScoringMinute = goal.Minute;
                newGoal.TeamID = awayTeamID;
                if (goal.IsOwnGoal)
                {
                    newGoal.ScorerID = ownGoalID;
                }
                else
                {
                    newGoal.ScorerID = GetPlayerId(goal.Scorer, awayTeamID, db);
                }

                db.matchgoal.Add(newGoal);
                db.SaveChanges();
            }
        }
        public static void AddAllMatchDeatilsToDb(DataObjects.MatchDetails match, sakilaEntities4 db)
        {
            var homeTeamID = db.team.First(x => x.TeamName == match.HomeTeam.Name).TeamID;
            var awayTeamID = db.team.First(x => x.TeamName == match.AwayTeam.Name).TeamID;
            var matchAlreadyExists =
                db.competitionmatch.FirstOrDefault(
                    x => x.HomeTeamID == homeTeamID && x.AwayTeamID == awayTeamID);
            if (matchAlreadyExists != null)
            {
                return;
            }

            AddMatchDetailsToDb(match, db, homeTeamID, awayTeamID);
            AddGoalsDetailsToDb(match, db, homeTeamID, awayTeamID);
            AddShotsDetailsToDb(match, db, homeTeamID, awayTeamID);
            AddCornersDetailsToDb(match, db, homeTeamID, awayTeamID);
            AddOffsidesDetailsToDb(match, db, homeTeamID, awayTeamID);
            AddPossessionDetailsToDb(match, db, homeTeamID, awayTeamID);
        }
        public static void MainUpdator()
        {
            var i = 1;
            var allMatchDetails = GetAllMatchesDetails();
            using (var db = new sakilaEntities4())
            {
                foreach (var matchDetails in allMatchDetails)
                {
                    AddAllMatchDeatilsToDb(matchDetails, db);
                    Console.WriteLine(i++);
                }
            }
        }
    }


      
}
