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
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

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
            tickerViewModel.DaysInTradeList = buildDaysInTradeDropDownList("0");
            tickerViewModel.PercentList = buildPercentList(.5M);

            tickerViewModel.GraphData = new DayHigh();
            return View("Ticker", tickerViewModel);
        }

        [HttpGet]
        public ViewResult GetTickerData()
        {
            var tickerViewModel = new TickerViewModel();
            DayHigh dh = new DayHigh();

            string[] qs = Request.QueryString.Value.Replace("?", "").Split("&");

            var symbol = qs[0].Split('=')[1];
            var daysInTrade = qs[1].Split('=')[1];
            decimal percent = Convert.ToDecimal(qs[2].Split('=')[1]);

            try
            {
                //// Get Graph Data...
                dhda.DownloadHistory(symbol);
                dhda.DownloadSummary(symbol);
                
                dh = dhda.GetHeaderInfo(symbol);
                dh.DhArray = dhda.GetDayHighBySymbol(symbol);
                
                // Get Trading Data

                List<DayRecord> dayRecords = drda.GetBaseRecords(symbol, percent, Convert.ToInt32(daysInTrade));

                dayRecords.Sort((x, y) => x.Date.CompareTo(y.Date));

                var tempRecords = dayRecords;

                foreach (var dayRecord in dayRecords)
                {
                    var sellRecord = (from dayrec in tempRecords 
                       where dayrec.Symbol == dayRecord.Symbol &&
                             dayrec.DayHigh >= dayRecord.Open + (dayRecord.Open * (percent / 100)) &&
                             dayrec.Date >= dayRecord.Date.AddDays(Convert.ToInt32(daysInTrade)) orderby dayrec.Date select dayrec).FirstOrDefault();

                   if (sellRecord != null)
                   {
                       dayRecord.SellDate = sellRecord.Date;
                       dayRecord.SellPrice = dayRecord.Open + (dayRecord.Open * (percent / 100));
                       dayRecord.Profit = dayRecord.SellPrice - dayRecord.Open;
                   }
                }
                
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
                tickerViewModel.DaysInTradeList = buildDaysInTradeDropDownList(daysInTrade);
                tickerViewModel.PercentList = buildPercentList(percent);
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

        private List<SelectListItem> buildDaysInTradeDropDownList(string SelectedDaysInTrade)
        {
            var selectList = new List<SelectListItem>();
            
            for (int i = 0; i <= 20; i++)
            {
                selectList.Add(new SelectListItem { Value = Convert.ToString(i), Text = Convert.ToString(i) });
                if (Convert.ToInt32(SelectedDaysInTrade).Equals(i))
                {
                    selectList.Last().Selected = true;
                }
            }

            return selectList;
        }

        private List<SelectListItem> buildPercentList(decimal selectedPercent)
        {
            var selectList = new List<SelectListItem>();
            for (decimal i = .5M; i <= 20M; i += .5M)
            {
                selectList.Add(new SelectListItem { Value = Convert.ToString(i), Text = Convert.ToString(i) });
                if (selectedPercent.Equals(i))
                {
                    selectList.Last().Selected = true;
                }
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