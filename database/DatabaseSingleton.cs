﻿using System.Configuration;
using Microsoft.Data.SqlClient;
//Note: this part of the code is NOT mine (DatabaseSingle)

namespace DatabaseEditingProgram.database
{
    public class DatabaseSingleton
    {
        private static SqlConnection? conn = null;
        private DatabaseSingleton()
        {

        }
        public static SqlConnection GetInstance()
        {
            if (conn == null)
            {
                SqlConnectionStringBuilder consStringBuilder = new SqlConnectionStringBuilder();
                consStringBuilder.UserID = ReadSetting("Name");
                consStringBuilder.Password = ReadSetting("Password");
                consStringBuilder.InitialCatalog = ReadSetting("Database");
                consStringBuilder.DataSource = ReadSetting("DataSource");
                consStringBuilder.ConnectTimeout = 30;
                consStringBuilder.TrustServerCertificate = true;

                conn = new SqlConnection(consStringBuilder.ConnectionString);
                Console.WriteLine(consStringBuilder.ConnectionString);
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
            }
            return conn;
        }


        public static void CloseConnection()
        {
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }

        private static string ReadSetting(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings[key] ?? "Not Found";
            return result;
        }
    }
}
