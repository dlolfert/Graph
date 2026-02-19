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
        
        [Route("Yahoo")]
        public IActionResult Index()
        {
            return View("Index", new Yahoo());
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