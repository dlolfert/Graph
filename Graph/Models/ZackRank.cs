using System.ComponentModel;

namespace Graph.Models
{
    public class ZackRank
    {
        [DisplayName("Not Symbol")]
        public string Symbol { get; set; }
        public string Rank { get; set; }
        public string RankDate { get; set; }
    }
}