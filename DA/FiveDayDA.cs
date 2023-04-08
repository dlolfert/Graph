using System.Data;
using System.Data.SqlClient;

namespace DA
{
    public class FiveDayDa : BaseDa
    {

        public void BuildData(string symbol, int daysBack)
        {
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(this.Cs))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.Add("Symbol", SqlDbType.NChar).Value = symbol;
                    comm.Parameters.Add("DaysBack", SqlDbType.Int).Value = daysBack;
                    comm.CommandText = "[WeeklyHigh]";
                    comm.Connection = conn;
                    conn.Open();

                    comm.ExecuteNonQuery();
                }
            }
        }
    }
}
