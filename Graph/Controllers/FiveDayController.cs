using DA;
using DM;
using Graph.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Graph.Controllers
{
    public class FiveDayController : Controller
    {
        ZacksRankDa _zr = new ZacksRankDa();
        
        // GET: FiveDayDA
        [Route("{symbol}")]
        public ActionResult Index(string symbol)
        {
            FiveDayViewModel fdtvm = new FiveDayViewModel();
            
            fdtvm.TickerList = BuildSelectDropDownList(symbol);
                       
            return View("Index", fdtvm);
        }

        [HttpGet]
        public ViewResult GetFiveDayData()
        {
            var fiveDayViewModel = new FiveDayViewModel();
            try
            {
                var fdda = new FiveDayDa();

                if (Request.QueryString.Value != null)
                {
                    string[] qs = Request.QueryString.Value.Replace("?", "").Split("&");
                    var symbol = qs[0].Split('=')[1];

                    TickerDA tickerDA = new TickerDA();
                    tickerDA.DownloadSummary(symbol);
                    tickerDA.DownloadHistory(symbol);

                    fiveDayViewModel.TickerList = BuildSelectDropDownList(symbol);

                    fiveDayViewModel.FiveDayList = fdda.GetFiveDayData(symbol);
                    
                    string json = "[";
                    foreach (var fd in fiveDayViewModel.FiveDayList.Take(60))
                    {
                        json += "['" + fd.Monday.ToString("yyyy/MM/dd") + "', " + fd.WeekEndValue + ", " + fd.MaxValue + "],";
                    }
                    if (json.LastIndexOf(',') > 0) json = json.Substring(0, json.LastIndexOf(','));
                    json += "]";

                    fiveDayViewModel.WeekArray = json;
                }
            }
            catch (Exception e)
            {
                fiveDayViewModel.ErrorMessage = e.Message;
            }
            
            return View("Index", fiveDayViewModel);
        }
        private List<SelectListItem> BuildSelectDropDownList(string selectedSymbol)
        {
            var selectList = new List<SelectListItem>();

            var tickers = _zr.GetSymbols();
            foreach (var t in tickers)
            {
                var selectListItem = new SelectListItem();
                var name = t.Name;
                var symbol = t.Symbol;

                selectListItem.Text = $"{name} ({symbol})";
                selectListItem.Value = symbol;
                if (symbol.ToUpper().Equals(selectedSymbol.ToUpper()))
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }

            return selectList;
        }
    }
}