using System;
using DA;

namespace JobController
{
    class Program
    {
        static void Main(string[] args)
        {
            var yahooDa = new YahooDa();
            var dayHighDa = new DayHighDa();

            var symbols = yahooDa.GetDistinctSymbolList();

            foreach (var symbol in symbols)
            {
                Console.WriteLine($"Downloading History for : {symbol}");
                dayHighDa.DownloadHistory(symbol);
            }
        }
    }
}
