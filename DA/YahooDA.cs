using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DM;

namespace DA
{
    public class YahooDa : BaseDa
    {
        public  List<Yahoo> GetTradeData(string symbol,int days = 200)
        {
            var td = new List<Yahoo>();

            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(Cs))
                {
                    comm.CommandText = $"Select Top {days} [Date], [Open], DayHigh From dbo.Yahoo Where Symbol = '{symbol}' Order By [Date]";
                    comm.Connection = conn;

                    conn.Open();
                    var dr = comm.ExecuteReader();

                    while (dr.Read())
                    {
                        var y = new Yahoo();

                        y.Symbol = symbol;
                        y.TradeDate = Convert.ToDateTime(dr["[Date]"]);
                        y.Open = Convert.ToDecimal(dr["[Open]"]);
                        y.DayHigh = Convert.ToDecimal(dr["DayHigh"]);

                        td.Add(y);
                    }
                }
            }

            return td;
        }
    }
}