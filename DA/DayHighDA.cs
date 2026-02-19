using DM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
//using Newtonsoft.Json;
using System.Text.Json;


namespace DA
{

    public class DayHighDa : BaseDa
    {
        public static DateTime LastPeriodUpdateTime = DateTime.Now.AddDays(-10);

        public DayHigh GetHeaderInfo(string symbol)
        {
            DayHigh dh = new DayHigh();

            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = Cs;
                    comm.CommandText = "GetHeaderInfo";
                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("Symbol", symbol);
                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader dr = comm.ExecuteReader();
                    dr.Read();

                    dh.Symbol = Convert.ToString(dr["Symbol"]);
                    dh.Name = Convert.ToString(dr["Name"]);
                    dh.Average = Convert.ToString(dr["Average"]);
                    dh.DaysAboveAvg = Convert.ToString(dr["DaysAboveAvg"]);
                    dh.PercentDaysAboveAvg = Convert.ToString(dr["% Days Above Average"]);
                    dh.DaysCloseAboveOpen = Convert.ToString(dr["DaysCloseAboveOpen"]);
                    dh.PercentDaysCloseAboveOpen = Convert.ToString(dr["% Day Close Above Open"]);
                    dh.DaysHighAboveOpen = Convert.ToString(dr["DaysHighAboveOpen"]);
                    dh.PercentHighAboveOpen = Convert.ToString(dr["% Day High Above Open"]);
                    dh.StdDev = Convert.ToString(dr["StdDev"]);
                    dh.Records = Convert.ToString(dr["Records"]);
                    dh.LastClose = Convert.ToString(dr["LastClose"]);
                    //Console.WriteLine($"{n:#,##0.0K}");
                    dh.AvgVolume = Convert.ToString($"{dr["avgVolume"]:###,###,###,###}");
                }
            }

            return dh;
            //Symbol Name    Average DaysAboveAvg    Total AdjustedTotal   DaysCloseAboveOpen DaysHighAboveOpen	% Day High Above Open   StdDev Records
        }

        //public DayHigh GetSumGrid(DayHigh dh, string symbol)
        //{
        //    using (SqlCommand comm = new SqlCommand())
        //    {
        //        using (SqlConnection conn = new SqlConnection())
        //        {
        //            conn.ConnectionString = Cs;
        //            comm.CommandText = "SumGrid";
        //            comm.CommandType = System.Data.CommandType.StoredProcedure;
        //            comm.Parameters.AddWithValue("Symbol", symbol);
        //            comm.Connection = conn;
        //            conn.Open();

        //            SqlDataReader dr = comm.ExecuteReader();
        //            dr.Read();

        //            dh.V100 = Convert.ToString(dr["T100"]);
        //            dh.V90 = Convert.ToString(dr["T90"]);
        //            dh.V80 = Convert.ToString(dr["T80"]);
        //            dh.V70 = Convert.ToString(dr["T70"]);
        //            dh.V60 = Convert.ToString(dr["T60"]);
        //            dh.V50 = Convert.ToString(dr["T50"]);
        //            dh.V40 = Convert.ToString(dr["T40"]);
        //            dh.V30 = Convert.ToString(dr["T30"]);
        //            dh.V20 = Convert.ToString(dr["T20"]);
        //            dh.V10 = Convert.ToString(dr["T10"]);
        //        }
        //    }

        //    return dh;
        //    //Symbol Name    Average DaysAboveAvg    Total AdjustedTotal   DaysCloseAboveOpen DaysHighAboveOpen	% Day High Above Open   StdDev Records
        //}

        public string GetDayHighAverage(string symbol)
        {
            string average = string.Empty;
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = Cs;
                    comm.CommandText = "SELECT " +
                                       "AVG(DayHigh - [Open]) " +
                                       "FROM[Barchart].[dbo].[Yahoo] " +
                                       $"Where Symbol = '{symbol}' And [DATE] >= DATEADD(DAY, -500, [Date])";
                    comm.Connection = conn;
                    conn.Open();

                    average = Convert.ToString(comm.ExecuteScalar());
                }
            }

            try
            {
                return Convert.ToDecimal(average).ToString("#.##");
            }
            catch (Exception ex)
            {
                return ".00";
            }
        }

        public string GetDayHighBySymbol(string symbol)
        {
            string json = "[";

            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = Cs;
                    comm.CommandText = "SELECT " +
                                       "[Date], " +
                                       "CASE  " +
                                       "WHEN ([DayHigh] - [Open]) > 0 THEN ([DayHigh] - [Open]) " +
                                       "ELSE '0.00' " +
                                       "END AS DayHigh, " +
                                       "CASE " +
                                       "WHEN ([DayHigh] - [Open]) <= 0 THEN ([Open] - [Close]) " +
                                       "ELSE '0.00' " +
                                       "END AS DayLow " +
                                       $"FROM [Barchart].[dbo].[Yahoo] Where Symbol = '{symbol}' And [DATE] >= DATEADD(DAY, -500, GETDATE()) Order By Date DESC";
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

            if (json.LastIndexOf(',') > 0) json = json.Substring(0, json.LastIndexOf(','));
            return json += "]";
        }

        /// <summary>
        /// This was for Yahoo to retreive a data range
        /// </summary>
        public void RetreivePeriods()
        {
            var epoch = Convert.ToDateTime("1970-01-01");

            var epochTicks = epoch.Ticks;
            var today = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            today = today.AddHours(16);
            today = today.AddSeconds(805);

            var yearAgo = today.AddYears(-1);


            var todayTicks = today.Ticks;
            var yearAgoTicks = yearAgo.Ticks;

            var todayTicksFromEpoch = todayTicks - epochTicks;
            var yearAgoTicksFromEpoch = yearAgoTicks - epochTicks;

            var todaySecondsFromEpoch = todayTicksFromEpoch / 10000000;
            var yearAgoSecondsFromEpoch = yearAgoTicksFromEpoch / 10000000;



            SettingsDa settingsDa = new SettingsDa();
            settingsDa.UpsertSetting("period1", Convert.ToString(yearAgoSecondsFromEpoch));
            settingsDa.UpsertSetting("period2", Convert.ToString(todaySecondsFromEpoch));

            // LastPeriodUpdateTime = DateTime.Now;
            //}
        }
    }
}