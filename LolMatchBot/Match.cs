using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using RiotSharp.MatchEndpoint;

namespace LolMatchBot
{
    class Match
    {
        //Our Riot API key we will be using
        private static string key = "0f8aa821-afe1-4591-a536-af0fb7f910d5";
        //Static instance of the API for champion names, item names, etc.
        StaticRiotApi staticAPI = StaticRiotApi.GetInstance(key);
        //Our API for getting match details
        RiotApi api = RiotApi.GetInstance(key);

        /// <summary>
        /// Gets a match from the specified region with the specified id
        /// </summary>
        /// <param name="matchID"></param>
        /// <param name="region"></param>
        /// <returns>MatchDetail object</returns>
        public MatchDetail getMatch(long matchID, String region)
        {
            
            ///var api = RiotApi.GetInstance(key);
            try
            {
                //Get the region
                Region r = getRegion(region);
                //Get the match
                var match = api.GetMatch(r, matchID);
                //Return the match
                return match;
            }
            catch(RiotSharpException exp)
            {
                //Throw an exception if something went wrong
                throw new ApplicationException(exp.ToString());
            }
        }

        /// <summary>
        /// Gets the region for the matchid
        /// </summary>
        /// <param name="region">String of region (ie. NA, EUW, KR)</param>
        /// <returns>Region object</returns>
        private Region getRegion(string region)
        {
            Region r;
            switch(region)
            {
                case "BR":
                    r = Region.br;
                    break;

                case "EUNE":
                    r = Region.eune;
                    break;

                case "EUW":
                    r = Region.euw;
                    break;

                case "KR":
                    r = Region.kr;
                    break;

                case "LAN":
                    r = Region.lan;
                    break;

                case "LAS":
                    r = Region.las;
                    break;

                case "NA":
                    r = Region.na;
                    break;

                case "OCE":
                    r = Region.oce;
                    break;

                case "RU":
                    r = Region.ru;
                    break;
                    
                case "TR":
                    r = Region.tr;
                    break;

                default:
                    r = Region.na;
                    break;
                
            }

            return r;
        }


        /// <summary>
        /// Gets the associated champ name and icon for the leagueoflegends subreddit
        /// From the passed in ID
        /// </summary>
        /// <param name="id">The ID of the champion</param>
        /// <returns>Formatted string for champion icon + name</returns>
        public string getChampIcon(int id)
        {
            //Get the champion from the static api
            var champ = staticAPI.GetChampion(Region.na, id);
            //Return the champion icon for the league of legends subreddit + the name
            return "[](/" + champ.Name + ") " + champ.Name;
        }

        /// <summary>
        /// Gets the item name for the passed in champion id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getItem(long id)
        {
            //Get the item specified from the static API
            var item = staticAPI.GetItem(Region.na, Convert.ToInt32(id));
            //Return the item name
            //var image = item.Image;
            return item.Name;
        }
    
    }
}
