using DM;
using Microsoft.AspNetCore.Mvc;
using DA;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using Graph.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Graph.Controllers
{
    public class GraphController : Controller
    {
        
        DayHighDa _dhda = new DA.DayHighDa();
        ZacksRankDa _zr = new ZacksRankDa();
        private DayRecordDa _drda = new DayRecordDa();

        // GET: Graph
        public ActionResult Index()
        {
            return View();
        }

        [Route("Graph/Ticker")]
        public ActionResult Ticker()
        {
            var tickerViewModel = new TickerViewModel();
            tickerViewModel.TickerList = BuildSelectDropDownList("");
            tickerViewModel.DaysInTradeList = BuildDaysInTradeDropDownList("0");
            tickerViewModel.PercentList = BuildPercentList(.5M);

            tickerViewModel.GraphData = new DayHigh();
            return View("Ticker", tickerViewModel);
        }

        [Route("Graph/DayHigh/{symbol}")]
        public ActionResult DayHigh(string symbol)
        {
            return View("DayHigh", new DM.DayHigh());
        }

        [HttpGet]
        public ViewResult GetTickerData()
        {
            var tickerViewModel = new TickerViewModel();
            DayHigh dh = new DayHigh();

            if (Request.QueryString.Value != null)
            {
                string[] qs = Request.QueryString.Value.Replace("?", "").Split("&");

                var symbol = qs[0].Split('=')[1];
                var daysInTrade = qs[1].Split('=')[1];
                decimal percent = Convert.ToDecimal(qs[2].Split('=')[1]);

                try
                {
                    //// Get Graph Data...
                    _dhda.DownloadHistory(symbol);
                    _dhda.DownloadSummary(symbol);
                
                    dh = _dhda.GetHeaderInfo(symbol);
                    dh.DhArray = _dhda.GetDayHighBySymbol(symbol);
                
                    // Get Trading Data

                    List<DayRecord> dayRecords = _drda.GetBaseRecords(symbol, percent, Convert.ToInt32(daysInTrade));

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
                        json += Convert.ToString(dayRecord.RunningCost, CultureInfo.InvariantCulture) + ", ";
                        json += Convert.ToString(dayRecord.RunningProfit, CultureInfo.InvariantCulture);
                        json += "],";
                    }
                
                    if (json.LastIndexOf(',') > 0) json = json.Substring(0, json.LastIndexOf(','));
                    json += "]";

                    dh.TradeData = json;
                
                    // Get Trading Data

                    tickerViewModel.TickerList = BuildSelectDropDownList(symbol);
                    tickerViewModel.DaysInTradeList = BuildDaysInTradeDropDownList(daysInTrade);
                    tickerViewModel.PercentList = BuildPercentList(percent);
                }
                catch (Exception e)
                {
                    dh.ErrorMessage = e.Message;
                }
            }

            tickerViewModel.GraphData = dh;

            return View("Ticker", tickerViewModel);
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

        private List<SelectListItem> BuildDaysInTradeDropDownList(string selectedDaysInTrade)
        {
            var selectList = new List<SelectListItem>();
            
            for (int i = 0; i <= 20; i++)
            {
                selectList.Add(new SelectListItem { Value = Convert.ToString(i), Text = Convert.ToString(i) });
                if (Convert.ToInt32(selectedDaysInTrade).Equals(i))
                {
                    selectList.Last().Selected = true;
                }
            }

            return selectList;
        }

        private List<SelectListItem> BuildPercentList(decimal selectedPercent)
        {
            var selectList = new List<SelectListItem>();
            for (decimal i = .5M; i <= 20M; i += .5M)
            {
                selectList.Add(new SelectListItem { Value = Convert.ToString(i, CultureInfo.InvariantCulture), Text = Convert.ToString(i, CultureInfo.InvariantCulture) });
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
            return View("Details", _zr.GetRankBySymbol(symbol));
        }

       
    }
}