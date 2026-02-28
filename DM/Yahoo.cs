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
        public DateTime DayHighDate { get; set; }
        public decimal DayHighPercent => Math.Round((1 - (Open / DayHigh)) * 100, 1);

        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get;set; }
        public decimal SellPrice { get; set; }
        public DateTime SellDate { get; set; }

        public decimal ShareCost { get; set; }
        public decimal ShareValue { get; set; }
        public decimal ShareProfitLoss => ShareValue - ShareCost;

        public decimal Gains { 
            get {
                    if(SellPrice > 0) {
                        return Math.Round(SellPrice - PurchasePrice, 2);
                    }
                    else {
                        return 0;
                    }
            } 
        }
        public decimal TotalProfitLoss => ShareProfitLoss + Gains;
    }
}