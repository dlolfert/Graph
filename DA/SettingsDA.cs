using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Text;

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
            int found = ExecuteSqlCommand("Select Count(*) From settings Where [Key] = '{key}'");
            if (Convert.ToBoolean(found))
            {
                ExecuteSqlCommand($"Update settings set [Value] = '{value}' Where [Key] = '{key}'");
            }
            else
            {
                ExecuteSqlCommand($"Insert Into settings Values('{key}', '{value}')");
            }
        }

        public IDictionary<string,string> GetAllSettings()
        {
            IDictionary<string,string> dict = new Dictionary<string, string>();
            SqlDataReader dr = ExecuteReader("Select * From settings");
            while (dr.Read())
            {
                dict.Add(dr["Key"].ToString(), dr["Value"].ToString());
            }

            return dict;
        } 
    }
}