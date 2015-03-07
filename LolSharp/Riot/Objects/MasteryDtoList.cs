using System.Collections.Generic;

namespace LolSharp.Riot.Objects
{
    public class MasteryDtoList
    {
        public Dictionary<string, MasteryDto> Data { get; set; }
        public MasteryTreeDto Tree { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
    }
}
