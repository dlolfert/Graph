using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Graph.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remotion.Linq.Clauses;

namespace Graph.Controllers
{
    public class GraphController : Controller
    {
        // GET: Graph
        public ActionResult Index()
        {
            return View();
        }

        [Route("Graph/Ticker")]
        public ActionResult Ticker()
        {
            return View("Ticker", GetSymbols());
        }
        // GET: Graph/Details/5
        [Route("Graph/Details/{symbol}")]
        public ActionResult Details(string symbol)
        {
            return View("Details", GetRankBySymbol(symbol));
        }

        [Route("Graph/DayHigh/{symbol}")]
        public ActionResult DayHigh(string symbol)
        {
            DayHigh dh = GetHeaderInfo(symbol);

            //string average = GetDayHighAverage(symbol);
            dh.DHArray = GetDayHighBySymbol(symbol);
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

        private string GetRankBySymbol(string symbol)
        {
            string json = "[";

            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=NIXON,1466;Database=Barchart;User Id=sa;Password=@a88word";
                    comm.CommandText = $"SELECT (Rank * -1) AS Rank, " +
                        "CASE IsNull(Momentum, 'F') " +
                        "WHEN 'F' THEN '1' " +
                        "WHEN 'D' THEN '2' " +
                        "WHEN 'C' THEN '3' " +
                        "WHEN 'B' THEN '4' " +
                        "WHEN 'A' THEN '5' " +
                        "END " +
                        "AS Momentum, "
                        + $" [Date] FROM [ZacksRank] Where Symbol = '{symbol}' Order By Date Desc";
                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader dr = comm.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            json +=
                                $"['{Convert.ToDateTime(dr["Date"]).ToString("yyyy/MM/dd")}',{Convert.ToString(dr["Rank"])},{Convert.ToString(dr["Momentum"])}],";
                        }
                    }
                }
            }

            json = json.Substring(0, json.LastIndexOf(','));
            return json += "]";
        }
        //[GetHeaderInfo]
        private DayHigh GetHeaderInfo(string symbol)
        {
            DayHigh dh = new DayHigh();
                        
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=NIXON,1466;Database=Barchart;User Id=sa;Password=@a88word";
                    comm.CommandText = "GetHeaderInfo";
                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("Symbol", symbol);
                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader dr = comm.ExecuteReader();
                    dr.Read();

                    dh.symbol = Convert.ToString(dr["Symbol"]);
                    dh.name = Convert.ToString(dr["Name"]);
                    dh.average = Convert.ToString(dr["Average"]);
                    dh.DaysAboveAvg = Convert.ToString(dr["DaysAboveAvg"]);
                    dh.PercentDaysAboveAvg = Convert.ToString(dr["% Days Above Average"]);
                    dh.DaysCloseAboveOpen = Convert.ToString(dr["DaysCloseAboveOpen"]);
                    dh.PercentDaysCloseAboveOpen = Convert.ToString(dr["% Day Close Above Open"]);
                    dh.DaysHighAboveOpen = Convert.ToString(dr["DaysHighAboveOpen"]);
                    dh.PercentHighAboveOpen = Convert.ToString(dr["% Day High Above Open"]);
                    dh.StdDev = Convert.ToString(dr["StdDev"]);
                    dh.records = Convert.ToString(dr["Records"]);
                }
            }
            return dh;
            //Symbol Name    Average DaysAboveAvg    Total AdjustedTotal   DaysCloseAboveOpen DaysHighAboveOpen	% Day High Above Open   StdDev Records
        }
        private string GetDayHighAverage(string symbol)
        {
            string average = string.Empty;
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=NIXON,1466;Database=Barchart;User Id=sa;Password=@a88word";
                    comm.CommandText = "SELECT " +
                        "AVG(DayHigh - [Open]) " +
                        "FROM[Barchart].[dbo].[ZacksRank] " +
                        $"Where Symbol = '{symbol}' And [DATE] >= DATEADD(DAY, -100, [Date])";
                    comm.Connection = conn;
                    conn.Open();

                    average = Convert.ToString(comm.ExecuteScalar());
                }
            }
            try
            {
                return Convert.ToDecimal(average).ToString("#.##");
            }
            catch(Exception ex)
            {
                return ".00";
            }
        }
        private string GetDayHighBySymbol(string symbol)
        {
            string json = "[";

            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=NIXON,1466;Database=Barchart;User Id=sa;Password=@a88word";
                    comm.CommandText = "SELECT TOP 100 " +
                        "[Date], " +
                        "CASE  " +
                        "WHEN ([DayHigh] - [Open]) > 0 THEN ([DayHigh] - [Open]) " +
                        "ELSE '0.00' " +
                        "END AS DayHigh, " +
                        "CASE " +
                        "WHEN ([DayHigh] - [Open]) <= 0 THEN ([Open] - [Close]) " +
                        "ELSE '0.00' " +
                        "END AS DayLow " +
                        $"FROM [Barchart].[dbo].[ZacksRank] Where Symbol = '{symbol}' Order By Date DESC";
                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader dr = comm.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            json +=
                                $"['{Convert.ToDateTime(dr["Date"]).ToString("yyyy/MM/dd")}',{Convert.ToString(dr["DayHigh"])},{Convert.ToString(dr["DayLow"])}],";
                        }
                    }
                }
            }
            if(json.LastIndexOf(',') > 0) json = json.Substring(0, json.LastIndexOf(','));
            return json += "]";
        }

        private List<Ticker> GetSymbols()
        {
            List<Ticker> tickers = new List<Ticker>();

            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=NIXON,1466;Database=Barchart;User Id=sa;Password=@a88word";
                    //comm.CommandText = $"SELECT [Symbol], [Name] FROM [Barchart].[dbo].[Top100]";

                    var stmt =
                        //"WITH cte AS " +
                        //   "( " +
                        //   "SELECT *, " +
                        //   "ROW_NUMBER() OVER (PARTITION BY Symbol ORDER BY Date DESC) AS rn " +
                        //   "FROM ZacksRank " +
                        //   ") Select * from cte where rn = 1";

                        "WITH cte AS " +
                        "( " +
                        "    SELECT *, " +
                        "    ROW_NUMBER() OVER(PARTITION BY Symbol ORDER BY Date DESC) AS rn " +
                        "FROM ZacksRank " +
                        "), " +
                        "Name AS(Select[Name], [Symbol] From Top100) " +
                        "Select CTE.[Symbol], Name.[Name], CTE.[Rank], CTE.[Date] from cte Inner Join Name on CTE.Symbol = Name.Symbol Where rn = 1";




                    comm.CommandText = stmt;

                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader dr = comm.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            var t = new Ticker();
                            t.Name = Convert.ToString(dr["Symbol"]);
                            t.Symbol = Convert.ToString(dr["Symbol"]);
                            t.Rank = Convert.ToString(dr["Rank"]);
                            t.Name = Convert.ToString(dr["Name"]);
                            tickers.Add(t);
                        }
                    }
                }
            }

            return tickers;
        }

        private string GetLatestRank(string symbol)
        {
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=NIXON,1466;Database=Barchart;User Id=sa;Password=@a88word";
                    comm.CommandText = $"SELECT TOP (1) [Rank], [Date] FROM [Barchart].[dbo].[ZacksRank] Where Symbol = '{symbol}' Order by DATE DESC";
                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader dr = comm.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            return Convert.ToString(dr["Rank"]);
                        }
                    }
                }
            }

            return "6";
        }
    }
    public class DayHigh
    {
        //Symbol Name    Average DaysAboveAvg    Total AdjustedTotal   DaysCloseAboveOpen DaysHighAboveOpen	% Day High Above Open   StdDev Records
//USO NULL	0.14	21	9.22	5.74	29	66	97	0.18	68

        public string symbol { get; set; }
        public string name { get; set; }
        public string average { get; set; }
        public string DaysAboveAvg { get; set; }
        public string PercentDaysAboveAvg { get; set; }
        public string DaysCloseAboveOpen { get; set; }
        public string PercentDaysCloseAboveOpen { get; set; }
        public string DaysHighAboveOpen { get; set; }
        public string PercentHighAboveOpen { get; set; }
        public string StdDev { get; set; }
        public string records { get; set; }
        public string DHArray { get; set; }
    }
}