using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DM
{
    public class Ticker
    {
        [DisplayName("Ticker")]
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }

        public string Rank { get; set; }
    }
}