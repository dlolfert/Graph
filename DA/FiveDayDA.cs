using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using DM;

namespace DA
{
    public class FiveDayDA : BaseDa
    {
        public DateTime FindFirstTradingDay(int DaysBack)
        {
            var start = DateTime.Now.AddDays(DaysBack * -1);
            while (start.DayOfWeek != DayOfWeek.Monday)
            {
                start = start.AddDays(1);
            }

            return start;
        }

        public DateTime FindSecondTradingDay(DateTime startDate)
        {
            return startDate.AddDays(1);
        }

        public DateTime FindLastTradingDay(DateTime startDate)
        {
            while (startDate.DayOfWeek != DayOfWeek.Friday)
            {
                startDate = startDate.AddDays(1);
            }

            return startDate;
        }

        public decimal GetOpenPrice(DateTime startDate, string symbol)
        {
            return Convert.ToDecimal(
                ExecuteScalar(
                    @"Select [Open] From [Barchart].[dbo].[ZacksRank] Where [Date] = '{startDate}' And Symbol = '{symbol}'"));
        }
    }
}
