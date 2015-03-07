using System.Collections.Generic;

namespace LolSharp.Riot.Objects
{
    public class RuneDtoList
    {
        public BasicDataDto Basic { get; set; }
        public Dictionary<string, RuneDto> Data { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
    }
}
