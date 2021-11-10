namespace OOA_Inlamning1.Helpers
{

    using OOA_Inlamning1.Extensions;
    using System;
    using System.Data.SqlClient;
    using System.Xml.Linq;
    using Dapper;
    using System.IO;

    internal static class DBHelpers
    {
        internal static bool CheckForDB(string name)
        {
            int returnVal = 0;
            string sql = $"SELECT COUNT(name) FROM master.dbo.sysdatabases WHERE name = '{name}'";
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr("BuildDB")))
            {
                try
                {
                    returnVal = (int)connection.ExecuteScalar(sql);
                }
                catch (Exception ex)
                {
                    ex.LogThis();
                    Console.WriteLine("Error in checking for database existance.");
                    Console.ReadLine();
                }
            }
            return returnVal == 1 ? true : false;
        }

        internal static void CreateTable()
        {
            string sql = File.ReadAllText(@"..\..\..\FakePeople.sql");
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr("PeopleDB")))
            {
                try
                {
                    connection.Execute(sql);

                }
                catch (Exception ex)
                {
                    ex.LogThis();
                    Console.WriteLine("Error while trying to create database.");
                    Console.ReadLine();
                }
            }
        }

        internal static void CreateDB(string dbName)
        {
            string sql = $"IF NOT EXISTS (SELECT NAME FROM master.dbo.sysdatabases WHERE name = '{dbName}') CREATE DATABASE {dbName}";
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr("BuildDB")))
            {
                try
                {
                    connection.Execute(sql);

                }
                catch (Exception ex)
                {
                    ex.LogThis();
                    Console.WriteLine("Error while trying to create database.");
                    Console.ReadLine();
                }
            }

        }
        internal static bool CheckForTable(string name)
        {
            bool tableExists = false;
            string sql = $"SELECT count(name) FROM dbo.sysobjects where name = 'FakePeople' and xtype = 'U'";
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.CnnStr("PeopleDB")))
            {
                try
                {
                    if ((int)connection.ExecuteScalar(sql) == 1) tableExists = true;
                }
                catch (Exception ex)
                {
                    ex.LogThis();
                    Console.WriteLine("Error while checking for table existance.");
                    Console.ReadLine();
                }
            }
            return tableExists;
        }
    }
}
