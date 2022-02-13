using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DM
{
    public class FiveDay
    {
        [DisplayName("FiveDay")]
        public string Symbol { get; set; }
        public string Name { get; set; }

        public List<SelectListItem> Tickers { get; set; }
    } 
}