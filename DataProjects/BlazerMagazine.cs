using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;
using CsQuery.ExtensionMethods.Internal;

namespace DataProjects
{
    public class BlazerMagazine
    {
        public static List<ChampionsLeagueStage> ChampionsLeagueStats()
        {
            var allStages = new List<ChampionsLeagueStage>();
            for (int i = 2000; i <= 2016; i++)
            {
                Console.WriteLine(i);
                var page = $"http://www.uefa.com/uefachampionsleague/season={i}/clubs/index.html";
                var dom = CQ.CreateFromUrl(page);
                var list = dom[".clubList.clearfix"];
                foreach (var l in list)
                {
                    if (l.ChildElements.Count() == 32)
                    {
                        var last32 = l.ChildElements
                            .Select(x => x.Cq().Text())
                            .Select(x =>
                                new ChampionsLeagueTeam
                                {
                                    Team = x.Split('(').First().Trim(),
                                    Country = x.Split('(').Last().Trim(')').Trim()
                                }).ToList();

                        var newItem = new ChampionsLeagueStage
                        {
                            Name = "Group Stage",
                            Teams = last32,
                            Year = i
                        };

                        var alreadyExists = allStages
                            .FirstOrDefault(x => x.Year == newItem.Year && x.Name == newItem.Name);

                        if (alreadyExists != null)
                        {
                            continue;
                        }

                        allStages.Add(newItem);

                    }

                    if (l.ChildElements.Count() == 16)
                    {
                        var last16 = l.ChildElements
                            .Select(x => x.Cq().Text())
                            .Select(x =>
                                new ChampionsLeagueTeam
                                {
                                    Team = x.Split('(').First().Trim(),
                                    Country = x.Split('(').Last().Trim(')').Trim()
                                }).ToList();

                        var newItem = new ChampionsLeagueStage
                        {
                            Name = "Last 16",
                            Teams = last16,
                            Year = i
                        };

                        var alreadyExists = allStages
                            .FirstOrDefault(x => x.Year == newItem.Year && x.Name == newItem.Name);

                        if (alreadyExists != null)
                        {
                            continue;
                        }

                        allStages.Add(newItem);
                    }

                    if (l.ChildElements.Count() == 8)
                    {
                        var last8 = l.ChildElements
                            .Select(x => x.Cq().Text())
                            .Select(x =>
                                new ChampionsLeagueTeam
                                {
                                    Team = x.Split('(').First().Trim(),
                                    Country = x.Split('(').Last().Trim(')').Trim()
                                }).ToList();

                        var newItem = new ChampionsLeagueStage
                        {
                            Name = "Quarter Final",
                            Teams = last8,
                            Year = i
                        };

                        var alreadyExists = allStages
                            .FirstOrDefault(x => x.Year == newItem.Year && x.Name == newItem.Name);

                        if (alreadyExists != null)
                        {
                            continue;
                        }

                        allStages.Add(newItem);
                    }

                    if (l.ChildElements.Count() == 4)
                    {
                        var last4 = l.ChildElements
                            .Select(x => x.Cq().Text())
                            .Select(x =>
                                new ChampionsLeagueTeam
                                {
                                    Team = x.Split('(').First().Trim(),
                                    Country = x.Split('(').Last().Trim(')').Trim()
                                }).ToList();

                        var newItem = new ChampionsLeagueStage
                        {
                            Name = "Half Final",
                            Teams = last4,
                            Year = i
                        };

                        var alreadyExists = allStages
                            .FirstOrDefault(x => x.Year == newItem.Year && x.Name == newItem.Name);

                        if (alreadyExists != null)
                        {
                            continue;
                        }

                        allStages.Add(newItem);
                    }

                    if (l.ChildElements.Count() == 2)
                    {
                        var last2 = l.ChildElements
                            .Select(x => x.Cq().Text())
                            .Select(x =>
                                new ChampionsLeagueTeam
                                {
                                    Team = x.Split('(').First().Trim(),
                                    Country = x.Split('(').Last().Trim(')').Trim()
                                }).ToList();

                        var newItem = new ChampionsLeagueStage
                        {
                            Name = "Final",
                            Teams = last2,
                            Year = i
                        };

                        var alreadyExists = allStages
                            .FirstOrDefault(x => x.Year == newItem.Year && x.Name == newItem.Name);

                        if (alreadyExists != null)
                        {
                            continue;
                        }

                        allStages.Add(newItem);
                    }
                }
            }

            return allStages;
        }

        public static List<string> GetCountriesInStatgeBetween(string stage, int fromYear, int untilYear)
        {
            var allStages = ChampionsLeagueStats();
            var test = allStages.Where(x => x.Name == stage && x.Year >= fromYear && x.Year <= untilYear);
            var ret = allStages.Where(x => x.Name == stage && x.Year >= fromYear && x.Year <= untilYear)
                .SelectMany(x => x.Teams)
                .GroupBy(x => x.Country)
                .Select(x => x.First().Country)
                .OrderBy(x => x)
                .ToList();

            return ret;
        }

        public static void GetAllCountriesInStatgeBetween(string stage, int fromYear, int untilYear)
        {
            var allStages = ChampionsLeagueStats();
            var test = allStages.Where(x => x.Name == stage && x.Year >= fromYear && x.Year <= untilYear);
            var ret = allStages.Where(x => x.Name == stage && x.Year >= fromYear && x.Year <= untilYear)
                .SelectMany(x => x.Teams)
                .GroupBy(x => x.Country)
                .OrderByDescending(x => x.Count())
                .ToList();

            var print = string.Join("\n", ret.Select(x => x.First().Country + "\t" + x.Count()));
            Console.WriteLine(print);
            Console.Read();
        }

        public static void GetAllTeamsInStatgeBetween(string stage, int fromYear, int untilYear)
        {
            var allStages = ChampionsLeagueStats();
            var test = allStages.Where(x => x.Name == stage && x.Year >= fromYear && x.Year <= untilYear);
            var ret = allStages.Where(x => x.Name == stage && x.Year >= fromYear && x.Year <= untilYear)
                .SelectMany(x => x.Teams)
                .GroupBy(x => x.Team)
                .OrderByDescending(x => x.Count())
                .ToList();

            var print = string.Join("\n", ret.Select(x => x.First().Team + "\t" + x.Count()));
            Console.WriteLine(print);
            Console.Read();
        }

        public static void DistinctParticipantFromCountryInStageBetween(string stage, int fromYear,
            int untilYear)
        {
            var allStages = ChampionsLeagueStats();

            var ret = allStages.Where(x => x.Name == stage && x.Year >= fromYear && x.Year <= untilYear)
                .SelectMany(x => x.Teams)
                .Distinct()
                .GroupBy(x => x.Team)
                .Select(x => x.First())
                .GroupBy(x => x.Country)
                .OrderByDescending(x => x.Count())
                .Select(x => new {x.First().Country, Count = x.Count(), All = x})
                .ToList();

            var print = string.Join("\n", ret.Select(x => x.Country + "\t" + x.Count + "\n\n" + string.Join("\n", x.All.Select(y => y.Team))));
            Console.WriteLine(print);
            Console.Read();

        }
        public class ChampionsLeagueStage
        {
            public int Year;
            public string Name;
            public List<ChampionsLeagueTeam> Teams;
        }
        public class ChampionsLeagueTeam
        {
            public string Team;
            public string Country;
        }
    }
}
