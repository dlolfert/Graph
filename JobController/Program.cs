using DA;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace JobController
{
    class Program
    {
        static void Main(string[] args)
        {
            var tickerDA = new TickerDA();

            var symbols = GetDistinctSymbolList();

            foreach (var symbol in symbols)
            {
                Console.WriteLine($"Downloading History for : {symbol}");
                tickerDA.DownloadHistory(symbol);
            }
        }
        
        private static List<string> GetDistinctSymbolList()
        {
            var symbols = new List<string>();
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(""))
                {
                    comm.CommandText = "SELECT Distinct Symbol FROM [Barchart].[dbo].[ZacksRank]";
                    comm.Connection = conn;
                    conn.Open();

                    var dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        symbols.Add(Convert.ToString(dr["Symbol"]));
                    }

                }
            }

            return symbols;
        }

        //private static bool SymbolDateExist(string symbol, string date)
        //{
        //    bool exists = false;
        //    using (SqlCommand comm = new SqlCommand())
        //    {
        //        using (SqlConnection conn = new SqlConnection(Cs))
        //        {
        //            comm.CommandText =
        //                $"Select Count(1) From [ZacksRank] Where [Date] = '{Convert.ToDateTime(date).ToString("yyyy-MM-dd")}' And Symbol = '{symbol}'";
        //            comm.Connection = conn;
        //            conn.Open();
        //            exists = Convert.ToBoolean(comm.ExecuteScalar());
        //        }
        //    }

        //    return exists;
        //}
    }
}