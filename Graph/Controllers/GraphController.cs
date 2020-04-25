using Graph.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using Graph.Models;
using DM;
using DA;


namespace Graph.Controllers
{
    public class GraphController : Controller
    {
        DayHighDA dhda = new DayHighDA();
        ZacksRankDA zr = new ZacksRankDA();

        // GET: Graph
        public ActionResult Index()
        {
            return View();
        }

        [Route("Graph/Ticker")]
        public ActionResult Ticker()
        {
            return View("Ticker", zr.GetSymbols());
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
            dhda.DownloadHistory(symbol);
            DayHigh dh = dhda.GetHeaderInfo(symbol);
            dhda.GetSumGrid(dh, symbol);
            //string average = GetDayHighAverage(symbol);
            dh.DHArray = dhda.GetDayHighBySymbol(symbol);
            return View("DayHigh", dh);
            //return View("DayHigh", GetDayHighBySymbol(symbol) + "***" + average);
        }

        // GET: Graph/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Graph/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Graph/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Graph/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Graph/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Graph/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}