using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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