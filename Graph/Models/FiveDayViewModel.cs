using DM;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Graph.Models
{
    public class FiveDayViewModel
    {        
        public List<SelectListItem> TickerList { get; set; }
        public List<FiveDay> FiveDayList { get; set; }
        public string ErrorMessage { get; set; }
    }
}