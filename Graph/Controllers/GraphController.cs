using System.Runtime.InteropServices;
using System.Threading;
using DM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DA;
using System.Collections.Generic;
using System;
using System.Globalization;
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
            var tickerViewModel = new TickerViewModel();
            DayHigh dh = new DayHigh();

            var symbol = Request.QueryString.Value.Split('=')[1];

            try
            {
                //// Get Graph Data...
                dhda.DownloadHistory(symbol);
                dhda.DownloadSummary(symbol);
                
                dh = dhda.GetHeaderInfo(symbol);
                dhda.GetSumGrid(dh, symbol);

                dh.DhArray = dhda.GetDayHighBySymbol(symbol);
                
                tickerViewModel.TickerList = buildSelectDropDownList(symbol);
            }
            catch (Exception e)
            {
                dh.ErrorMessage = e.Message;
            }
            
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

       
    }
}