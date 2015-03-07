using System.Collections.Generic;

namespace LolSharp.Riot.Objects
{
    public class MasteryTreeDto
    {
        public List<MasteryTreeListDto> Defense { get; set; }
        public List<MasteryTreeListDto> Offense { get; set; }
        public List<MasteryTreeListDto> Utility { get; set; } 
    }
}
