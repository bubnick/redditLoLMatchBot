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
            Match match = new Match();
            var r = new Reddit();
            var u = r.LogIn("LeagueMatch", "D6rCfKgzehv5wdLY");
            var s = r.GetSubreddit("/r/LeagueMatch");
            foreach(var c in s.Comments.Take(50))
            {
                if (c.Body.Contains("League Match: "))
                {
                    c.Reply("Output!");
                }
                //match.getMatch(matchID);
            }
            

        }
    }
}
