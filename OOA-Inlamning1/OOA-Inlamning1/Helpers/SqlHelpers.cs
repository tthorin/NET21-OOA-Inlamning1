namespace OOA_Inlamning1.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Dapper;

    internal static class SqlHelpers
    {
        internal static int ExecuteScalar(string sql, string connectionString = "PeopleDB")
        {

            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr(connectionString)))
            {
                return (int)connection.ExecuteScalar(sql);
            }

        }
        internal static void Execute(string sql, string connectionString = "PeopleDB")
        {

            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr(connectionString)))
            {
                connection.Execute(sql);
            }

        }
    }
}
