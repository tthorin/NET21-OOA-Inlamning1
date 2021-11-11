namespace OOA_Inlamning1.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using Dapper;

    internal static class SqlHelpers
    {
        internal static int ExecuteScalar(string sql, string connectionString = "PeopleDB")
        {

            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr(connectionString)))
            {
                return connection.ExecuteScalar<int>(sql);
            }

        }
        internal static void Execute(string sql, string connectionString = "PeopleDB")
        {

            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr(connectionString)))
            {
                connection.Execute(sql);
            }

        }
        internal static List<Person> Query(string sql, string connectionString = "PeopleDB")
        {

            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr(connectionString)))
            {   
                return connection.Query<Person>(sql).ToList();
            }

        }
        internal static List<(int,int)> QueryTuple(string sql, string connectionString = "PeopleDB")
        {

            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr(connectionString)))
            {
                return connection.Query<(int,int)>(sql).ToList();
            }

        }
    }
}
