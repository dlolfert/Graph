using System.Runtime.InteropServices;
using System.Threading;
using DM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DA;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using Graph.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace Graph.Controllers
{
    public class GraphController : Controller
    {
        
        DayHighDa dhda = new DA.DayHighDa();
        ZacksRankDa zr = new ZacksRankDa();
        private DayRecordDA drda = new DayRecordDA();

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
                dh.DhArray = dhda.GetDayHighBySymbol(symbol);
                
                // Get Trading Data

                List<DayRecord> dayRecords = drda.GetBaseRecords(symbol, 1.0M, 1);
                foreach (var dayRecord in dayRecords)
                {
                    var dr = drda.GetSellValues(dayRecord, 1.0M, 1);
                    dayRecord.SellDate = dr.SellDate;
                    dayRecord.SellPrice = dr.SellPrice;
                    dayRecord.Profit = dr.Profit;
                }

                dayRecords.Sort((x, y) => x.Date.CompareTo(y.Date));
                
                var tempRecords = dayRecords;

                decimal runningCost = 0.0M;
                decimal runningProfit = 0.0M;

                foreach (var dayRecord in dayRecords)
                {
                    var list = from dr in tempRecords where dr.SellDate == dayRecord.Date orderby dr.Date select dr;
                    foreach (var r in list)
                    {
                        runningProfit += r.Profit;
                        runningCost -= r.Open;
                    }

                    runningCost += dayRecord.Open;


                    dayRecord.RunningCost = runningCost;
                    dayRecord.RunningProfit = runningProfit;
                }

                var json = string.Empty;
                json += "[";
                foreach (var dayRecord in dayRecords)
                {
                    json += "[";
                    json += "'" + dayRecord.Date.ToString("yyyy/MM/dd") + "', ";
                    json += Convert.ToString(dayRecord.RunningCost) + ", ";
                    json += Convert.ToString(dayRecord.RunningProfit);
                    json += "],";
                }
                
                if (json.LastIndexOf(',') > 0) json = json.Substring(0, json.LastIndexOf(','));
                json += "]";

                 dh.TradeData = json;
                
                // Get Trading Data

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