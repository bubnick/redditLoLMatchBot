using System.Collections.Generic;

namespace LolSharp.Riot.Objects
{
    public class RankedStats
    {
        public long ModifyDate { get; set; }
        public long SummonerId { get; set; }
        public List<ChampionStats> Champions { get; set; } 
    }
}
