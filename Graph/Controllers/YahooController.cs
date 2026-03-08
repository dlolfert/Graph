using Microsoft.AspNetCore.Mvc;
using DA;
using DM;
using Graph.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Graph.Controllers
{
    public class YahooController : Controller
    {
        ZacksRankDa _zr = new ZacksRankDa();
        TickerDA tickerDA = new TickerDA();
        YahooDa _da = new YahooDa();
        
        YahooViewModel _yahooViewModel = new YahooViewModel();
        
        [Route("Yahoo")]
        public IActionResult Index()
        {
            _yahooViewModel.StartDayList = BuildStartDayOfWeekList(DayOfWeek.Monday);
            _yahooViewModel.TickerList = BuildSelectDropDownList("");
            _yahooViewModel.PercentIncreaseList = BuildSelectPercentIncreaseList("1.0");
            _yahooViewModel.HoldDays = BuildSelectHoldDaysList("5");
            _yahooViewModel.DumpPercent = BuildSelectDumpPercentList(".01");
            
            return View("Index", _yahooViewModel);
        }

        [HttpGet]
        public ViewResult GetYahooData()
        {
            string[] qs = Request.QueryString.Value.Replace("?", "").Split("&");
            var symbol = qs[0].Split('=')[1];
            var DayOfWeek = qs[1].Split('=')[1];
            var PercentIncrease = Convert.ToDecimal(qs[2].Split('=')[1]);
            var HoldDays = Convert.ToInt16(qs[3].Split('=')[1]);
            var DumpPercent = Convert.ToDecimal(qs[4].Split('=')[1]);

            tickerDA.DownloadSummary(symbol);
            tickerDA.DownloadHistory(symbol);

            _yahooViewModel.TickerList = BuildSelectDropDownList(symbol);
            _yahooViewModel.YahooList = _da.GetTradeData(symbol);

            var _OriginalList = _yahooViewModel.YahooList;

            // Build Report
            // 1. Rolup by Week
            _yahooViewModel.YahooList = RollupByWeek(_yahooViewModel.YahooList, DayOfWeek, HoldDays);


            // 2. Find Sell Date and Sell Price
            var x = FindSellDate(_yahooViewModel.YahooList, _OriginalList.OrderBy(ol => ol.TradeDate).ToList(), PercentIncrease, HoldDays, DumpPercent);
            _yahooViewModel.YahooList = x.OrderByDescending(ol => ol.TradeDate).ToList();

            // Summary Data
            _yahooViewModel.Gains = _yahooViewModel.YahooList.Sum(x => x.Gains);
            _yahooViewModel.Cost = _yahooViewModel.YahooList.Sum(x => x.ShareCost);

            // Drop Downlists
            _yahooViewModel.StartDayList = BuildStartDayOfWeekList((DayOfWeek)Enum.Parse(typeof(DayOfWeek), DayOfWeek));
            _yahooViewModel.PercentIncreaseList = BuildSelectPercentIncreaseList(qs[2].Split('=')[1]);
            _yahooViewModel.HoldDays = BuildSelectHoldDaysList(qs[3].Split('=')[1]);
            _yahooViewModel.DumpPercent = BuildSelectDumpPercentList(qs[4].Split('=')[1]);

            return View("Index", _yahooViewModel);
        }


        public List<SelectListItem> BuildStartDayOfWeekList(DayOfWeek SelectedValue)
        {
            var weekdays = Enum.GetValues(
                typeof(DayOfWeek)).Cast<DayOfWeek>()
                .Where(d => d >= DayOfWeek.Monday && d <= DayOfWeek.Friday)
                .Select(d => new SelectListItem { Text = d.ToString(), Value = d.ToString(), Selected = d.ToString() == SelectedValue.ToString()}).ToList();
            
            return weekdays;
        }

        public List<SelectListItem> BuildSelectPercentIncreaseList(string SelectedValue)
        {
            var pi = new List<SelectListItem>();

            decimal x = 0.0m;

            while (x < 20m)
            {
                pi.Add(new SelectListItem 
                { 
                    Text = x.ToString(), 
                    Value = x.ToString(), 
                    Selected = x.ToString().Equals(SelectedValue) 
                });
                
                x = x + .5m;
            }

            return pi;
        }
        public List<SelectListItem> BuildSelectHoldDaysList(string SelectedValue)
        {
            var hd = new List<SelectListItem>();
            
            int x = 1;

            while (x <= 20)
            {
                hd.Add(new SelectListItem 
                { 
                    Text = x.ToString(), 
                    Value = x.ToString(), 
                    Selected = x.ToString().Equals(SelectedValue) 
                });
                
                x++;
            }

            return hd;
        }

        public List<SelectListItem> BuildSelectDumpPercentList(string SelectedValue)
        {
            var dp = new List<SelectListItem>();

            decimal x = 0.0m;

            while (x <= .01m)
            {
                dp.Add(new SelectListItem 
                { 
                    Text = x.ToString(), 
                    Value = x.ToString(),
                    Selected = x.ToString().Equals(SelectedValue)
                });

                x = x + .001m;
            }

            return dp;
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

        private List<Yahoo> FindSellDate(List<Yahoo> yList, List<Yahoo> _OrignalList, decimal PercentIncrease, int HoldDays, decimal DumpPercent)
        {

            foreach(var y in yList)
            {
                Program.WriteLog($"Trade Date : {y.TradeDate.ToString("yyyy-MM-dd")} Date to Sell at cost : {y.TradeDate.AddDays(HoldDays).ToString("yyyy-MM-dd")}");
            }

            List<Yahoo> SellDateList = new List<Yahoo>();

            PercentIncrease = PercentIncrease * .01m;
            foreach (var y in yList)
            {
                var sellPrice = Math.Round((y.Open * PercentIncrease) + y.Open, 2);

                Program.WriteLog($"{ y.TradeDate.ToString("yyyy-MM-dd")} {y.TradeDate.AddDays(HoldDays)}");
                
                var x = _OrignalList.Where(o => o.TradeDate >= y.TradeDate).Where(o => o.TradeDate < y.TradeDate.AddDays(HoldDays)).Where(o => o.DayHigh >= sellPrice).FirstOrDefault();
                if(x != null)
                {
                    y.SellPrice = Math.Round(sellPrice,2);
                    y.SellDate = x.TradeDate;
                    y.PurchaseDate = y.TradeDate;
                    y.PurchasePrice = y.Open;
                }
                else
                {
                    var SellPrice = Math.Round((y.Open * DumpPercent) + y.Open, 2);
                    x = _OrignalList.OrderBy(o => o.TradeDate).Where(o => o.TradeDate >= y.TradeDate.AddDays(HoldDays)).Where(o => o.DayHigh >= SellPrice).FirstOrDefault();
                    if(x != null)
                    {
                        y.SellPrice = SellPrice;
                        y.SellDate = x.TradeDate;
                        y.PurchaseDate = y.TradeDate;
                        y.PurchasePrice = y.Open;
                    }
                }

                if(y.SellDate == DateTime.MinValue)
                {
                    y.ShareCost = y.Open;
                }
                
                SellDateList.Add(y);
            }

            return SellDateList;
        }

        private List<Yahoo> RollupByWeek(List<Yahoo> yList, string dow, int holdDays)
        {
            // find the first monday
            List<Yahoo> byWeekList = new List<Yahoo>();

            var firstDate = yList.OrderBy(y => y.TradeDate).FirstOrDefault().TradeDate;
            var startDate = GetStartDayOfWeek(firstDate, dow);

            while (startDate <= DateTime.Now)
            {
                var endDate = startDate.AddDays(holdDays - 1);
                
                //Program.WriteLog($"Start Date : {startDate.ToString("yyyy-MM-dd")}   End Date : {endDate.ToString("yyyy-MM-dd")} Days Apart : {(endDate - startDate).Days}");

                var week = yList.Where(y => y.TradeDate >= startDate).Where(y => y.TradeDate <= endDate).ToList();

                if (week.Count > 0)
                {
                    Yahoo yahoo = new Yahoo();
                    yahoo.Symbol = week.First().Symbol;
                    yahoo.TradeDate = week.ToList().Last().TradeDate;
                    yahoo.Open = week.ToList().Last().Open;
                    yahoo.DayHigh = week.OrderBy(y => y.DayHigh).Last().DayHigh;
                    yahoo.DayHighDate = week.OrderBy(y => y.DayHigh).Last().TradeDate;

                    byWeekList.Add(yahoo);
                }

                startDate = startDate.AddDays(holdDays);
            }
            
            return [.. byWeekList.OrderByDescending(y => y.TradeDate)];
        }

        private static DateTime GetStartDayOfWeek(DateTime startDate, string dow)
        {
            while(!startDate.DayOfWeek.ToString().Equals(dow))
            {
                startDate = startDate.AddDays(-1);
            }
            return startDate;
        }


        private static DateTime GetMonday(DateTime startDate)
        {
            if (startDate.DayOfWeek == DayOfWeek.Sunday) { return startDate.AddDays(-6); }
            if (startDate.DayOfWeek == DayOfWeek.Monday) { return startDate; }
            if (startDate.DayOfWeek == DayOfWeek.Tuesday) { return startDate.AddDays(-1); }
            if (startDate.DayOfWeek == DayOfWeek.Wednesday) { return startDate.AddDays(-2); }
            if (startDate.DayOfWeek == DayOfWeek.Thursday) { return startDate.AddDays(-3); }
            if (startDate.DayOfWeek == DayOfWeek.Friday) { return startDate.AddDays(-4); }
            if (startDate.DayOfWeek == DayOfWeek.Saturday) { return startDate.AddDays(-5); }
            return startDate;
        }

        private static DateTime GetFriday(DateTime startDate)
        {
            if (startDate.DayOfWeek == DayOfWeek.Sunday) { return startDate.AddDays(-2); }
            if (startDate.DayOfWeek == DayOfWeek.Monday) { return startDate.AddDays(4); }
            if (startDate.DayOfWeek == DayOfWeek.Tuesday) { return startDate.AddDays(3); }
            if (startDate.DayOfWeek == DayOfWeek.Wednesday) { return startDate.AddDays(2); }
            if (startDate.DayOfWeek == DayOfWeek.Thursday) { return startDate.AddDays(1); }
            if (startDate.DayOfWeek == DayOfWeek.Friday) { return startDate; }
            if (startDate.DayOfWeek == DayOfWeek.Saturday) { return startDate.AddDays(-1); }
            return startDate;
        }

    }
}