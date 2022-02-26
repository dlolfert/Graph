using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DA
{
    public class YahooDa : BaseDa
    {
        public bool SymbolDateExist(string symbol, string date)
        {
            bool exists = false;
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(Cs))
                {
                    comm.CommandText =
                        $"Select Count(1) From [ZacksRank] Where [Date] = '{Convert.ToDateTime(date).ToString("yyyy-MM-dd")}' And Symbol = '{symbol}'";
                    comm.Connection = conn;
                    conn.Open();
                    exists = Convert.ToBoolean(comm.ExecuteScalar());
                }
            }

            return exists;
        }

        public List<string> GetDistinctSymbolList()
        {
            var symbols = new List<string>();
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(this.Cs))
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
    }
}