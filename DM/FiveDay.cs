using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DM
{
    public class FiveDay
    {
        [DisplayName("FiveDay")]
        public string Symbol { get; set; }
        public System.DateTime Monday { get; set; }
        public System.DateTime Friday { get; set; }
        public string Name { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal WeekEndValue { get; set; }
        public decimal MaxValue {  get; set; }
        public string ErrorMessage { get; set; }
        public decimal WeekEndPercent => Math.Round(((((Open * 100) / Close) - 100) * -1), 1);
        public decimal MaxPercent => Math.Round(((((Open * 100) / High) - 100) * -1), 1);

    }
}