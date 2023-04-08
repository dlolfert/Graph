using System;

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
        public decimal DailyProfit { get; set; }
        public decimal OnePercent { get; set; }
        public decimal TwoPercent { get; set; }

        public DateTime SellDate { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Profit { get; set; }
        public decimal RunningCost { get; set; }
        public decimal RunningProfit { get; set; }
    }
}
