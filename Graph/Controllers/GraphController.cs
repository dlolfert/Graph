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
            //var md = new List<ZackRank>();
            
            //md.Add( new ZackRank() {Symbol = "alsn", Rank = "1", RankDate = "2018-01-01"});
            //md.Add(new ZackRank() { Symbol = "alsn", Rank = "1", RankDate = "2018-01-02" });
            //md.Add(new ZackRank() { Symbol = "alsn", Rank = "2", RankDate = "2018-01-03" });
            //md.Add(new ZackRank() { Symbol = "alsn", Rank = "2", RankDate = "2018-01-04" });
            //md.Add(new ZackRank() { Symbol = "alsn", Rank = "2", RankDate = "2018-01-05" });
            //md.Add(new ZackRank() { Symbol = "alsn", Rank = "3", RankDate = "2018-01-06" });
            //md.Add(new ZackRank() { Symbol = "alsn", Rank = "2", RankDate = "2018-01-07" });
            //md.Add(new ZackRank() { Symbol = "alsn", Rank = "2", RankDate = "2018-01-08" });
            //md.Add(new ZackRank() { Symbol = "alsn", Rank = "1", RankDate = "2018-01-09" });
            //md.Add(new ZackRank() { Symbol = "alsn", Rank = "1", RankDate = "2018-01-10" });

            //var json = "";
            //json += "[";
            //json += "['2018-01-01', -1],";
            //json += "['2018-01-02', -1],";
            //json += "['2018-01-03', -2],";
            //json += "['2018-01-04', -2],";
            //json += "['2018-01-05', -2],";
            //json += "['2018-01-06', -3],";
            //json += "['2018-01-07', -3],";
            //json += "['2018-01-08', -2],";
            //json += "['2018-01-09', -1],";
            //json += "['2018-01-10', -1]";
            //json += "]";

            ////return View();
            return View("Details", GetRankBySymbol(symbol));
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
                    conn.ConnectionString = "Server=10.0.0.220,1466;Database=Barchart;User Id=sa;Password=@a88word";
                    comm.CommandText =$"SELECT (Rank * -1) AS Rank, [Date] FROM [ZacksRank] Where Symbol = '{symbol}' Order By Date Desc";
                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader dr = comm.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            json +=
                                $"['{Convert.ToDateTime(dr["Date"]).ToString("yyyy/MM/dd")}',{Convert.ToString(dr["Rank"])}],";
                        }
                    }
                }
            }

            json = json.Substring(0, json.LastIndexOf(','));
            return json += "]";
        }
        private List<Ticker> GetSymbols()
        {
            List<Ticker> tickers = new List<Ticker>();

            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=10.0.0.220,1466;Database=Barchart;User Id=sa;Password=@a88word";
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
                    conn.ConnectionString = "Server=10.0.0.220,1466;Database=Barchart;User Id=sa;Password=@a88word";
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
}