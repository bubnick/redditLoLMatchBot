using System.Collections.Generic;

namespace LolSharp.Riot.Objects
{
    public class GameHistory
    {
        public List<Game> Games { get; set; }
        public long SummonerId { get; set; }
    }
}
