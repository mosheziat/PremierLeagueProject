using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;
using CsQuery.ExtensionMethods.Internal;

namespace DataProjects
{
    public class PremierLeagueMainProject
    {
        const string PremierLeagueSitePath = "http://www.premierleague.com";
        const string BbcSitePath = "http://www.bbc.com";
        public static List<string> ExtractSquadLists()
        {
            var list = new List<string>();

            var page = "https://www.premierleague.com/clubs";
            var dom = CQ.CreateFromUrl(page);
            list = dom[".clubIndex .indexSection a"]
                .Select(x => x.GetAttribute("href"))
                .Select(x => PremierLeagueSitePath + x.Replace("overview", "squad"))
                .ToList();
            return list;
        }
        public static List<PlayerDetails> GetPlayerDetailsFromSquadPage(CQ dom)
        {
            var playerCards = dom[".playerCardInfo"].ToList();
            var allDetails = new List<PlayerDetails>();

            foreach (var playerCard in playerCards)
            {
                var allChilds = playerCard.ChildElements.ToList();
                var playerName = allChilds.First(x => x.ClassName == "name").InnerText;
                var position = allChilds.First(x => x.ClassName == "position").InnerText;
                var newPlayer = new PlayerDetails
                {
                    PlayerName = Helper.NormalizePlayerName(playerName),
                    PlayerPosition = position
                };

                allDetails.Add(newPlayer);
            }

            return allDetails;
        }
        public static void AddAllPlayersToDb()
        {
            var allSquadLinks = ExtractSquadLists();

            using (var db = new sakilaEntities4())
            {
                foreach (var squadLink in allSquadLinks)
                {
                    var dom = CQ.CreateFromUrl(squadLink);
                    var teamName = Helper.NormalizeTeamName(dom[".clubDetails .team"].Text());
                    Console.WriteLine(teamName);
                    var teamId = db.team.First(x => x.TeamName == teamName).TeamID;
                    var players = GetPlayerDetailsFromSquadPage(dom);

                    foreach (var player in players)
                    {
                        Helper.AddPlayerToDb(player, db, teamId);
                    }
                }
            }
        }
        public static void AddNewTeams()
        {
            var teams = new List<string>
            {
                "Brighton & Hove Albion",
                "Huddersfield Town",
            };

            using (var db = new sakilaEntities4())
            {
                var leagueTeamID = 1;

                foreach (var team in teams)
                {
                    var newTeam = new team
                    {
                        TeamName = team,
                        TeamTypeID = leagueTeamID
                    };
                    db.team.Add(newTeam);
                }

                db.SaveChanges();
            }

    }

        public static string NormalizeDate(DateTime date)
        {
            var year = date.Year.ToString();
            var month = date.Month.ToString();
            if (month.Length == 1)
            {
                month = "0" + month;
            }
            var day = date.Day.ToString();
            if (day.Length == 1)
            {
                day = "0" + day;
            }

            return year+month+day;
        }
        public static List<string> GetMatchesForDate(DateTime date)
        {
            string normalizedDate = NormalizeDate(date);
            string url = $"http://www.espn.com/soccer/fixtures/_/date/{normalizedDate}/league/eng.1";
            var dom = CQ.CreateFromUrl(url);
            var links = dom[".record a"]
                .Where(x => x.HasAttribute("href"))
                .Select(x => x.GetAttribute("href"))
                .Where(x => !x.Contains("matchstats") || x.Contains("gameId=513548"))
                .Select(x => "http://www.espn.com" + x.Replace("/report", "/matchstats"))
                .ToList();

            return links;
        }
        public static DataObjects.MatchDetails GetMatchStatisticsFromEspn(string url)
        {
            var awayDetails = new DataObjects.TeamDetails();
            var homeDetails = new DataObjects.TeamDetails();
            var matchDetails = new DataObjects.MatchDetails();

            var summaryPage = url.Replace("matchstats", "match");
            var summaryPageDom = CQ.CreateFromUrl(summaryPage);
            var title = summaryPageDom["title"].Text();
            var dateStr = title.Split('-')[2].Trim();
            matchDetails.Date = DateTime.Parse(dateStr);

            var dom = CQ.CreateFromUrl(url);

            homeDetails.Name = dom[".team.away .long-name"].Text();
            awayDetails.Name = dom[".team.home .long-name"].Text();

            homeDetails.Goals = int.Parse(dom[".score-container [data-home-away=home]"].Text());
            awayDetails.Goals = int.Parse(dom[".score-container [data-home-away=away]"].Text());

            var awayScorers = dom[".team.home [data-event-type=goal]"]
                .Select(x => x.Cq().Text().Trim())
                .SelectMany(x => x.Split(')').ToList())
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(Helper.RemoveDiacritics)
                .Select(Helper.NormalizePlayerName)
                .ToList();

            var awayGoals = GetGoalsForTeam(awayScorers);
            var homeScorers = dom[".team.away [data-event-type=goal]"]
               .Select(x => x.Cq().Text().Trim())
                .SelectMany(x => x.Split(')').ToList())
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(Helper.RemoveDiacritics)
                .Select(Helper.NormalizePlayerName)
                .ToList();


            var homeGoals = GetGoalsForTeam(homeScorers);
            homeDetails.GoalsDetails = homeGoals;
            awayDetails.GoalsDetails = awayGoals;

            homeDetails.Possession = int.Parse(dom[".possession [data-home-away=home]"].First().Text().Replace("%", "").Trim());
            awayDetails.Possession = int.Parse(dom[".possession [data-home-away=away]"].First().Text().Replace("%", "").Trim());

            var homeShots = dom[".shots [data-home-away=home]"].First().Text();
            homeDetails.TotalShots = int.Parse(homeShots.Split('(').First());
            homeDetails.OnTarget = int.Parse(homeShots.Split('(').Last().Replace(")", "").Trim());

            var awayShots = dom[".shots [data-home-away=away]"].First().Text();
            awayDetails.TotalShots = int.Parse(awayShots.Split('(').First());
            awayDetails.OnTarget = int.Parse(awayShots.Split('(').Last().Replace(")", "").Trim());

            homeDetails.Fouls = int.Parse(dom["[data-stat=foulsCommitted]"].First().Text());
            awayDetails.Fouls = int.Parse(dom["[data-stat=foulsCommitted]"].Last().Text());

            homeDetails.YellowCards = int.Parse(dom["[data-stat=yellowCards]"].First().Text());
            awayDetails.YellowCards = int.Parse(dom["[data-stat=yellowCards]"].Last().Text());
            
            homeDetails.RedCards = int.Parse(dom["[data-stat=redCards]"].First().Text());
            awayDetails.RedCards = int.Parse(dom["[data-stat=redCards]"].Last().Text());

            homeDetails.Offsides = int.Parse(dom["[data-stat=offsides]"].First().Text());
            awayDetails.Offsides = int.Parse(dom["[data-stat=offsides]"].Last().Text());
            
            homeDetails.Corners = int.Parse(dom["[data-stat=wonCorners]"].First().Text());
            awayDetails.Corners = int.Parse(dom["[data-stat=wonCorners]"].Last().Text());

            matchDetails.HomeTeam = homeDetails;
            matchDetails.AwayTeam = awayDetails;

            return matchDetails;
        }
        public static DataObjects.MatchDetails GetMatchStatistics(string url)
        {
            var awayDetails = new DataObjects.TeamDetails();
            var homeDetails = new DataObjects.TeamDetails();
            var matchDetails = new DataObjects.MatchDetails();

            var dom = CQ.CreateFromUrl(url);
            var date = dom[".fixture__date"].Text().Replace("Sept", "sep");
            matchDetails.Date = DateTime.ParseExact(date, "ddd dd MMM yyyy", new CultureInfo("us"));

            var matchHeaderQuery = ".match-overview-header";
            var teamNames = matchHeaderQuery + " " + ".fixture__team-name--HorA";
            homeDetails.Name = dom[teamNames.Replace("HorA", "home")].Text();
            awayDetails.Name = dom[teamNames.Replace("HorA", "away")].Text();

            var score = matchHeaderQuery + " " + ".fixture__number--HorA";
            homeDetails.Goals = int.Parse(dom[score.Replace("HorA", "home")].Text());
            awayDetails.Goals = int.Parse(dom[score.Replace("HorA", "away")].Text());

            var scorersQuery = matchHeaderQuery + " " + ".fixture__scorers-HorA";
            var homeScorers = dom[scorersQuery.Replace("HorA", "home")]
                .Select(x => x.Cq().Text())
                .Where(x => !string.IsNullOrEmpty(x))
                .SelectMany(x => x.Split('Â').ToList())
                .Where(x => !x.Contains("Dismissed"))
                .Select(Helper.RemoveDiacritics)
                .Select(Helper.NormalizePlayerName)
                .ToList();
            var homeGoals = GetGoalsForTeam(homeScorers);
            var awayScorers = dom[scorersQuery.Replace("HorA", "away")]
                .Select(x => x.Cq().Text())
                .Where(x => !string.IsNullOrEmpty(x))
                .SelectMany(x => x.Split('Â').ToList())
                .Where(x => !x.Contains("Dismissed"))
                .Select(Helper.RemoveDiacritics)
                .Select(Helper.NormalizePlayerName)
                .ToList();
            var awayGoals = GetGoalsForTeam(awayScorers);
            homeDetails.GoalsDetails = homeGoals;
            awayDetails.GoalsDetails = awayGoals;

            var percentageRowQuery = ".percentage-row";
            var relevantDetails = dom[percentageRowQuery].ToList();

            try
            {
                var possesion = relevantDetails.First(x => x.Cq().Text().Contains("Possession"));
                var possessionElements = possesion.ChildElements.ToList();
                homeDetails.Possession = int.Parse(possessionElements[1].Cq().Text().Replace("Home", "").Trim().Trim('%'));
                awayDetails.Possession = int.Parse(possessionElements.Last().Cq().Text().Replace("Away", "").Trim().Trim('%'));

                var totalShots = relevantDetails.First(x => x.Cq().Text().Contains("Shots"));
                var totalShotsElements = totalShots.ChildElements.ToList();
                homeDetails.TotalShots = int.Parse(totalShotsElements[1].Cq().Text().Replace("Home", ""));
                awayDetails.TotalShots = int.Parse(totalShotsElements.Last().Cq().Text().Replace("Away", ""));


                var shotsOnTarget = relevantDetails.First(x => x.Cq().Text().Contains("Shots on Target"));
                var shotsOnTargetElements = shotsOnTarget.ChildElements.ToList();
                homeDetails.OnTarget = int.Parse(shotsOnTargetElements[1].Cq().Text().Replace("Home", ""));
                awayDetails.OnTarget = int.Parse(shotsOnTargetElements.Last().Cq().Text().Replace("Away", ""));

                var corners = relevantDetails.First(x => x.Cq().Text().Contains("Corners"));
                var cornernsElements = corners.ChildElements.ToList();
                homeDetails.Corners = int.Parse(cornernsElements[1].Cq().Text().Replace("Home", ""));
                awayDetails.Corners = int.Parse(cornernsElements.Last().Cq().Text().Replace("Away", ""));

                var fouls = relevantDetails.First(x => x.Cq().Text().Contains("Fouls"));
                var foulsElements = fouls.ChildElements.ToList();
                homeDetails.Fouls = int.Parse(foulsElements[1].Cq().Text().Replace("Home", ""));
                awayDetails.Fouls = int.Parse(foulsElements.Last().Cq().Text().Replace("Away", ""));
                }
            catch (Exception)
            {
                Console.WriteLine("No Stats for this match!");
            }

            matchDetails.HomeTeam = homeDetails;
            matchDetails.AwayTeam = awayDetails;

            return matchDetails;
        }

        public static void addOnlyStatisticsToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID,
            int awayTeamID)
        {

            Helper.AddEventToDb((int)DataObjects.EventType.Possession, homeTeamID, match.MatchID, match.HomeTeam.Possession, db);
            Helper.AddEventToDb((int)DataObjects.EventType.Corner, homeTeamID, match.MatchID, match.HomeTeam.Corners, db);
            Helper.AddEventToDb((int)DataObjects.EventType.Fouls, homeTeamID, match.MatchID, match.HomeTeam.Fouls, db);
            Helper.AddEventToDb((int)DataObjects.EventType.TotalShots, homeTeamID, match.MatchID, match.HomeTeam.TotalShots, db);
            Helper.AddEventToDb((int)DataObjects.EventType.ShotsOnTarget, homeTeamID, match.MatchID, match.HomeTeam.OnTarget, db);

            Helper.AddEventToDb((int)DataObjects.EventType.Possession, awayTeamID, match.MatchID, match.AwayTeam.Possession, db);
            Helper.AddEventToDb((int)DataObjects.EventType.Corner, awayTeamID, match.MatchID, match.AwayTeam.Corners, db);
            Helper.AddEventToDb((int)DataObjects.EventType.Fouls, awayTeamID, match.MatchID, match.AwayTeam.Fouls, db);
            Helper.AddEventToDb((int)DataObjects.EventType.TotalShots, awayTeamID, match.MatchID, match.AwayTeam.TotalShots, db);
            Helper.AddEventToDb((int)DataObjects.EventType.ShotsOnTarget, awayTeamID, match.MatchID, match.AwayTeam.OnTarget, db);
        }

        public static void addOnlyPossessionToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID,
            int awayTeamID)
        {
            Helper.AddEventToDb((int)DataObjects.EventType.Possession, homeTeamID, match.MatchID, match.HomeTeam.Possession, db);
            Helper.AddEventToDb((int)DataObjects.EventType.Possession, awayTeamID, match.MatchID, match.AwayTeam.Possession, db);
        }

        public static void AddFullMatchDetailsToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID,
            int awayTeamID)
        {
            Helper.AddMatchDetailsToDb(match, db, homeTeamID, awayTeamID);
            Helper.AddGoalsDetailsToDb(match, db, homeTeamID, awayTeamID);
            addOnlyStatisticsToDb(match, db, homeTeamID, awayTeamID);
        }

        public static void FixShotsIssue()
        {
            var competitionId = 3;
            using (var db = new sakilaEntities4())
            {
                var events = db.matchevent
                    .Where(x => x.competitionmatch.CompetitionID == competitionId)
                    .Where(x => x.EventTypeID == 5)
                    .GroupBy(x => new  { x.MatchID, x.TeamID})
                    .ToList();

                foreach (var ev in events)
                {
                    if (ev.Count() == 2)
                    {
                        var larger = ev.OrderByDescending(x => x.eventvalue).First();
                        larger.EventTypeID = 10;
                        db.SaveChanges();
                    }
                }
            }
        }
        public static List<DataObjects.Goal> GetGoalsForTeam(List<string> scorersList)
        {
            var allGoals = new List<DataObjects.Goal>();
            foreach (var scorer in scorersList)
            {
                var goals = Helper.GetGoalsFromString(scorer);
                allGoals.AddRange(goals);
            }

            return allGoals;
        }
        public static List<string> ExtractAllMatches()
        {
            var matchesPage = "http://www.bbc.com/sport/football/premier-league/scores-fixtures";
            var dom = CQ.CreateFromUrl(matchesPage);
            var links = dom["a"].Where(x => x.HasAttribute("href"))
                .Select(x => x.GetAttribute("href"))
                .Where(Helper.IsRelevantMatchLink)
                .Select(x => BbcSitePath + x)
                .Reverse()
                .ToList();

            return links;
        }

        public static void UpdateResultsOnly(string  season, int competitionId)
        {
            var page = $"http://www.betexplorer.com/soccer/england/premier-league-{season}/results/";
            var dom = CQ.CreateFromUrl(page);
            var allResults = dom[".result-table tr"].Skip(1).ToList();
            using (var db = new sakilaEntities4())
            {
                foreach (var result in allResults)
                {
                    var elements = result.ChildElements.Select(x => x.Cq().Text()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    if (elements.Count != 3)
                    {
                        continue;
                    }
                    var teams = elements[0];
                    var matchResult = elements[1];
                    var date = elements[2];

                    var homeTeam = teams.Split('-').First();
                    var awayTeam = teams.Split('-').Last();

                    var homeGoals = int.Parse(matchResult.Split(':').First().Trim());
                    var awayGoals = int.Parse(matchResult.Split(':').Last().Trim());

                    var normalizedHomeTeamName =
                     Helper.NormalizeTeamName(homeTeam);
                    var normalizedAwayTeamName =
                        Helper.NormalizeTeamName(awayTeam);

                    Console.WriteLine(normalizedHomeTeamName + " VS. " + normalizedAwayTeamName);


                    var homeTeamId = db.team.First(x => x.TeamName == normalizedHomeTeamName).TeamID;
                    var awayTeamId = db.team.First(x => x.TeamName == normalizedAwayTeamName).TeamID;

                    var parsedDate = DateTime.Parse(date);

                    var matchAlreadyExists =
                             db.competitionmatch
                             .FirstOrDefault(x => x.HomeTeamID == homeTeamId &&
                                    x.AwayTeamID == awayTeamId &&
                                    x.MatchDate == parsedDate);

                    if (matchAlreadyExists != null)
                    {
                        Console.WriteLine("Match Exists!");
                        continue;
                    }

                    Helper.AddMatchDetailsToDb(db, homeTeamId, awayTeamId, homeGoals, awayGoals, parsedDate, competitionId);
                }
            }
        }

        public static void UpdateBetsOdds(string season, int competitionId)
        {
            var i = 1;
            var page = $"http://www.betexplorer.com/soccer/england/premier-league-{season}/results/";
            var dom = CQ.CreateFromUrl(page);
            var allResults = dom[".table-main tr"].Skip(1).ToList();

            using (var db = new sakilaEntities4())
            {
                foreach (var result in allResults)
                {
                    var resultOdds = new List<String>();
                    var odds = result.ChildElements.Where(x => x.ClassName == "table-main__odds" || x.ClassName == "table-main__odds colored").ToList();
                    foreach (var element in odds)
                    {
                        var text = element.InnerText;
                        var odd = element.GetAttribute("data-odd");
                        if (odd == null)
                        {
                            var e = element.FirstElementChild.FirstElementChild.FirstElementChild;
                            odd = e.GetAttribute("data-odd");
                        }

                        resultOdds.Add(odd);
                    }

                    var elements = result.ChildElements.Select(x => x.Cq().Text()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    if (elements.Count != 3)
                    {
                        continue;
                    }

                    Console.WriteLine(i++);

                    var teams = elements[0];
                    var matchResult = elements[1];
                    var date = elements[2];

                    if (matchResult.Equals("POSTP."))
                        continue;

                    var homeTeam = teams.Split('-').First();
                    var awayTeam = teams.Split('-').Last();

                    var homeGoals = int.Parse(matchResult.Split(':').First().Trim());
                    var awayGoals = int.Parse(matchResult.Split(':').Last().Trim());

                    var normalizedHomeTeamName =
                     Helper.NormalizeTeamName(homeTeam);
                    var normalizedAwayTeamName =
                        Helper.NormalizeTeamName(awayTeam);

                    Console.WriteLine(normalizedHomeTeamName + " VS. " + normalizedAwayTeamName);


                    var homeTeamId = db.team.First(x => x.TeamName == normalizedHomeTeamName).TeamID;
                    var awayTeamId = db.team.First(x => x.TeamName == normalizedAwayTeamName).TeamID;
                    var parsedDate = DateTime.Today;
                    try
                    {
                        parsedDate = DateTime.Parse(date);
                    }

                    catch
                    {


                        try
                        {
                            var year = season.Split('-').First();
                            var dateMonth = int.Parse(date.Split('.')[1]);
                            if (dateMonth <= 5)
                            {
                                year = season.Split('-').Last();
                            }
                            parsedDate = DateTime.Parse(date + year);
                        }

                        catch
                        {
                            if (date.Equals("Today"))
                            {
                                parsedDate = DateTime.Today;
                            }

                            else if (date.Equals("Yesterday"))
                            {
                                parsedDate = DateTime.Today.AddDays(-1);
                            }

                            else
                            {
                                throw new Exception("Failed to parse date!");
                            }
                        }

                    }

                    var match =
                             db.competitionmatch
                             .FirstOrDefault(x => x.HomeTeamID == homeTeamId &&
                                    x.AwayTeamID == awayTeamId &&
                                    x.MatchDate == parsedDate);

                    if (match == null)
                    {
                        throw new Exception();
                    }

                    var oddsAlreadyExists = db.matchodds.FirstOrDefault(x => x.MatchID == match.CompetitionMatchID);
                    if (oddsAlreadyExists != null)
                    {
                        continue;
                    }

                    var newOdds = new matchodds();
                    newOdds.HomeTeamOdds = decimal.Parse(resultOdds[0]);
                    newOdds.DrawOdds = decimal.Parse(resultOdds[1]);
                    newOdds.AwayTeamOdds = decimal.Parse(resultOdds[2]);
                    newOdds.MatchID = match.CompetitionMatchID;
                    db.matchodds.Add(newOdds);
                    db.SaveChanges();
                
                }
            }

        }

        public static void SolvePossessionIssue()
        {
            var i = 0;
            using (var db = new sakilaEntities4())
            {
                var evs = db.matchevent.Where(x => x.competitionmatch.CompetitionID == 2 && x.EventTypeID == 7 && x.eventvalue == 0).ToList();
                Console.WriteLine(evs.Count());
                foreach (var e in evs)
                {
                    Console.WriteLine(i++);
                    e.EventTypeID = 16;
                    db.SaveChanges();
                }
            }

        }

        public static void MainUpdatorFromEspn(int daysToTake, int competitionId)
        {
            using (var db = new sakilaEntities4())
            {
                for (var i = 0; i <= daysToTake; i++)
                {
                    var relevantDate = DateTime.Today.AddDays(i*-1);
                    Console.WriteLine(relevantDate);

                    var matches = GetMatchesForDate(relevantDate);
                    foreach (var match in matches)
                    {
                        var matchDetails = GetMatchStatisticsFromEspn(match);
                        if (matchDetails.Date == DateTime.Today)
                        {
                            // continue;
                        }
                        var normalizedHomeTeamName =
                             Helper.NormalizeTeamName(matchDetails.HomeTeam.Name);
                        var normalizedAwayTeamName =
                            Helper.NormalizeTeamName(matchDetails.AwayTeam.Name);

                        Console.WriteLine(normalizedHomeTeamName + " VS. " + normalizedAwayTeamName);


                        var homeTeamId = db.team.First(x => x.TeamName == normalizedHomeTeamName).TeamID;
                        var awayTeamId = db.team.First(x => x.TeamName == normalizedAwayTeamName).TeamID;

                        var matchAlreadyExists =
                            db.competitionmatch
                                .FirstOrDefault(x => x.HomeTeamID == homeTeamId &&
                                x.AwayTeamID == awayTeamId
                                && x.CompetitionID == competitionId);

                        if (matchAlreadyExists != null)
                        {
                            //check if events exists
                            var eventExist = db.matchevent.FirstOrDefault(x => x.competitionmatch.HomeTeamID == homeTeamId
                                                                      && x.competitionmatch.AwayTeamID == awayTeamId
                                                                      && x.competitionmatch.CompetitionID == competitionId
                                                                      && x.EventTypeID != 7);

                            if (eventExist != null)
                            {

                                //check if possession exists
                                var possessionExist = db.matchevent.FirstOrDefault(x => x.competitionmatch.HomeTeamID == homeTeamId
                                                                          && x.competitionmatch.AwayTeamID == awayTeamId
                                                                          && x.competitionmatch.CompetitionID == competitionId
                                                                          && x.EventTypeID == 7);

                                if (possessionExist != null)
                                {
                                    Console.WriteLine("Match Exists!");
                                    continue;
                                }

                                matchDetails.MatchID = matchAlreadyExists.CompetitionMatchID;
                                addOnlyPossessionToDb(matchDetails, db, homeTeamId, awayTeamId);
                                Console.WriteLine("updating events");
                                continue;
                            }

                            matchDetails.MatchID = matchAlreadyExists.CompetitionMatchID;
                            addOnlyStatisticsToDb(matchDetails, db, homeTeamId, awayTeamId);
                            Console.WriteLine("updating events");
                            continue;
                        }

                        AddFullMatchDetailsToDb(matchDetails, db, homeTeamId, awayTeamId);
                    }

                }
            }
        }


        public static void MainUpdatorFromEspn(int startYear, int startMonth, int startDay, int daysToTake, int competitionId)
        {
            var startDate = new DateTime(startYear, startMonth, startDay);
            using (var db = new sakilaEntities4())
            {
                for (var i = 0; i <= daysToTake; i++)
                {
                    var relevantDate = startDate.AddDays(i * -1);
                    Console.WriteLine(relevantDate);

                    var matches = GetMatchesForDate(relevantDate);
                    foreach (var match in matches)
                    {

                        try
                        {
                            var matchDetails = GetMatchStatisticsFromEspn(match);
                            if (matchDetails.Date == DateTime.Today)
                            {
                                continue;
                            }
                            var normalizedHomeTeamName =
                                 Helper.NormalizeTeamName(matchDetails.HomeTeam.Name);
                            var normalizedAwayTeamName =
                                Helper.NormalizeTeamName(matchDetails.AwayTeam.Name);

                            Console.WriteLine(normalizedHomeTeamName + " VS. " + normalizedAwayTeamName);


                            var homeTeamId = db.team.First(x => x.TeamName == normalizedHomeTeamName).TeamID;
                            var awayTeamId = db.team.First(x => x.TeamName == normalizedAwayTeamName).TeamID;

                            var matchAlreadyExists =
                                db.competitionmatch
                                    .FirstOrDefault(x => x.HomeTeamID == homeTeamId &&
                                    x.AwayTeamID == awayTeamId
                                    && x.CompetitionID == competitionId);

                            if (matchAlreadyExists != null)
                            {
                                //check if events exists
                                var eventExist = db.matchevent.FirstOrDefault(x => x.competitionmatch.HomeTeamID == homeTeamId
                                                                          && x.competitionmatch.AwayTeamID == awayTeamId
                                                                          && x.competitionmatch.CompetitionID == competitionId
                                                                          && x.EventTypeID != 7);

                                if (eventExist != null)
                                {

                                    //check if possession exists
                                    var possessionExist = db.matchevent.FirstOrDefault(x => x.competitionmatch.HomeTeamID == homeTeamId
                                                                              && x.competitionmatch.AwayTeamID == awayTeamId
                                                                              && x.competitionmatch.CompetitionID == competitionId
                                                                              && x.EventTypeID == 7);

                                    if (possessionExist != null)
                                    {
                                        Console.WriteLine("Match Exists!");
                                        continue;
                                    }

                                    matchDetails.MatchID = matchAlreadyExists.CompetitionMatchID;
                                    addOnlyPossessionToDb(matchDetails, db, homeTeamId, awayTeamId);
                                    Console.WriteLine("updating Possession");
                                    continue;
                                }

                                matchDetails.MatchID = matchAlreadyExists.CompetitionMatchID;
                                addOnlyStatisticsToDb(matchDetails, db, homeTeamId, awayTeamId);
                                Console.WriteLine("updating events");
                                continue;
                            }

                            AddFullMatchDetailsToDb(matchDetails, db, homeTeamId, awayTeamId);
                        }
                        catch
                        {
                            Console.WriteLine("Caught!");
                        }
                    }

                }
            }
        }


        public static void MainUpdator()
        {
            var matches = ExtractAllMatches()
                .ToList();
            using (var db = new sakilaEntities4())
            {
                foreach (var match in matches)
                {
                    var matchDetails = GetMatchStatistics(match);
                    var normalizedHomeTeamName =
                         Helper.NormalizeTeamName(matchDetails.HomeTeam.Name);
                    var normalizedAwayTeamName =
                        Helper.NormalizeTeamName(matchDetails.AwayTeam.Name);

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
                        Console.WriteLine("Match Exists!");
                        continue;
                    }

                    AddFullMatchDetailsToDb(matchDetails, db, homeTeamId, awayTeamId);
                }
            }

        }

        public static void CreateMatchesFile()
        {
            const string page = "http://www.bbc.com/sport/football/premier-league/fixtures";
            var dom = CQ.CreateFromUrl(page);
            var allMatches = new List<string>();
            var i = 0;
            var path = @"C:\Users\user\Desktop\DataProjects\PremierLeagueMatches2018.tsv";

            var allVenues = dom[".table-stats"].Take(50).ToList();
            foreach (var venue in allVenues)
            {
                Console.WriteLine(i++);
                var fullDate = venue.ChildElements.First().Cq().Text();
                var dateArr = fullDate.Trim().Split().ToList();
                var lastThreeWords = dateArr.GetRange(dateArr.Count - 3, 3);
                var potentialDate = string.Join(" ", lastThreeWords)
                    .Replace("th", "")
                    .Replace("nd", "")
                    .Replace("st", "")
                    .Replace("rd", "");
                DateTime date;
                if (!DateTime.TryParse(potentialDate, out date))
                {
                    continue;
                }
                var elements = venue.ChildElements.Last().ChildElements.ToList();
                var matchesToAdd = elements.Where(x => x.ClassName == "preview");
                foreach (var match in matchesToAdd)
                {
                    var matchElements = match.ChildElements.First(x => x.ClassName == "match-details").ChildElements.First().ChildElements;
                    var homeTeam = Helper.NormalizeTeamName(matchElements.First(x => x.ClassName == "team-home teams").Cq().Text().Trim());
                    var awayTeam = Helper.NormalizeTeamName(matchElements.First(x => x.ClassName == "team-away teams").Cq().Text().Trim());
                    allMatches.Add(homeTeam + "\t" + awayTeam + "\t" + date);
                }
            }

            File.WriteAllLines(path, allMatches);

        }

        public static List<FutureMatch> GetNextMatches(int nextDaysToGet)
        {
            var path = @"C:\Users\user\Desktop\DataProjects\2019\Fixtures.tsv";
            var today = DateTime.Today;
            var nextDays = today.AddDays(nextDaysToGet);

            var matches = File.ReadAllLines(path)
                .Select(tab => tab.Split('\t'))
                .Select(x => new FutureMatch {HomeTeam = x[1], AwayTeam = x[2], Date = DateTime.Parse(x[0])})
                .Where(x => x.Date >= today && x.Date <= nextDays)
                .ToList();

            return matches;

        }

        public class FutureMatch
        {
            public DateTime Date;
            public string HomeTeam;
            public string AwayTeam;
        }

        

        public class PlayerDetails
        {
            public string PlayerName;
            public string PlayerPosition;
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
}
