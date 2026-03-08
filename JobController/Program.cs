using DA;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Threading;

namespace JobController
{
    class Program
    {
        static void Main(string[] args)
        {
            var tickerDA = new TickerDA();

            var symbols = tickerDA.GetTickerList();

            foreach (var symbol in symbols)
            {
                Console.WriteLine($"Downloading Summary for : {symbol}");
                HandleSymbol(symbol);

                Thread.Sleep(15000);
            }
        }

        static void HandleSymbol(string symbol)
        {
            string strResponse = string.Empty;

            try
            {
                string url = $"https://api.polygon.io/v3/reference/tickers/{symbol.ToUpper()}?apiKey=RaYykPoInrSUBjOV582kC4zI_mhXpJxq";
                WebResponse response = null;
                string command = string.Empty;


                var wr = WebRequest.Create(url);

                try
                {
                    response = wr.GetResponse();
                    strResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(strResponse);

                    var name = obj.results.name.Value.ToString().Replace("'", "^");

                    command = $"Select [Name] From [Barchart].[dbo].[Top100] Where Symbol = '{symbol}' " +
                                  "IF(@@ROWCOUNT > 0) " +
                                  "BEGIN " +
                                  $"Update [Barchart].[dbo].[Top100] Set [Name] = '{name}', [Date] = GETDATE() Where Symbol = '{symbol}' " +
                                  "END " +
                                  "ELSE " +
                                  "BEGIN " +
                                  $"INSERT INTO [Barchart].[dbo].[Top100] ([Name], Symbol, [Date]) Values('{name}', '{symbol.ToUpper()}', GETDATE()) " +
                                  "END";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if (ex.Message.Equals("The remote server returned an error: (404) Not Found."))
                    {
                        command = $"Delete From [Barchart].[dbo].[Top100] Where Symbol = '{symbol}'";
                    }
                }

                if (command != string.Empty)
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        using (SqlConnection conn = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=Barchart;User Id=sa;Password=@a88word;Connection Timeout=360;"))
                        {
                            comm.CommandText = command;
                            comm.Connection = conn;

                            conn.Open();

                            comm.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw;
            }

            if (strResponse != string.Empty)
            {
                System.IO.File.WriteAllText($"C:\\Temp\\{symbol}_api.ploygon.io.Summary.txt", strResponse);
            }
            else
            {
                System.IO.File.WriteAllText($"C:\\Temp\\{symbol}_deleted_api.ploygon.io.Summary.txt", $"Deleted Symbol : {symbol}");
            }
        }
    }
}