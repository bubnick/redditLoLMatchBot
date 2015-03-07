using System.Collections.Generic;

namespace LolSharp.Riot.Objects
{
    public class Timeline
    {
        public long FrameInterval { get; set; }
        public List<Frame> Frames { get; set; } 
    }
}
