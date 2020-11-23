using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProjects
{
    public class Helper
    {
        public static string GetContentWithinParenthesis(string s)
        {
            var output = s.Split('(', ')')[1];
            return output;


        }

        public static List<string> SpecialSplit(string toSplit)
        {
            var toReturn = new List<string>();
            var indexesOf = AllIndexesOf(toSplit, "),").Select(x => x + 2).ToList();
            if (!indexesOf.Any())
            {
                toReturn.Add(toSplit);
                return toReturn;
            }

            if (indexesOf.Count == 1)
            {
                var relevant = indexesOf[0];
                var first = toSplit.Substring(relevant, toSplit.Length - relevant);
                var second = toSplit.Substring(0, relevant).Trim(',');
                toReturn.Add(first);
                toReturn.Add(second);
                return toReturn;
            }
            var j = 0;
            for (var i = 0; i < indexesOf.Count() - 1; i++)
            {
                var curIndex = indexesOf[i];
                var nextIndex = indexesOf[i + 1];
                toReturn.Add(toSplit.Substring(curIndex, nextIndex - curIndex));
                j = i;
            }
            var lastIndex = indexesOf[j];
            toReturn.Add(toSplit.Substring(lastIndex, toSplit.Length - lastIndex));
            return toReturn;
        }

        public static IEnumerable<int> AllIndexesOf(string str, string searchstring)
        {
            int minIndex = str.IndexOf(searchstring);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
            }
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

        public static string NormalizePlayerName(string name)
        {
            return name
                .Replace("&#233;", "é")
                .Replace("&#214;", "Ö")
                .Replace("A¶", "ö")
                .Replace("&#225;", "á")
                .Replace("&#239;", "i")
                .Replace("&#235;", "e")
                .Replace("&#250;", "u")
                .Replace("&#252;", "u")
                .Replace("&#237;", "i")
                .Replace("&#193;", "a")
                .Replace("&#248;", "i")
                .Replace("&#243;", "o")
                .Replace("&#246;", "u")
                .Replace("&#241", "n")
                .Replace("&#229;", "a")
                .Replace("A¼", "u")
                .Replace("A¡", "a")
                .Replace("A©", "é")
                .Replace("A³", "ó")
                .Replace("Ã¼", "")
                .Replace("A\u0096", "o")
                .Replace("A±", "n")
                .Replace("RamA­rez", "Ramirez")
                .Trim();
        }


        public static string NormalizeTeamName(string name)
        {
            var result = name.Trim();
            if (result.Equals("QPR"))
            {
                result = "Queens Park Rangers";
            }

            if (result.Equals("Wolves"))
            {
                result = "Wolverhampton wanderers";
            }

            if (result.Equals("Cardiff"))
            {
                result = "Cardiff City";
            }

            if (result.StartsWith("Man "))
            {
                result = result.Replace("Man ", "Manchester ");
            }

            if (result.Contains("Utd"))
            {
                result = result.Replace("Utd", "United");
            }

            if (result.EndsWith(" Ham"))
            {
                result = result.Replace("Ham", "Ham United");
            }

            if (result.Equals("Tottenham"))
            {
                result = result + " Hotspur";
            }

            if (result.Equals("Spurs"))
            {
                result = "Tottenham Hotspur";
            }
            if (result.Equals("West Brom"))
            {
                result = "West Bromwich Albion";
            }

            if (result.Contains(" and "))
            {
                result = result.Replace(" and ", " & ");
            }

            if (result.Trim().Equals("Huddersfield"))
            {
                result = result + " Town";
            }

            if (result.Trim().Equals("Brighton"))
            {
                result = result + " & Hove Albion";
            }

            if (result.Equals("Newcastle"))
            {
                result = "Newcastle United";
            }
            
            if (!result.EndsWith("City")
                && (result.StartsWith("Norwich") || result.StartsWith("Leicester")
                || result.StartsWith("Stoke") || result.StartsWith("Hull")
                 || result.StartsWith("Swansea")
                ))
            {
                result = result + " City";
            }

            result = result.Replace("AFC", "").Trim();
                

            return result;
        }

        public static List<DataObjects.Goal> GetGoalsFromString(string s)
        {
            var toReturn = new List<DataObjects.Goal>();
            //Olivier Giroud (5, 78, 80)
            //Mark Bunn (90 + 2 OG)
            //Wayne Rooney (43)

            var name = s.Split('(').First().Trim();
            var contentWithinParentheses = Helper.GetContentWithinParenthesis(s).ToLower();
            var isOwnGoals = contentWithinParentheses.EndsWith("og");
            var minutes = contentWithinParentheses.Replace("og", "").Trim().Split(',').ToList();

            foreach (var minute in minutes)
            {
                var g = new DataObjects.Goal();
                g.IsOwnGoal = isOwnGoals;
                g.Scorer = name;

                var min = minute.ToLower().Replace("pen", "").Replace("minutes", "").Replace("'", "").Trim();
                if (minute.Contains("+"))
                {
                    min = minute.Split('+').First().Trim().Replace("'", "");
                }


                g.Minute = int.Parse(min);

                toReturn.Add(g);
            }


            return toReturn;

        }

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static bool IsRelevantMatchLink(string s)
        {
            if (!s.Contains("/sport/football"))
            {
                return false;
            }

            var lastIdentifire = s.Split('/').Last();
            double d;
            if (!double.TryParse(lastIdentifire, out d))
            {
                return false;
            }

            return true;
        }
        public static void AddMatchDetailsToDb(DataObjects.MatchDetails match, sakilaEntities4 db, int homeTeamID, int awayTeamID, int competitionID = 10)
        {
            var newMatch = new competitionmatch();
            newMatch.HomeTeamID = homeTeamID;
            newMatch.AwayTeamID = awayTeamID;
            newMatch.HomeGoals = match.HomeTeam.Goals;
            newMatch.AwayGoals = match.AwayTeam.Goals;
            newMatch.WinnerTeamID = Helper.GetWinnerTeamID(homeTeamID, match.HomeTeam.Goals, awayTeamID,
                match.AwayTeam.Goals);
            newMatch.MatchDate = match.Date;
            newMatch.CompetitionID = competitionID;

            db.competitionmatch.Add(newMatch);
            db.SaveChanges();
            match.MatchID = newMatch.CompetitionMatchID;
        }

        public static List<DataObjects.AttributeType> GetTeamAttributeFromDict(int teamId,
            Dictionary<int, List<DataObjects.AttributeType>> attributesDict)
        {
            if (attributesDict.ContainsKey(teamId))
            {
                return attributesDict.First(x => x.Key == teamId).Value;
            }

            return new List<DataObjects.AttributeType>();
        }

        public static List<string> GetTeamAttributesList(int teamId,
            Dictionary<int, List<DataObjects.AttributeType>> attributesDict)
        {
            var toReturn = new List<string>();
            var attributes = GetTeamAttributeFromDict(teamId, attributesDict);
            foreach (var attribute in attributes)
            {
                toReturn.Add($"This Team is One of the Most {attribute.ToString().Replace("_", " ")} Teams in the League.");
            }

            return toReturn;
        }

        public static void AddMatchDetailsToDb(sakilaEntities4 db, int homeTeamID, int awayTeamID, int homeTeamGoals, int awayTeamGoals, DateTime matchDate, int competitionMatchID)
        {
            var newMatch = new competitionmatch();
            newMatch.HomeTeamID = homeTeamID;
            newMatch.AwayTeamID = awayTeamID;
            newMatch.HomeGoals = homeTeamGoals;
            newMatch.AwayGoals = awayTeamGoals;
            newMatch.WinnerTeamID = Helper.GetWinnerTeamID(homeTeamID, homeTeamGoals, awayTeamID,
                awayTeamGoals);
            newMatch.MatchDate = matchDate;
            newMatch.CompetitionID = competitionMatchID;

            db.competitionmatch.Add(newMatch);
            db.SaveChanges();
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
        public static int GetPlayerId(string name, int teamID, sakilaEntities4 db)
        {
            var nameArr = name.Replace("Ä\u0087", "c").Split().ToList();
            var lastName = nameArr.Last();
            var p =
                db.player.FirstOrDefault(
                    x => x.TeamID == teamID && x.PlayerName.EndsWith(lastName));

            if (p != null)
            {
                return p.PlayerID;
            }

            return AddPlayerNameAsNa(name, teamID, db);
        }

        public static int AddPlayerNameAsNa(string name, int teamId, sakilaEntities4 db)
        {
            var newPlayer = new player
            {
                TeamID = teamId,
                PlayerName = "NA " + name,
                PositionID = 1 //NA
            };

            db.player.Add(newPlayer);
            db.SaveChanges();

            return newPlayer.PlayerID;
        }

        public static void UpdateMarketValue()
        {
            var allValues = new Dictionary<string, int>();
            allValues.Add("Arsenal", 398);
            allValues.Add("Manchester United", 454);
            allValues.Add("Manchester City", 440);
            allValues.Add("Liverpool", 329);
            allValues.Add("Tottenham Hotspur", 310);
            allValues.Add("Leicester City", 178);
            allValues.Add("Southampton", 169);
            allValues.Add("West Ham United", 205);
            allValues.Add("Stoke City", 146);
            allValues.Add("Chelsea", 437);
            allValues.Add("Everton", 203);
            allValues.Add("Swansea City", 90);
            allValues.Add("Watford", 110);
            allValues.Add("West Bromwich Albion", 91);
            allValues.Add("Crystal Palace", 134);
            allValues.Add("Bournemouth", 103);
            allValues.Add("Sunderland", 77);
            allValues.Add("Burnley", 56);
            allValues.Add("Middlesbrough", 89);
            allValues.Add("Hull City", 70);
            allValues.Add("Newcastle United", 102);
            allValues.Add("Norwich City", 61);
            allValues.Add("Aston Villa", 72);
            using (var db =new sakilaEntities4())
            {
                foreach (var value in allValues)
                {
                    Console.WriteLine(value.Key);
                    var team = db.team.First(x => x.TeamName == value.Key);
                    team.MarketValue = value.Value;
                }

                db.SaveChanges();
            }

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
                var scorer = NormalizePlayerName(goal.Scorer); 
                if (goal.IsOwnGoal)
                {
                    newGoal.ScorerID = ownGoalID;
                }
                else
                {
                    newGoal.ScorerID = GetPlayerId(scorer, homeTeamID, db);
                }

                db.matchgoal.Add(newGoal);
            }

            foreach (var goal in match.AwayTeam.GoalsDetails)
            {
                var newGoal = new matchgoal();
                newGoal.MatchID = match.MatchID;
                newGoal.ScoringMinute = goal.Minute;
                newGoal.TeamID = awayTeamID;
                var scorer = NormalizePlayerName(goal.Scorer);
                if (goal.IsOwnGoal)
                {
                    newGoal.ScorerID = ownGoalID;
                }
                else
                {
                    newGoal.ScorerID = GetPlayerId(scorer, awayTeamID, db);
                }

                db.matchgoal.Add(newGoal);
            }

            db.SaveChanges();
        }

        public static List<string> LookForSimilarSequencesInList(List<string> givenList, string sequence)
        {
            var sequenceLength = sequence.Count();
            var toReturn = new List<string>();
            for (var i = 0; i <= givenList.Count - sequenceLength - 1; i++)
            {
                var curSequence = "";
                for (var j = 0; j < sequenceLength; j++)
                {
                    curSequence = curSequence + "" + givenList[j + i];
                }

                if (curSequence.Trim() == sequence)
                {
                    toReturn.Add(givenList[i + sequenceLength]);
                }
            }

            return toReturn;
        }

        public static IEnumerable<IEnumerable<T>> GetPermutationsWithRept<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutationsWithRept(list, length - 1)
                .SelectMany(t => list,
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static List<string> GetAllLettersCombination(int count)
        {
            var letters = new List<string>
            {
                "W", "D", "L"
            };

            var allLists = GetPermutationsWithRept(letters, count)
                .Select(x => x.ToList())
                .Select(x => string.Join("", x))
                .ToList();
            return allLists;
        }


        public static Dictionary<string, List<LetterDistribution>> BuildSequencesDict(List<List<string>> allLettersLists, int sequenceAmount = 3)
        {
            var dict = new Dictionary<string, List<LetterDistribution>>();
            var lettersCombination = GetAllLettersCombination(sequenceAmount);
            foreach (var lettersSeq in lettersCombination)
            {
                var t = allLettersLists
                    .SelectMany(x => LookForSimilarSequencesInList(x, lettersSeq))
                    .ToList();
                var h = GetLetterExpectation(t);
                dict.Add(lettersSeq, h);
            }

            return dict;
        }

        public static List<LetterDistribution> GetLetterExpectation(List<string> givenList)
        {
            var toReturn = new List<LetterDistribution>();
            var grouped = givenList.GroupBy(x => x);

            foreach (var group in grouped)
            {
                var count = group.Count();
                var percent = (decimal)count / givenList.Count;
                var letter = group.First();
                var newItem = new LetterDistribution
                {
                    Letter = letter,
                    Percent = percent,
                    Count = count
                };

                toReturn.Add(newItem);
            }

            return toReturn;
        }

        public static void PrintUpdatedFullExpectedPrediction(int sequenceAmount = 3)
        {
            var resultPath = $@"C:\Users\user\Desktop\DataProjects\FullExpectedPrediction{sequenceAmount}.tsv";
            var combinedLetterDict = LettersSequenceCalculator.GetCombinedLettersDictionary(sequenceAmount)
            .Where(x => x.Value != null && x.Value.Any())
            .OrderByDescending(x => x.Value.OrderByDescending(y => y.Percent).First().Percent)
            .Select(x => x.Key + ":: " + Math.Round(x.Value.OrderByDescending(y => y.Percent).First().Percent, 2)
            + " ::" + x.Value.OrderByDescending(y => y.Percent).First().Letter
            + " ::" + x.Value.OrderByDescending(y => y.Percent).First().Count
            )
            .ToList();

            File.WriteAllLines(resultPath, combinedLetterDict);
        }

        public static List<int> GetParticipatedTeamList(sakilaEntities4 db, int competitionID)
        {
            var result = db.competitionmatch.Where(x => x.CompetitionID == competitionID)
                .Select(x => x.HomeTeamID)
                .Distinct()
                .ToList();

            return result;
        }

        public static string PrintMatchDetails(int competitionMatchId, sakilaEntities4 db)
        {
            var match = db.competitionmatch.First(x => x.CompetitionMatchID == competitionMatchId);
            var homeTeam = db.team.First(x => x.TeamID == match.HomeTeamID).TeamName;
            var awayTeam = db.team.First(x => x.TeamID == match.AwayTeamID).TeamName;

            return $"{homeTeam} Vs. {awayTeam} {match.HomeGoals}:{match.AwayGoals}";
        }

        public static List<string> GetNearestItems(List<string> items, int index, int itemsToTake)
        {
            var num = itemsToTake / 2;

            if (index <= num)
            {
                return items.Take(num + 1).ToList();
            }

            if (index + num >= items.Count)
            {
                return items.TakeLast(num + 1).ToList();
            }

            var startPoint = index - num;

            return items.GetRange(startPoint, itemsToTake + 1);
        }

        public class LetterDistribution
        {
            public string Letter;
            public decimal Percent;
            public int Count;
        }

    }
}
