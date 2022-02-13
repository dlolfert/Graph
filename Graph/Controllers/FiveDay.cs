using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                slix.Text = String.IsNullOrWhiteSpace(t.Name) ? "": t.Name;
                slix.Value =  String.IsNullOrWhiteSpace(t.Symbol) ? "" : t.Symbol;
                fiveDays.Tickers.Add(slix);
                i++;
                if (i > 5)
                    break;
            }

            return View("Index", fiveDays);
        }

        // GET: FiveDayDA/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FiveDayDA/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FiveDayDA/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FiveDayDA/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FiveDayDA/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FiveDayDA/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FiveDayDA/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
