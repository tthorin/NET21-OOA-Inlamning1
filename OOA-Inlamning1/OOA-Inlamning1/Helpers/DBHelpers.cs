namespace OOA_Inlamning1.Helpers
{

    using OOA_Inlamning1.Extensions;
    using System;
    using System.Data.SqlClient;
    using System.Xml.Linq;
    using Dapper;
    using System.IO;
    using static Helpers.SqlHelpers;
    using System.Configuration;

    internal static class DBHelpers
    {
        private static string dbName = "TT_Net21_People";
        private static string tableName = "FakePeople";
        internal static bool CheckForDB(string name)
        {
            bool dbExists = false;
            string sql = $"SELECT COUNT(name) FROM master.dbo.sysdatabases WHERE name = '{name}'";
            if (ExecuteScalar(sql,"BuildDB")==1) dbExists = true;
            return dbExists;
        }

        internal static void CreateTable()
        {
            string filePath = ConfigurationManager.AppSettings.Get("sqlTableCreateFile");
            string sql = File.ReadAllText(filePath);
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
        private static void AskToCreateTable()
        {
            string input = "";
            do
            {
                Console.Write($"Table {tableName} doesn't exist, would you like to (c)reate it or (e)xit? ");
                input = Console.ReadLine().Trim().ToLower();
            } while (!(input == "c" || input == "create") && !(input == "e" || input == "exit"));
            if (input[0] == 'c') Helpers.DBHelpers.CreateTable();
        }


        private static void AskToCreateDB()
        {
            string input = "";
            do
            {
                Console.Write($"Database {dbName} doesn't exist, would you like to (c)reate it or (e)xit? ");
                input = Console.ReadLine().Trim().ToLower();
            } while (!(input == "c" || input == "create") && !(input == "e" || input == "exit"));
            if (input[0] == 'c') Helpers.DBHelpers.CreateDB(dbName);
        }

        internal static bool CheckData()
        {
            if (!CheckForDB(dbName)) AskToCreateDB();
            if (CheckForDB(dbName) && !CheckForTable()) AskToCreateTable();
            if (CheckForDB(dbName) && CheckForTable()) return true;
            else return false;
        }
    }

}
