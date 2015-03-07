using System.Collections.Generic;

namespace LolSharp.Riot.Objects
{
    public class SummonerSpellDtoList
    {
        public Dictionary<string, SummonerSpellDto> Data { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
    }
}
