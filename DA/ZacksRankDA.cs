using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using DM;

namespace DA
{
    public class ZacksRankDa : BaseDa
    {
        public List<Ticker> GetSymbols()
        {
            List<Ticker> tickers = new List<Ticker>();

            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = Cs;
                    //comm.CommandText = $"SELECT [Symbol], [Name] FROM [Barchart].[dbo].[Top100]";

                    var stmt = "Select Symbol, Name From Top100 Order By Symbol";
                        
                        //"WITH cte AS " +
                        //"( " +
                        //"    SELECT *, " +
                        //"    ROW_NUMBER() OVER(PARTITION BY Symbol ORDER BY Date DESC) AS rn " +
                        //"FROM ZacksRank " +
                        //"), " +
                        //"Name AS(Select[Name], [Symbol] From Top100) " +
                        //"Select CTE.[Symbol], Name.[Name], CTE.[Rank], CTE.[Date] from cte Inner Join Name on CTE.Symbol = Name.Symbol Where rn = 1";

                    


                    comm.CommandText = stmt;

                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader dr = comm.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            var t = new Ticker();
                            t.Symbol = Convert.ToString(dr["Symbol"]);
                            t.Rank = Convert.ToString("F");
                            t.Name = Convert.ToString(dr["Name"]);
                            tickers.Add(t);
                        }
                    }
                }
            }

            return tickers;
        }
        public string GetRankBySymbol(string symbol)
        {
            string json = "[";

            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = Cs;
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
        public string GetLatestRank(string symbol)
        {
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = Cs;
                    comm.CommandText =
                        $"SELECT TOP (1) [Rank], [Date] FROM [Barchart].[dbo].[ZacksRank] Where Symbol = '{symbol}' Order by DATE DESC";
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
