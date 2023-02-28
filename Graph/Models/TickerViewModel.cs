using System.Collections.Generic;
using DM;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Graph.Models
{
    public class TickerViewModel
    {
        public List<SelectListItem> TickerList { get; set; }
        public List<SelectListItem> PercentList { get; set; }
        public List<SelectListItem> DaysInTradeList { get; set; }
        public DayHigh GraphData { get; set; }
    }
}
