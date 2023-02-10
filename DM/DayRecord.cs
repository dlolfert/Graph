using System;
using System.Collections.Generic;
using System.Text;

namespace DM
{
    public class DayRecord
    {
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public decimal DayHigh { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal DayLow { get; set; }
        public int Volume { get; set; }
        public decimal Daily_Profit { get; set; }
        public decimal One_Percent { get; set; }
        public decimal Two_Percent { get; set; }

        public DateTime SellDate { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Profit { get; set; }
        public decimal RunningCost { get; set; }
        public decimal RunningProfit { get; set; }
    }
}
