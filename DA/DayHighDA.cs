using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using DM;

namespace DA 
{
    
    public class DayHighDa : BaseDa
    {
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

        public DayHigh GetSumGrid(DayHigh dh, string symbol)
        {
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = Cs;
                    comm.CommandText = "SumGrid";
                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("Symbol", symbol);
                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader dr = comm.ExecuteReader();
                    dr.Read();

                    dh.V100 = Convert.ToString(dr["T100"]);
                    dh.V90 = Convert.ToString(dr["T90"]);
                    dh.V80 = Convert.ToString(dr["T80"]);
                    dh.V70 = Convert.ToString(dr["T70"]);
                    dh.V60 = Convert.ToString(dr["T60"]);
                    dh.V50 = Convert.ToString(dr["T50"]);
                    dh.V40 = Convert.ToString(dr["T40"]);
                    dh.V30 = Convert.ToString(dr["T30"]);
                    dh.V20 = Convert.ToString(dr["T20"]);
                    dh.V10 = Convert.ToString(dr["T10"]);
                }
            }

            return dh;
            //Symbol Name    Average DaysAboveAvg    Total AdjustedTotal   DaysCloseAboveOpen DaysHighAboveOpen	% Day High Above Open   StdDev Records
        }

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
        public void DownloadHistory(string symbol)
        {
            try
            {
                SettingsDa sda = new SettingsDa();
                string p1 = sda.GetSetting("Period1");
                string p2 = sda.GetSetting("Period2");
                var wr = WebRequest.Create(
                    $"http://query1.finance.yahoo.com/v7/finance/download/{symbol}?period1={p1}&period2={p2}&interval=1d&events=history");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var resp = wr.GetResponse();

                var sr = new StreamReader(resp.GetResponseStream());



                System.IO.File.AppendAllText($"C:\\temp\\{symbol}.csv", sr.ReadToEnd());
                UploadData($"C:\\temp\\{symbol}.csv", symbol);
                //Microsoft.VisualBasic.FileSystem.Rename($"C:\\Users\\dlolf\\Downloads\\{symbol}.csvx", $"C:\\Users\\dlolf\\Downloads\\{symbol}.csv");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void UploadData(string path, string symbol)
        {
            string sqlCommand = string.Empty;

            //string[] files = Directory.GetFiles(@"C:\Users\dlolf\Downloads", "*.csv");
            //foreach (var file in files)
            //{
            try
            {


                var lines = System.IO.File.ReadAllLines(path);

                bool header = true;
                foreach (var line in lines)
                {
                    try
                    {
                        if (!header)
                        {
                            var fields = line.Split(",".ToCharArray());
                            Record rec = new Record();
                            rec.Date = fields[0];
                            rec.Open = fields[1];
                            rec.High = fields[2];
                            rec.Low = fields[3];
                            rec.Close = fields[4];
                            rec.Adjclose = fields[5];
                            rec.Volume = fields[6];

                            if (SymbolDateExist(symbol, rec.Date))
                            {
                                sqlCommand =
                                    $"Update [Yahoo] Set DayHigh = '{rec.High}', [Open] = '{rec.Open}', [Close] = '{rec.Close}', [DayLow] = '{rec.Close}', Volume = '{rec.Volume}' Where Symbol = '{symbol}' And [Date] = '{rec.Date}'";
                            }
                            else
                            {
                                sqlCommand =
                                    $"Insert Into [Yahoo] (Symbol, [Date], DayHigh, [Open], [Close], DayLow, Volume) Values('{symbol}','{rec.Date}','{rec.High}','{rec.Open}','{rec.Close}', '{rec.Low}', '{rec.Volume}')";
                            }

                            ExecuteSqlCommand(sqlCommand);
                        }

                        header = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //}
        }

        public bool SymbolDateExist(string symbol, string date)
        {
            bool exists = false;
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(Cs))
                {
                    comm.CommandText =
                        $"Select Count(1) From [Yahoo] Where [Date] = '{Convert.ToDateTime(date).ToString("yyyy-MM-dd")}' And Symbol = '{symbol}'";
                    comm.Connection = conn;
                    conn.Open();
                    exists = Convert.ToBoolean(comm.ExecuteScalar());
                }
            }

            return exists;
        }
    }
}
