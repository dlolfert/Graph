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
            List<string> symbols = new List<string>();
            var dr = ExecuteReader("Select Distinct Symbol From ZacksRank");
            while (dr.Read())
            {
                symbols.Add(dr["Symbol"].ToString());
            }

            return symbols;
        }
    }
}