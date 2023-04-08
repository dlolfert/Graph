using System.Data.SqlClient;

namespace DA
{
    public class BaseDa
    {
        public string Cs = @"Server=(localdb)\MSSQLLocalDB;Database=Barchart;User Id=sa;Password=@a88word";
        public int ExecuteSqlCommand(string sqlCommand)
        {
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(Cs))
                {
                    comm.CommandText = sqlCommand;
                    comm.Connection = conn;
                    conn.Open();
                    return comm.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(string sqlCommand)
        {
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(Cs))
                {
                    comm.CommandText = sqlCommand;
                    comm.Connection = conn;
                    conn.Open();
                    return comm.ExecuteScalar();
                }
            }
        }

        //public SqlDataReader ExecuteReader(string command)
        //{
        //    using (SqlCommand comm = new SqlCommand())
        //    {
        //        using (SqlConnection conn = new SqlConnection(Cs))
        //        {
        //            comm.CommandText = command;
        //            comm.Connection = conn;
                    
        //            conn.Open();
        //            return comm.ExecuteReader(CommandBehavior.CloseConnection);
        //        }
        //    }
        //}
    }
}
