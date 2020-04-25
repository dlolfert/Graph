using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DA
{
    public class BaseDA
    {
        public string cs = "Server=NIXON,1466;Database=Barchart;User Id=sa;Password=@a88word";
        public int ExecuteSqlCommand(string sqlCommand)
        {
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(cs))
                {
                    comm.CommandText = sqlCommand;
                    comm.Connection = conn;
                    conn.Open();
                    return comm.ExecuteNonQuery();
                }
            }
        }
    }
}
