using System.ComponentModel;
using System;

namespace DM
{
    public class Yahoo
    {
        [DisplayName("Yahoo")]
        public string Symbol { get; set; }
        public DateTime TradeDate { get; set;  }
        public decimal Open { get; set; }
        public decimal DayHigh { get; set; }
    }
}