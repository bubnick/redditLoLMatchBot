using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RiotSharp;
using RiotSharp.MatchEndpoint;

namespace LolMatchBot
{
    class Program
    {
        static void Main(string[] args)
        {
            String acctName = "LeagueMatch";
            String acctPassword = "";
            String subreddit = "/r/LeagueMatch";

            //Prompt for pw
            Console.WriteLine("Enter password for: " + acctName);
            acctPassword = Console.ReadLine();

            // TODO: Should The match generation be done every loop rather then at start? Depends on if API handles new games after it has been generated
            //Var cause lazy
            Match api = new Match();
            var reddit = new Reddit();
            var user = reddit.LogIn(acctName, acctPassword);
            var subr = reddit.GetSubreddit(subreddit);

            //List<String> alreadyReplied = new List<String>();
            //Get comments here
            HashSet<Comment> prevComments = getComments();

            while (true)
            {
                //Get a datetime object for yesterday
                DateTime yesterday = DateTime.Today.AddDays(-1);

                foreach (var c in subr.Comments.Take(50))
                {
                    //Only check comments that are less than a day old
                    Comment comment = new Comment(c.Id, c.Body);
                    if (c.Body.Contains("Match: ") && c.Created.CompareTo(yesterday) >= 0 && !prevComments.Contains(comment))
                    {

                        try
                        {
                            String table = buildTable(c, api);                                                       
                            c.Reply(table);
                            prevComments.Add(comment);
                            bool success = storeComment(comment);

                            if (success)
                            {
                                Console.WriteLine("Commented on commentID: " + c.Id/* + ", matchID: " + matchID*/ + "\nSuccessfully saved comment.");
                            }
                            else
                            {
                                Console.WriteLine("Commented on commentID: " + c.Id/* + ", matchID: " + matchID*/ + "\nUnsuccessfully saved comment.");

                            }
                        }
                        catch (Exception exp)
                        {
                            //Dont want to write anything as this will catch when a comment doesnt contain our string
                            //Console.WriteLine("CommentID: " + c.Id + " is invalid");
                        }

                    }
                }


            }
        }

        private static HashSet<Comment> getComments()
        {
            HashSet<Comment> comments = new HashSet<Comment>();
            CommentToSerialize commentToSerialize = new CommentToSerialize();

            Serializer serializer = new Serializer();
            //TODO: Add some error checking to check for corrupt vals
            commentToSerialize = serializer.DeSerializeComment("prevComments.bin");
            comments = commentToSerialize.Comments;

            return comments;
        }

        private static bool storeComment(Comment comment)
        {
            HashSet<Comment> comments = new HashSet<Comment>();
            comments.Add(comment);
            CommentToSerialize commentToSerialize = new CommentToSerialize();
            commentToSerialize.Comments = comments;

            Serializer serializer = new Serializer();
            try
            {
                serializer.SerializeComment("prevComments.bin", commentToSerialize);
                return true;
            }
            catch(Exception exp)
            {
                Console.WriteLine(exp);
                return false;
            }
        }

        private static string buildTable(RedditSharp.Things.Comment comment, Match api)
        {
            //FIX: This is a gross and dirty way of doing it. Write a method to loop through the comment body and get a number
            //FIX: Also need to generate region from this
            //Ie. if char is digit add to string then parse string
            long matchID = long.Parse(comment.Body.Substring(13, 11));
            var match = api.getMatch(matchID, "NA");
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

            //TODO: Find a better way to write these tables, break out into methods as well since it is the same code with diff vars
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

                    if (p.Stats.Item0 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item0));
                    }

                    if (p.Stats.Item1 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item1));
                    }

                    if (p.Stats.Item2 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item2));
                    }

                    if (p.Stats.Item3 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item3));
                    }

                    if (p.Stats.Item4 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item4));
                    }

                    if (p.Stats.Item5 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item5));
                    }

                    if (p.Stats.Item6 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item6));
                    }

                    sb.Append(api.getChampIcon(p.ChampionId) + " | " + p.Stats.ChampLevel + " | " + p.Stats.Kills + "/" + p.Stats.Deaths + "/" + p.Stats.Assists + " | " + p.Stats.GoldEarned + " | " + totalCS + " | ");
                    string strItems = "";
                    foreach (string str in items)
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

                    if (p.Stats.Item0 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item0));
                    }

                    if (p.Stats.Item1 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item1));
                    }

                    if (p.Stats.Item2 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item2));
                    }

                    if (p.Stats.Item3 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item3));
                    }

                    if (p.Stats.Item4 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item4));
                    }

                    if (p.Stats.Item5 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item5));
                    }

                    if (p.Stats.Item6 != 0)
                    {
                        items.Add(api.getItem(p.Stats.Item6));
                    }

                    sb.Append(api.getChampIcon(p.ChampionId) + " | " + p.Stats.ChampLevel + " | " + p.Stats.Kills + "/" + p.Stats.Deaths + "/" + p.Stats.Assists + " | " + p.Stats.GoldEarned + " | " + totalCS + " | ");
                    string strItems = "";
                    foreach (string str in items)
                    {
                        strItems += str + ", ";

                    }
                    strItems = strItems.Substring(0, strItems.Length - 2);
                    sb.AppendLine(strItems);
                }
            }

            sb.AppendLine("\n^^I ^^am ^^a ^^bot! ^^summon ^^me ^^with ^^'Match: ^^1234567890'.");
            sb.AppendLine("\n^^Maintained ^^by ^^/u/bubnick ^^v1.2");

            return sb.ToString();
        }
    }
}
