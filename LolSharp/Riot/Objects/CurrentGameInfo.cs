using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LolSharp.Riot.Objects
{
    public class CurrentGameInfo
    {
        public int GameId { get; set; }
        public int MapId { get; set; }
        public string GameMode { get; set; }
        public string GameType { get; set; }
        public int GameQueueConfigId { get; set; }
        public List<Participant> Participants { get; set; }
        public Observers Observers { get; set; }
        public string PlatformId { get; set; }
        public List<BannedChampion> BannedChampions { get; set; }
        public long GameStartTime { get; set; }
        public int GameLength { get; set; }
    }
}
