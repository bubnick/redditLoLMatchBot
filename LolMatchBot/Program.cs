using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RiotSharp;

namespace LolMatchBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Match m = new Match();
            var r = new Reddit();
            var u = r.LogIn("LeagueMatch", "D6rCfKgzehv5wdLY");
            var s = r.GetSubreddit("/r/LeagueMatch");
            List<String> alreadyReplied = new List<String>();

            while(true)
            {

                DateTime yesterday = DateTime.Today.AddDays(-1);

                foreach (var c in s.Comments.Take(50))
                {
                    //TODO: store list in text file as well so that if bot needs to be rebooted, dont lose list to memory
                    //TODO: Check other servers besides NA!!!!
                    //Only check comments that are less than a day old
                    if (c.Body.Contains("Match: ") && c.Created.CompareTo(yesterday) >= 0  && !alreadyReplied.Contains(c.Id))
                    {

                        try
                        {
                            //TODO: This is a gross and dirty way of doing it. Write a method to loop through the comment body and get a number
                            //Ie. if char is digit add to string then parse string
                            long matchID = long.Parse(c.Body.Substring(13, 11));
                            var match = m.getMatch(matchID, "NA");
                            var time = match.MatchDuration;
                            String winner = "";
                            long blueTotalKills = 0;
                            long purpleTotalKills = 0;

                            foreach (var team in match.Teams)
                            {
                                if (team.Winner)
                                {
                                    winner = team.TeamId == 100 ? "Blue" : "Purple";
                                }
                            }

                            foreach (var p in match.Participants)
                            {
                                if (p.TeamId == 100)
                                {
                                    blueTotalKills += p.Stats.Kills;
                                }
                                else
                                {
                                    purpleTotalKills += p.Stats.Kills;
                                }
                            }

                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("**Match ID: " + matchID + "**\n\n**" + winner + "** team won **" + (winner == "Blue" ? (blueTotalKills + "-" + purpleTotalKills) : (purpleTotalKills + "-" + blueTotalKills)) + "** in **" + time.ToString() + "**\n");
                            sb.AppendLine("***Blue***\n");
                            sb.AppendLine("Champion | Level | KDA | Gold |  CS | Item Build");
                            sb.AppendLine("---------|----------|----------|----------|----------|----------");
                            foreach (var p in match.Participants)
                            {
                                //If blue
                                if (p.TeamId == 100)
                                {
                                    long totalCS = p.Stats.MinionsKilled + p.Stats.NeutralMinionsKilled;
                                    List<string> items = new List<string>();
                                
                                    if(p.Stats.Item0 != null && p.Stats.Item0 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item0));
                                    }

                                    if(p.Stats.Item1 != null && p.Stats.Item1 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item1));
                                    }

                                    if(p.Stats.Item2 != null && p.Stats.Item2 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item2));
                                    }

                                    if(p.Stats.Item3 != null && p.Stats.Item3 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item3));
                                    }

                                    if(p.Stats.Item4 != null && p.Stats.Item4 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item4));
                                    }

                                    if(p.Stats.Item5 != null && p.Stats.Item5 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item5));
                                    }

                                    if(p.Stats.Item6 != null && p.Stats.Item6 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item6));
                                    }

                                    sb.Append(m.getChampIcon(p.ChampionId) + " | " + p.Stats.ChampLevel + " | " + p.Stats.Kills + "/" + p.Stats.Deaths + "/" + p.Stats.Assists + " | " + p.Stats.GoldEarned + " | " + totalCS + " | ");
                                    string strItems = "";
                                    foreach(string str in items)
                                    {
                                        strItems += str + ", ";
                                    
                                    }
                                    strItems = strItems.Substring(0, strItems.Length - 2);
                                    sb.AppendLine(strItems);
                                }
                            }
                            sb.AppendLine("\n***Purple***\n");
                            sb.AppendLine("Champion | Level | KDA | Gold |  CS | Item Build");
                            sb.AppendLine("---------|----------|----------|----------|----------|----------");
                            foreach (var p in match.Participants)
                            {
                                //If blue
                                if (p.TeamId == 200)
                                {
                                    long totalCS = p.Stats.MinionsKilled + p.Stats.NeutralMinionsKilled;
                                    List<string> items = new List<string>();

                                    if (p.Stats.Item0 != null && p.Stats.Item0 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item0));
                                    }

                                    if (p.Stats.Item1 != null && p.Stats.Item1 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item1));
                                    }

                                    if (p.Stats.Item2 != null && p.Stats.Item2 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item2));
                                    }

                                    if (p.Stats.Item3 != null && p.Stats.Item3 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item3));
                                    }

                                    if (p.Stats.Item4 != null && p.Stats.Item4 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item4));
                                    }

                                    if (p.Stats.Item5 != null && p.Stats.Item5 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item5));
                                    }

                                    if (p.Stats.Item6 != null && p.Stats.Item6 != 0)
                                    {
                                        items.Add(m.getItem(p.Stats.Item6));
                                    }

                                    sb.Append(m.getChampIcon(p.ChampionId) + " | " + p.Stats.ChampLevel + " | " + p.Stats.Kills + "/" + p.Stats.Deaths + "/" + p.Stats.Assists + " | " + p.Stats.GoldEarned + " | " + totalCS + " | ");
                                    string strItems = "";
                                    foreach (string str in items)
                                    {
                                        strItems += str + ", ";

                                    }
                                    strItems = strItems.Substring(0, strItems.Length - 2);
                                    sb.AppendLine(strItems);
                                }
                            }

                            sb.AppendLine("\n^^I ^^am ^^a ^^bot! ^^summon ^^me ^^with ^^'Match: ^^1234567890'. ^^Currently ^^NA ^^only!");
                            sb.AppendLine("\n^^Maintained ^^by ^^/u/bubnick ^^v1.0");
                            c.Reply(sb.ToString());
                            alreadyReplied.Add(c.Id);
                            Console.WriteLine("Commented on commentID: " + c.Id + ", matchID: " + matchID);
                        }
                        catch (Exception exp)
                        {
                            //Console.WriteLine("CommentID: " + c.Id + " is invalid");
                        }

                    }
                }


            }
        }
    }
}
