using System.Collections.Generic;

namespace LolSharp.Riot.Objects
{
    public class SummaryStats
    {
        public List<PlayerStatsSummary> PlayerStatSummaries { get; set; }
        public long SummonerId { get; set; }
    }
}
