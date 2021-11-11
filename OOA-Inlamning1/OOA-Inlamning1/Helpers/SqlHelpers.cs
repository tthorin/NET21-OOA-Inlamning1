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
            int val;
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr(connectionString)))
            {
                val = connection.ExecuteScalar<int>(sql);
            }
            return val;
        }
        internal static void Execute(string sql, string connectionString = "PeopleDB")
        {

            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr(connectionString)))
            {
                connection.Execute(sql);
            }

        }
        internal static List<Person> QueryPerson(string sql, string connectionString = "PeopleDB")
        {
            List<Person> list = new();
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr(connectionString)))
            {
                list = connection.Query<Person>(sql).ToList();
            }
            return list;
        }
        internal static List<(string, int)> QueryTupleStringInt(string sql, string connectionString = "PeopleDB")
        {
            List<(string, int)> list = new();
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr(connectionString)))
            {
                list = connection.Query<(string, int)>(sql).ToList();
            }
            return list;
        }
        internal static List<(int, int, int)> QueryTupleIntIntInt(string sql, string connectionString = "PeopleDB")
        {
            List<(int, int, int)> list = new();
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr(connectionString)))
            {
                list = connection.Query<(int, int, int)>(sql).ToList();
            }
            return list;
        }
    }
}
