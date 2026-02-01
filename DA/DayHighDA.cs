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

        public void DownloadHistory(string symbol)
        {
            RetreivePeriods();

            //https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=META&outputsize=full&apikey=WO3Z4BIZGDF6BMR4
            //https://api.polygon.io/v2/aggs/ticker/AAPL/range/1/day/2024-08-08/2025-08-08?apiKey=RaYykPoInrSUBjOV582kC4zI_mhXpJxq
            /*Worked : https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&outputsize=compact&datatype=csv&apikey=WO3Z4BIZGDF6BMR4")
             * 
             * 
             */
            string from = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");
            string to = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
               var wr = WebRequest.Create(
                    $"https://api.polygon.io/v2/aggs/ticker/{symbol}/range/1/day/{from}/{to}?apiKey=RaYykPoInrSUBjOV582kC4zI_mhXpJxq");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var resp = wr.GetResponse();

                var sr = new StreamReader(resp.GetResponseStream());

                string json = sr.ReadToEnd();

                var stockData = JsonSerializer.Deserialize<StockData>(json);

                    

                //System.IO.File.AppendAllText($"C:\\temp\\{symbol}.csv", sr.ReadToEnd());
                UploadData(stockData, symbol);
                //Microsoft.VisualBasic.FileSystem.Rename($"C:\\Users\\dlolf\\Downloads\\{symbol}.csvx", $"C:\\Users\\dlolf\\Downloads\\{symbol}.csv");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void DownloadSummary(string symbol)
        {
            string strResponse = string.Empty;

            try
            {
                string url = $"https://api.polygon.io/v3/reference/tickers/{symbol}?apiKey=RaYykPoInrSUBjOV582kC4zI_mhXpJxq";
                
                var wr = WebRequest.Create(url);

                var response = wr.GetResponse();

                strResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();

                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(strResponse);

                var name = obj.results.name.Value.ToString().Replace("'", "^");

                var command = $"Select [Name] From [Barchart].[dbo].[Top100] Where Symbol = '{symbol}' " +
                              "IF(@@ROWCOUNT > 0) " +
                              "BEGIN " +
                              $"Update [Barchart].[dbo].[Top100] Set [Name] = '{name}', [Date] = GETDATE() Where Symbol = '{symbol}' " +
                              "END " +
                              "ELSE " +
                              "BEGIN " +
                              $"INSERT INTO [Barchart].[dbo].[Top100] ([Name], Symbol, [Date]) Values('{name}', '{symbol.ToUpper()}', GETDATE()) " +
                              "END";

                using (SqlCommand   comm = new SqlCommand())
                {
                    using (SqlConnection conn = new SqlConnection(Cs))
                    {
                        comm.CommandText = command;
                        comm.Connection = conn;

                        conn.Open();

                        comm.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw;
            }
            
            System.IO.File.WriteAllText($"C:\\Temp\\{symbol}.txt", strResponse);
        }


        public void UploadData(StockData data, string symbol)
        {
            StringBuilder sqlCommand = new StringBuilder();

            //string[] files = Directory.GetFiles(@"C:\Users\dlolf\Downloads", "*.csv");
            //foreach (var file in files)
            //{
            try
            {

                //data = data.Replace('\r', ' ');
                //var lines = data.Split('\n');

                //bool header = true;
                foreach (var result in data.results)
                {
                    try
                    {
                        //if (!header)
                        //{
                            //var fields = line.Split(",".ToCharArray());
                            Record rec = new Record();
                            rec.Date = DateTimeOffset.FromUnixTimeMilliseconds(result.t).DateTime.ToString("yyyy-MM-dd");
                            rec.Open = result.o.ToString();
                            rec.High = result.h.ToString();
                            rec.Low = result.l.ToString();
                            rec.Close = result.c.ToString();
                            //rec.Adjclose = fields[5];
                            rec.Volume = result.v.ToString();

                            string comm =
                                $"If NOT EXISTS (Select * From Yahoo Where Symbol = '{symbol}' and [Date] = '{ Convert.ToDateTime(rec.Date).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) }') " +
                                "BEGIN " +
                                $"Insert Into [Yahoo] (Symbol, [Date], DayHigh, [Open], [Close], DayLow, Volume) Values('{symbol}','{rec.Date}','{rec.High}','{rec.Open}','{rec.Close}', '{rec.Low}', '{rec.Volume}') " +
                                "END; "; 

                            sqlCommand.Append(comm);

                            //if (SymbolDateExist(symbol, rec.Date))
                            //{
                            //    sqlCommand =
                            //        $"Update [Yahoo] Set DayHigh = '{rec.High}', [Open] = '{rec.Open}', [Close] = '{rec.Close}', [DayLow] = '{rec.Close}', Volume = '{rec.Volume}' Where Symbol = '{symbol}' And [Date] = '{rec.Date}'";
                            //}
                            //else
                            //{
                            //    sqlCommand =
                            //        $"Insert Into [Yahoo] (Symbol, [Date], DayHigh, [Open], [Close], DayLow, Volume) Values('{symbol}','{rec.Date}','{rec.High}','{rec.Open}','{rec.Close}', '{rec.Low}', '{rec.Volume}')";
                            //}
                        //}

                        //header = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                ExecuteSqlCommand(sqlCommand.ToString());


                //System.IO.File.Delete(path);
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

    public class StockResult
    {
        public double v { get; set; }         // Volume
        public double vw { get; set; }      // Volume Weighted Average Price
        public double o { get; set; }       // Open Price
        public double c { get; set; }       // Close Price
        public double h { get; set; }       // High Price
        public double l { get; set; }       // Low Price
        public long t { get; set; }         // Timestamp (Unix ms)
        public int n { get; set; }          // Number of Transactions
    }

    public class StockData
    {
        public string ticker { get; set; }
        public int queryCount { get; set; }
        public int resultsCount { get; set; }
        public bool adjusted { get; set; }
        public List<StockResult> results { get; set; }
        public string status { get; set; }
        public string request_id { get; set; }
        public int count { get; set; }
    }
}