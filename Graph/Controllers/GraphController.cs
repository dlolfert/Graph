using System.Runtime.InteropServices;
using System.Threading;
using DM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DA;
using System.Collections.Generic;
using System;
using Graph.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Graph.Controllers
{
    public class GraphController : Controller
    {
        
        DayHighDa dhda = new DA.DayHighDa();
        ZacksRankDa zr = new ZacksRankDa();

        // GET: Graph
        public ActionResult Index()
        {
            return View();
        }

        [Route("Graph/Ticker")]
        public ActionResult Ticker()
        {
            var tickerViewModel = new TickerViewModel();
            tickerViewModel.TickerList = buildSelectDropDownList("");
            tickerViewModel.GraphData = new DayHigh();
            return View("Ticker", tickerViewModel);
        }

        [HttpGet]
        public ViewResult GetTickerData()
        {
            var symbol = Request.QueryString.Value.Split('=')[1];

            // Get Graph Data...
            dhda.DownloadHistory(symbol);
            
            dhda.DownloadSummary(symbol);


            DayHigh dh = dhda.GetHeaderInfo(symbol);
            dhda.GetSumGrid(dh, symbol);

            dh.DhArray = dhda.GetDayHighBySymbol(symbol);

            var tickerViewModel = new TickerViewModel();
            tickerViewModel.TickerList = buildSelectDropDownList(symbol);
            tickerViewModel.GraphData = dh;

            return View("Ticker", tickerViewModel);
        }

        private List<SelectListItem> buildSelectDropDownList(string SelectedSymbol)
        {
            var selectList = new List<SelectListItem>();

            var tickers = zr.GetSymbols();
            foreach (var t in tickers)
            {
                var selectListItem = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                var name = t.Name;
                var symbol = t.Symbol;

                selectListItem.Text = $"{name} ({symbol})";
                selectListItem.Value = symbol;
                if (symbol.ToUpper().Equals(SelectedSymbol.ToUpper()))
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }

            return selectList;
        }


        // GET: Graph/Details/5
        [Route("Graph/Details/{symbol}")]
        public ActionResult Details(string symbol)
        {
            return View("Details", zr.GetRankBySymbol(symbol));
        }

        [Route("Graph/DayHigh/{symbol}")]
        public ActionResult DayHigh(string symbol)
        {
            symbol = symbol.ToUpper();

            dhda.DownloadHistory(symbol);
            
            DayHigh dh = dhda.GetHeaderInfo(symbol);
            dhda.GetSumGrid(dh, symbol);
            
            dh.DhArray = dhda.GetDayHighBySymbol(symbol);
            
            return View("DayHigh", dh);
        }

        // GET: Graph/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: Graph/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Graph/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        // POST: Graph/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Graph/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: Graph/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}