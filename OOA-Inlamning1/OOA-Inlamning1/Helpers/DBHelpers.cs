namespace OOA_Inlamning1.Helpers
{
    
    using OOA_Inlamning1.Extensions;
    using System;
    using System.Data.SqlClient;

    internal static class DBHelpers
    {
        private static string dbName = "People";
        internal static bool CheckForDB()
        {
            int returnVal = 0;
            string sql = $"SELECT COUNT(name) FROM master.dbo.sysdatabases where name = '{dbName}'";
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr("BuildDB")))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                try
                {
                    connection.Open();
                    returnVal = (int)cmd.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    ex.LogThis();
                    Console.WriteLine("Could not connect to local database.");
                    Console.ReadLine();
                }
            }
            return returnVal == 1 ? true : false;
        }
    }
}
