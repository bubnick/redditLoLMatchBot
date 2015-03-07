using System.Collections.Generic;

namespace LolSharp.Riot.Objects
{
    public class Frame
    {
        public long Timestamp { get; set; }
        public List<Event> Events { get; set; }
        public Dictionary<string, ParticipantFrame> ParticipantFrames { get; set; }
    }
}
