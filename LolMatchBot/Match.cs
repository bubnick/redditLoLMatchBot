using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;

namespace LolMatchBot
{
    class Match
    {
        private static string key = "0f8aa821-afe1-4591-a536-af0fb7f910d5";
        StaticRiotApi api = StaticRiotApi.GetInstance(key);

        public RiotSharp.MatchEndpoint.MatchDetail getMatch(long matchID, String region)
        {
            //Todo do check on regionID
            var api = RiotApi.GetInstance(key);
            try
            {
                //TODO: Check Region ID and get match ID accordingly
                var match = api.GetMatch(Region.na, matchID);
                return match;
            }
            catch(RiotSharpException exp)
            {
                throw new ApplicationException(exp.ToString());
            }
        }

         public string getChampIcon(int id)
        {
            
            var champ = api.GetChampion(Region.na, id);
            return "[](/" + champ.Name + ") " + champ.Name;
        }

        public string getItem(long id)
        {
            var item = api.GetItem(Region.na, Convert.ToInt32(id));
            return item.Name;
        }
    
    }
}
