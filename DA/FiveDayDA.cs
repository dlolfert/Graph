using DM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DA
{
    public class FiveDayDa : BaseDa
    {

        public void BuildData(string symbol, int daysBack)
        {
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(this.Cs))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.Add("Symbol", SqlDbType.NChar).Value = symbol;
                    comm.Parameters.Add("DaysBack", SqlDbType.Int).Value = daysBack;
                    comm.CommandText = "[WeeklyHigh]";
                    comm.Connection = conn;
                    conn.Open();

                    comm.ExecuteNonQuery();
                }
            }
        }

        public List<FiveDay> GetFiveDayData(string symbol)
        {
            var records = new List<FiveDay>();

            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(Cs))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.Add("Symbol", SqlDbType.NChar).Value = symbol;
                    comm.CommandText = "[GetTickerDateByWeek]";
                    comm.CommandTimeout = 360;
                    comm.Connection = conn;
                    
                    conn.Open();

                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        var fiveDay = new FiveDay()
                        {
                            Symbol = Convert.ToString(dr["Symbol"]),
                            Monday = Convert.ToDateTime(dr["Monday"]),
                            Friday = Convert.ToDateTime(dr["Friday"]),
                            Open = Convert.ToDecimal(dr["Open"]),
                            Close = Convert.ToDecimal(dr["Close"]),
                            High = Convert.ToDecimal(dr["High"]),
                            WeekEndValue = Convert.ToInt32(dr["WeekEndValue"]),
                            MaxValue = Convert.ToDecimal(dr["MaxValue"])
                        };

                        records.Add(fiveDay);

                    }
                }
            }

            return records;
        }
    }
}