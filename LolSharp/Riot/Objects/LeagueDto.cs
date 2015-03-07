using System.Collections.Generic;

namespace LolSharp.Riot.Objects
{
    public class LeagueDto
    {
        public List<LeagueEntryDto> Entries { get; set; }
        public string Name { get; set; }
        public string ParticipantId { get; set; }
        public string Queue { get; set; }
        public string Tier { get; set; }
    }
}
