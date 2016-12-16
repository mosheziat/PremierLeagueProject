using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

namespace DataProjects
{
    public class PremierLeagueScraper
    {
        const string PremierLeagueSitePath = "http://www.premierleague.com";
        public static List<string> Teams2016 = new List<string>
        {
           "man-utd",
           "bournemouth",
           "west-ham",
           "norwich",
           "leicester",
           "crystal-palace",
           "aston-villa",
           "spurs",
           "everton",
           "man-city",
           "west-brom",
           "watford",
           "arsenal",
           "sunderland",
           "chelsea",
           "stoke",
           "southampton",
           "liverpool",
           "newcastle",
           "swansea"
        };

        public static void AddTeamsToDb(List<string> teamNames)
        {
            var leagueTeamID = 1;
            using (var db = new sakilaEntities4())
            {
                foreach (var teamName in teamNames)
                {
                    var t = new team();
                    t.TeamName = teamName;
                    t.TeamTypeID = leagueTeamID;
                    db.team.Add(t);
                }

                db.SaveChanges();
            }
        }
        public static List<string> ExtractTeamsNames()
        {
            var path = PremierLeagueSitePath + "/en-gb/matchday/league-table.html?season=2015-2016";
            var dom = CQ.CreateFromUrl(path);
            var clubs = dom[".col-club"].Select(x => x.Cq().Text().Trim()).ToList().Skip(1).Take(20).ToList();

            return clubs;
        }
        public static List<string> ExtractSquadsLinks()
        {
            var path = PremierLeagueSitePath + "/en-gb/matchday/league-table.html?season=2015-2016";
            var dom = CQ.CreateFromUrl(path);
            var links =
                dom[".team a"].Select(x => PremierLeagueSitePath + x.GetAttribute("href").Replace("overview", "squad"))
                    //.Where(x => x.Contains(".squads."))
                    .Distinct()
                    .Take(20)
                    .ToList();

            return links;
        }
        public static List<string> GetPlayersNamesForTeam(int teamID)
        {
            return null;
        }
        public static List<PremierLeagueMainProject.PlayerDetails> ExtractAllPlayers(CQ dom)
        {
            var toReturn = new List<PremierLeagueMainProject.PlayerDetails>();
            var names = dom[".player-squadno a"].Select(x => x.Cq().Text()).ToList();
            var positions = dom[".player-position"].Select(x => x.Cq().Text()).ToList();
            if (names.Count != positions.Count)
            {
                throw new Exception();
            }

            for (var i = 0; i < names.Count; i++)
            {
                var name = names[i];
                var position = positions[i];
                var p = new PremierLeagueMainProject.PlayerDetails();
                p.PlayerName = name;
                p.PlayerPosition = position;
                toReturn.Add(p);
            }

            return toReturn;
        }
        public static bool IsPlayerAlreadyExists(string playerName, int teamID, sakilaEntities4 db, out player p)
        {
            var potential =
                db.player.FirstOrDefault(x => x.PlayerName == playerName && (x.TeamID == teamID || x.TeamID == null));
            if (potential != null)
            {
                p = potential;
                return true;
            }

            p = null;
            return false;
        }
        public static int AddPlayerToDb(PremierLeagueMainProject.PlayerDetails p, sakilaEntities4 db, int teamId)
        {
            var positionId = GetPositionId(p.PlayerPosition);
            var pl = new player();
            if (IsPlayerAlreadyExists(p.PlayerName, teamId, db, out pl))
            {
                pl.TeamID = teamId;
                pl.PositionID = positionId;
                db.SaveChanges();
                return pl.PlayerID;
            }

            var newPlayer = new player();

            newPlayer.PlayerName = p.PlayerName;
            newPlayer.PositionID = positionId;
            newPlayer.TeamID = teamId;
            db.player.Add(newPlayer);
            db.SaveChanges();

            return newPlayer.PlayerID;
        }
        public static void AddAllPlayersToDb()
        {
            var squads = ExtractSquadsLinks();
            using (var db = new sakilaEntities4())
            {
                foreach (var squadLink in squads)
                {
                    var dom = CQ.CreateFromUrl(squadLink);
                    var teamName = dom[".overlay h2"].Text().Replace("AFC ", "").Trim();
                    Console.WriteLine(teamName);
                    var teamId = db.team.First(x => x.TeamName == teamName).TeamID;
                    var allPlayers = ExtractAllPlayers(dom);
                    Console.WriteLine("Players: " + allPlayers.Count);
                    var i = 0;
                    foreach (var player in allPlayers)
                    {
                        AddPlayerToDb(player, db, teamId);
                        Console.WriteLine(i++);
                    }
                }
            }
        }
        public static int GetPositionId(string position)
        {
            switch (position)
            {
                case "Goalkeeper":
                    return 2;
                case "Defender":
                    return 3;
                case "Midfielder":
                    return 4;
                case "Forward":
                    return 5;
            }

            return 1;
        }
        public static List<string> ExtractMatchesLinks(int year)
        {
            var path =
                $"http://www.premierleague.com/content/premierleague/en-gb/matchday/results.html?paramSeasonId={year}";

            var dom = CQ.CreateFromUrl(path);

            var links = dom[".score a"]
                .Select(x => PremierLeagueSitePath + x.GetAttribute("href"))
                .Where(x => x.Contains("/matches/"))
                .Select(x => x.Replace("report.html", "stats.html"))
                .ToList();

            return links;
        }
        public static DataObjects.MatchDetails GetMatchStatistics(string url)
        {
            var awayDetails = new DataObjects.TeamDetails();
            var homeDetails = new DataObjects.TeamDetails();
            var matchDetails = new DataObjects.MatchDetails();
            
            var dom = CQ.CreateFromUrl(url);
            var homeTeamName = dom[".club.home"].Text();
            var awayTeamName = dom[".club.away"].Text();
            var date = dom[".fixtureinfo"].Text().Split('|').First().Trim();

            matchDetails.Date = DateTime.Parse(date);
            homeDetails.Name = homeTeamName;
            awayDetails.Name = awayTeamName;

            var allRelevantSections = dom[".statsTable .contentTable tr"].ToList();
            for (int i = 0; i < allRelevantSections.Count; i = i + 3)
            {
                var parameters = allRelevantSections[i].ChildElements.Select(x => x.Cq().Text()).Skip(1).ToList();
                var homeParameters = allRelevantSections[i + 1].ChildElements.Select(x => x.Cq().Text()).Skip(1).ToList();
                var awayParameters = allRelevantSections[i + 2].ChildElements.Select(x => x.Cq().Text()).Skip(1).ToList();
                for (int j = 0; j < parameters.Count; j++)
                {
                    var parameter = parameters[j];
                    var homeValue = int.Parse(homeParameters[j]);
                    var awayValue = int.Parse(awayParameters[j]);
                    FillTheRightField(parameter, homeValue, homeDetails);
                    FillTheRightField(parameter, awayValue, awayDetails);
                }

            }

            var homeGoals = dom[".homeScore"].Text();
            var awayGoals = dom[".awayScore"].Text();

            homeDetails.Goals = int.Parse(homeGoals);
            awayDetails.Goals = int.Parse(awayGoals);

            var homeGoalsDetails = dom[".home.goals li"].Select(x => x.Cq().Text());
            var awayGoalsDetails = dom[".away.goals li"].Select(x => x.Cq().Text());

            homeDetails.GoalsDetails = new List<DataObjects.Goal>();
            awayDetails.GoalsDetails = new List<DataObjects.Goal>();

            foreach (var homeGoalsDetail in homeGoalsDetails)
            {
                var goalsToAdd = GetGoalsFromString(homeGoalsDetail);
                homeDetails.GoalsDetails.AddRange(goalsToAdd); 
            }

            foreach (var awayGoalsDetail in awayGoalsDetails)
            {
                var goalsToAdd = GetGoalsFromString(awayGoalsDetail);
                awayDetails.GoalsDetails.AddRange(goalsToAdd);
            }

            matchDetails.AwayTeam = awayDetails;
            matchDetails.HomeTeam = homeDetails;

            return matchDetails;
        }
        public static List<DataObjects.Goal> GetGoalsFromString(string s)
        {
            var toReturn = new List<DataObjects.Goal>();
            //Olivier Giroud (5, 78, 80)
            //Mark Bunn (90 + 2 OG)
            //Wayne Rooney (43)

            var name = s.Split('(').First().Trim();
            var contentWithinParentheses = Helper.GetContentWithinParenthesis(s);
            var isOwnGoals = contentWithinParentheses.EndsWith("OG");
            var minutes = contentWithinParentheses.Replace("OG", "").Split(',').ToList();

            foreach (var minute in minutes)
            {
                var g = new DataObjects.Goal();
                g.IsOwnGoal = isOwnGoals;
                g.Scorer = name;

                var min = minute.Replace("Pen", "");
                if (minute.Contains("+"))
                {
                    min = minute.Split('+').First().Trim();
                }


                g.Minute = int.Parse(min);

                toReturn.Add(g);
            }

            return toReturn;

        }
        public static DataObjects.TeamDetails FillTheRightField(string fieldName, int fieldValue,
            DataObjects.TeamDetails details)
        {
            switch (fieldName.ToUpper())
            {
                case "ASSISTS":
                    details.Assists = fieldValue;
                    break;
                case "FREE KICKS":
                    details.FreeKicks = fieldValue;
                    break;
                case "PENALTIES":
                    details.Penalties = fieldValue;
                    break;
                case "TOTAL SHOTS":
                    details.TotalShots = fieldValue;
                    break;
                case "SHOTS ON TARGET":
                    details.OnTarget = fieldValue;
                    break;
                case "SHOTS OFF TARGET":
                    details.OffTarget = fieldValue;
                    break;
                case "CROSSES":
                    details.Crossses = fieldValue;
                    break;
                case "CORNERS":
                    details.Corners = fieldValue;
                    break;
                case "THROW INS":
                    details.ThrowIns = fieldValue;
                    break;
                case "SAVES":
                    details.Saves = fieldValue;
                    break;
                case "BLOCKS":
                    details.Blocks = fieldValue;
                    break;
                case "CLEARANCES":
                    details.Clearances = fieldValue;
                    break;
                case "OFFSIDES":
                    details.Offsides = fieldValue;
                    break;
                case "HANDBALLS":
                    details.Handballs = fieldValue;
                    break;
                case "FOULS":
                    details.Fouls = fieldValue;
                    break;
                case "YELLOW CARDS":
                    details.YellowCards = fieldValue;
                    break;
                case "RED CARDS":
                    details.RedCards = fieldValue;
                    break;
            }

            return details;
        }
        public static void AddMatchDetailsToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID, int awayTeamID)
        {
            var newMatch = new competitionmatch();
            newMatch.HomeTeamID = homeTeamID;
            newMatch.AwayTeamID = awayTeamID;
            newMatch.HomeGoals = match.HomeTeam.Goals;
            newMatch.AwayGoals = match.AwayTeam.Goals;
            newMatch.WinnerTeamID = Helper.GetWinnerTeamID(homeTeamID, match.HomeTeam.Goals, awayTeamID,
                match.AwayTeam.Goals);
            newMatch.MatchDate = match.Date;
            newMatch.CompetitionID = 2;

            db.competitionmatch.Add(newMatch);
            db.SaveChanges();
            match.MatchID = newMatch.CompetitionMatchID;
        }

        public static void AddFullMatchDetailsToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID,
            int awayTeamID)
        {
            AddMatchDetailsToDb(match, db, homeTeamID, awayTeamID);

            AddEventToDb((int) DataObjects.EventType.Blocks, homeTeamID, match.MatchID, match.HomeTeam.Blocks, db);
            AddEventToDb((int)DataObjects.EventType.Clearances, homeTeamID, match.MatchID, match.HomeTeam.Clearances, db);
            AddEventToDb((int)DataObjects.EventType.Corner, homeTeamID, match.MatchID, match.HomeTeam.Corners, db);
            AddEventToDb((int)DataObjects.EventType.Crosses, homeTeamID, match.MatchID, match.HomeTeam.Crossses, db);
            AddEventToDb((int)DataObjects.EventType.Fouls, homeTeamID, match.MatchID, match.HomeTeam.Fouls, db);
            AddEventToDb((int)DataObjects.EventType.FreeKick, homeTeamID, match.MatchID, match.HomeTeam.FreeKicks, db);
            AddEventToDb((int)DataObjects.EventType.Handballs, homeTeamID, match.MatchID, match.HomeTeam.Handballs, db);
            AddEventToDb((int)DataObjects.EventType.Offside, homeTeamID, match.MatchID, match.HomeTeam.Offsides, db);
            AddEventToDb((int)DataObjects.EventType.Penalties, homeTeamID, match.MatchID, match.HomeTeam.Penalties, db);
            AddEventToDb((int)DataObjects.EventType.Possession, homeTeamID, match.MatchID, match.HomeTeam.Possession, db);
            AddEventToDb((int)DataObjects.EventType.YellowCard, homeTeamID, match.MatchID, match.HomeTeam.YellowCards, db);
            AddEventToDb((int)DataObjects.EventType.RedCard, homeTeamID, match.MatchID, match.HomeTeam.RedCards, db);
            AddEventToDb((int)DataObjects.EventType.Saves, homeTeamID, match.MatchID, match.HomeTeam.Saves, db);
            AddEventToDb((int)DataObjects.EventType.ShotsOffTarget, homeTeamID, match.MatchID, match.HomeTeam.OffTarget, db);
            AddEventToDb((int)DataObjects.EventType.ShotsOnTarget, homeTeamID, match.MatchID, match.HomeTeam.OnTarget, db);
            AddEventToDb((int)DataObjects.EventType.Handballs, homeTeamID, match.MatchID, match.HomeTeam.Handballs, db);
            AddEventToDb((int)DataObjects.EventType.ThrowIns, homeTeamID, match.MatchID, match.HomeTeam.ThrowIns, db);
            AddEventToDb((int)DataObjects.EventType.TotalShots, homeTeamID, match.MatchID, match.HomeTeam.TotalShots, db);

            AddEventToDb((int)DataObjects.EventType.Blocks, awayTeamID, match.MatchID, match.AwayTeam.Blocks, db);
            AddEventToDb((int)DataObjects.EventType.Clearances, awayTeamID, match.MatchID, match.AwayTeam.Clearances, db);
            AddEventToDb((int)DataObjects.EventType.Corner, awayTeamID, match.MatchID, match.AwayTeam.Corners, db);
            AddEventToDb((int)DataObjects.EventType.Crosses, awayTeamID, match.MatchID, match.AwayTeam.Crossses, db);
            AddEventToDb((int)DataObjects.EventType.Fouls, awayTeamID, match.MatchID, match.AwayTeam.Fouls, db);
            AddEventToDb((int)DataObjects.EventType.FreeKick, awayTeamID, match.MatchID, match.AwayTeam.FreeKicks, db);
            AddEventToDb((int)DataObjects.EventType.Handballs, awayTeamID, match.MatchID, match.AwayTeam.Handballs, db);
            AddEventToDb((int)DataObjects.EventType.Offside, awayTeamID, match.MatchID, match.AwayTeam.Offsides, db);
            AddEventToDb((int)DataObjects.EventType.Penalties, awayTeamID, match.MatchID, match.AwayTeam.Penalties, db);
            AddEventToDb((int)DataObjects.EventType.Possession, awayTeamID, match.MatchID, match.AwayTeam.Possession, db);
            AddEventToDb((int)DataObjects.EventType.RedCard, awayTeamID, match.MatchID, match.AwayTeam.RedCards, db);
            AddEventToDb((int)DataObjects.EventType.YellowCard, awayTeamID, match.MatchID, match.AwayTeam.YellowCards, db);
            AddEventToDb((int)DataObjects.EventType.Saves, awayTeamID, match.MatchID, match.AwayTeam.Saves, db);
            AddEventToDb((int)DataObjects.EventType.ShotsOffTarget, awayTeamID, match.MatchID, match.AwayTeam.OffTarget, db);
            AddEventToDb((int)DataObjects.EventType.ShotsOnTarget, awayTeamID, match.MatchID, match.AwayTeam.OnTarget, db);
            AddEventToDb((int)DataObjects.EventType.Handballs, awayTeamID, match.MatchID, match.AwayTeam.Handballs, db);
            AddEventToDb((int)DataObjects.EventType.ThrowIns, awayTeamID, match.MatchID, match.AwayTeam.ThrowIns, db);
            AddEventToDb((int)DataObjects.EventType.TotalShots, awayTeamID, match.MatchID, match.AwayTeam.TotalShots, db);

            AddGoalsDetailsToDb(match, db, homeTeamID, awayTeamID);
        }
        public static void AddEventToDb(int eventTypeId, int teamId, int matchId, int value, sakilaEntities4 db)
        {
            var newEv = new matchevent();
            newEv.EventTypeID = eventTypeId;
            newEv.TeamID = teamId;
            newEv.eventvalue = value;
            newEv.MatchID = matchId;

            db.matchevent.Add(newEv);
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
        public static int GetPlayerId(string name, int teamId, sakilaEntities4 db)
        {
            var p =
                db.player.FirstOrDefault(
                    x => x.TeamID == teamId && x.PlayerName.EndsWith(name));

            if (p != null)
            {
                return p.PlayerID;
            }

            var newPlayer = new PremierLeagueMainProject.PlayerDetails();
            newPlayer.PlayerName = name;
            newPlayer.PlayerPosition = "NA"; //NA

            var playerId = AddPlayerToDb(newPlayer, db, teamId);

            return playerId;
        }
        public static List<string> BuildPlMatches2016()
        {
            var path = "http://www.premierleague.com/en-gb/matchday/matches/2015-2016/epl.match-stats.html/HomeTeam-vs-AwayTeam";
            var toReturn = new List<string>();

            foreach (var team in Teams2016)
            {
                for (int i = 0; i < Teams2016.Count; i++)
                {
                    var teamAgainst = Teams2016[i];
                    if (teamAgainst.Equals(team))
                    {
                        continue;
                    }

                    var match = path.Replace("HomeTeam", team).Replace("AwayTeam", teamAgainst);
                    toReturn.Add(match);
                }
            }

            return toReturn;
        }
        public static void MainHandler(int year)
        {
            var allMatches = BuildPlMatches2016();
            using (var db = new sakilaEntities4())
            {
                foreach (var match in allMatches)
                {
                    var matchDetails = GetMatchStatistics(match);

                    var normalizedHomeTeamName =
                        NormalizeTeamName(matchDetails.HomeTeam.Name);
                    var normalizedAwayTeamName =
                        NormalizeTeamName(matchDetails.AwayTeam.Name);

                    Console.WriteLine(normalizedHomeTeamName + " VS. " + normalizedAwayTeamName);


                    var homeTeamId = db.team.First(x => x.TeamName == normalizedHomeTeamName).TeamID;
                    var awayTeamId = db.team.First(x => x.TeamName == normalizedAwayTeamName).TeamID;

                    var matchAlreadyExists =
                             db.competitionmatch
                             .FirstOrDefault(x => x.HomeTeamID == homeTeamId &&
                                    x.AwayTeamID == awayTeamId &&
                                    x.MatchDate == matchDetails.Date);

                    if (matchAlreadyExists != null)
                    {
                        continue;
                    }

                    AddFullMatchDetailsToDb(matchDetails, db, homeTeamId, awayTeamId);
                }
            }
        }
        public static string NormalizeTeamName(string name)
        {
            var result = name.Replace("Man", "Manchester")
                .Replace(" Ham", " Ham United")
                .Replace("Utd", "United")
                .Replace("Norwich", "Norwich City")
                .Replace("Leicester", "Leicester City")
                .Replace("Spurs", "Tottenham Hotspur")
                .Replace("West Brom", "West Bromwich Albion")
                .Replace("Stoke", "Stoke City")
                .Replace("Newcastle", "Newcastle United")
                .Replace("Swansea", "Swansea City");
  
            return result;
        }
    }




    public class TeamStats
    {
        public static int Assists;
        public static int FreeKicks;
        public static int Penalties;
        public static int TotalShots;
        public static int ShotsOnTarget;
        public static int ShotsOffTarget;
        public static int Crossses;
        public static int Coreners;
        public static int ThrowIns;
        public static int Saves;
        public static int Blocks;
        public static int Clearances;
        public static int Offsides;
        public static int Handballs;
        public static int Fouls;
        public static int YellowCards;
        public static int RedCards;
    }
}
