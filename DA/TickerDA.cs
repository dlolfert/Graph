using DM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace DA
{
    public class TickerDA : BaseDa
    {
        public void DownloadHistory(string symbol)
        {
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

            // Alpha Advantage
            DownloadAlphaAdvantage(symbol);

        }

        public void DownloadAlphaAdvantage(string symbol)
        {
            try
            {
                // replace the "demo" apikey below with your own key from https://www.alphavantage.co/support/#api-key
                string QUERY_URL = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&datatype=json&apikey=WO3Z4BIZGDF6BMR4";
                Uri queryUri = new Uri(QUERY_URL);

                using (WebClient client = new WebClient())
                {
                    // -------------------------------------------------------------------------
                    // if using .NET Framework (System.Web.Script.Serialization)

                    //JavaScriptSerializer js = new JavaScriptSerializer();
                    //dynamic json_data = js.Deserialize(client.DownloadString(queryUri), typeof(object));

                    // -------------------------------------------------------------------------
                    // if using .NET Core (System.Text.Json)
                    // using .NET Core libraries to parse JSON is more complicated. For an informative blog post
                    // https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-apis/

                    dynamic json_data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(client.DownloadString(queryUri));

                    // -------------------------------------------------------------------------

                    // do something with the json_data
                    System.IO.File.AppendAllText($"C:\\temp\\{symbol}.csv", json_data["Time Series (Daily)"].ToString());
                }
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

                using (SqlCommand comm = new SqlCommand())
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

            System.IO.File.WriteAllText($"C:\\Temp\\{symbol}_Summary.txt", strResponse);
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
                            $"If NOT EXISTS (Select * From Yahoo Where Symbol = '{symbol}' and [Date] = '{Convert.ToDateTime(rec.Date).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture)}') " +
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