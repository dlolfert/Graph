using DM;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Graph.Models
{
    public class YahooViewModel
    {
        public List<SelectListItem> TickerList { get; set; }
        public List<SelectListItem> PercentIncreaseList { get; set; }
        public List<SelectListItem> HoldDays { get; set; }
        public List<SelectListItem> DumpPercent { get; set; }
        public List<Yahoo> YahooList { get; set; }
        public decimal Gains { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get { return Math.Round(Gains - Cost, 2); } }
        public string ErrorMessage { get; set; }
    }
}