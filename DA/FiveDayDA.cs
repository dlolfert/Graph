using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using DM;

namespace DA
{
    public class FiveDayDA : BaseDa
    {

        public void BuildData(string Symbol, int DaysBack)
        {
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(this.Cs))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.Add("Symbol", SqlDbType.NChar).Value = Symbol;
                    comm.Parameters.Add("DaysBack", SqlDbType.Int).Value = DaysBack;
                    comm.CommandText = "[WeeklyHigh]";
                    comm.Connection = conn;
                    conn.Open();

                    comm.ExecuteNonQuery();
                }
            }
        }
    }
}
