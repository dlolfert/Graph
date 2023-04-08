using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DA;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Graph.Controllers
{
    public class FiveDay : Controller
    {
        // GET: FiveDayDA
        public ActionResult Index()
        {
            DM.FiveDay fiveDays = new DM.FiveDay();
            fiveDays.Tickers = new List<SelectListItem>();
            
            ZacksRankDa zrda = new ZacksRankDa();
            var symbols = zrda.GetSymbols();
            int i = 0;
            foreach (var t in symbols)
            {
                var slix = new SelectListItem();
                slix.Text = string.IsNullOrWhiteSpace(t.Name) ? "": t.Name;
                slix.Value =  string.IsNullOrWhiteSpace(t.Symbol) ? "" : t.Symbol;
                fiveDays.Tickers.Add(slix);
                i++;
                if (i > 5)
                    break;
            }

            return View("Index", fiveDays);
        }
    }
}