using DM;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Graph.Models
{
    public class YahooViewModel
    {
        public List<SelectListItem> TickerList { get; set; }
        public List<Yahoo> YahooList { get; set; }
        public string ErrorMessage { get; set; }

        public decimal Gains { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get { return Math.Round(Gains - Cost, 2); } }
    }
}


/*
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
 */