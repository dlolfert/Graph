using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using DM;

namespace DA
{
    public class DayRecordDA : BaseDa
    {
        public List<DayRecord> GetBaseRecords(string symbol, decimal percent, int minDaysInTrade)
        {
            List<DayRecord> records = new List<DayRecord>();

            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(Cs))
                {
                    comm.CommandText = "SELECT TOP (100) " +
                                       "[Symbol], " +
                                       "[Date], " +
                                       "[DayHigh], " +
                                       "[Open], " +
                                       "[Close], " +
                                       "[DayLow], " +
                                       "[Volume], " +
                                       "CASE ([DayHigh] - [Open]) When 0 Then [Close] - [Open] ELSE [DayHigh] - [Open] END AS Daily_Profit, " +
                                       "([Open] * 1 / 100) AS One_Percent, " +
                                       "([Open] * 2 / 100) AS Two_Percent, " +
                                       "([Open] * 3 / 100) AS Three_Percent, " +
                                       "([Open] * 4 / 100) AS Four_Percent, " +
                                       "([Open] * 5 / 100) AS Five_Percent " +
                                       $"FROM [Barchart].[dbo].[Yahoo] Where Symbol = '{symbol}' Order By DATE DESC";
                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        var dayRecord = new DayRecord
                        {
                            Symbol = Convert.ToString(dr["Symbol"]),
                            Date = Convert.ToDateTime(dr["Date"]),
                            DayHigh = Convert.ToDecimal(dr["DayHigh"]),
                            Open = Convert.ToDecimal(dr["Open"]),
                            Close = Convert.ToDecimal(dr["Close"]),
                            DayLow = Convert.ToDecimal(dr["DayLow"]),
                            Volume = Convert.ToInt32(dr["Volume"]),
                            Daily_Profit = Convert.ToDecimal(dr["Daily_Profit"]),
                            One_Percent = Convert.ToDecimal(dr["One_Percent"]),
                            Two_Percent = Convert.ToDecimal(dr["Two_Percent"])

                        };

                        records.Add(dayRecord);

                    }
                }
            }

            return records;
        }

    }
}