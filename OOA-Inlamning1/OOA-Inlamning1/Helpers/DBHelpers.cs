namespace OOA_Inlamning1.Helpers
{

    using OOA_Inlamning1.Extensions;
    using System;
    using System.Data.SqlClient;
    using System.Xml.Linq;
    using Dapper;
    using System.IO;
    using static Helpers.SqlHelpers;

    internal static class DBHelpers
    {
        internal static bool CheckForDB(string name)
        {
            bool dbExists = false;
            string sql = $"SELECT COUNT(name) FROM master.dbo.sysdatabases WHERE name = '{name}'";
            if (ExecuteScalar(sql,"BuildDB")==1) dbExists = true;
            return dbExists;
        }

        internal static void CreateTable()
        {
            string sql = File.ReadAllText(@"..\..\..\FakePeople.sql");
           Execute(sql);
        }

        internal static void CreateDB(string dbName)
        {
            string sql = $"IF NOT EXISTS (SELECT NAME FROM master.dbo.sysdatabases WHERE name = '{dbName}') CREATE DATABASE {dbName}";
            Execute(sql,"BuildDB");

        }
        internal static bool CheckForTable()
        {
            bool tableExists = false;
            string sql = $"SELECT count(name) FROM dbo.sysobjects where name = 'FakePeople' and xtype = 'U'";
            if (ExecuteScalar(sql) == 1) tableExists = true;
            return tableExists;
        }
    }
}
