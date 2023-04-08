using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DA
{
    public class SettingsDa : BaseDa
    {
        public string GetSetting(string key)
        {
            return Convert.ToString(ExecuteScalar($"Select [Value] From settings Where [Key] = '{key}'"));
        }

        public void UpsertSetting(string key, string value)
        {
            var found = ExecuteScalar($"Select Count(*) From settings Where [Key] = '{key}'");
            if (Convert.ToBoolean(found))
            {
                ExecuteSqlCommand($"Update settings set [Value] = '{value}' Where [Key] = '{key}'");
            }
            else
            {
                ExecuteSqlCommand($"Insert Into settings Values('{key}', '{value}')");
            }
        }

        public void Delete(string key)
        {
            ExecuteScalar($"Delete From Settings Where [Key] = '{key}'");
        }

        public IDictionary<string,string> GetAllSettings()
        {
            IDictionary<string,string> dict = new Dictionary<string, string>();
 
            using (SqlCommand comm = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(Cs))
                {
                    comm.CommandText = "Select * From settings";
                    comm.Connection = conn;

                    conn.Open();
                    var dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        dict.Add(dr["Key"].ToString(), dr["Value"].ToString());
                    }
                }
            }
            
            return dict;
        } 
    }
}