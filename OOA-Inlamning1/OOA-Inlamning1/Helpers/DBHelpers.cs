namespace OOA_Inlamning1.Helpers
{
    using System;
    using System.IO;
    using static Helpers.SqlHelpers;
    using static Helpers.ConsolePrintHelpers;
    using System.Configuration;
    using System.Collections.Generic;

    internal static class DBHelpers
    {
        internal static string DbName { get; } = "TT_Net21_People";
        private static string tableName = "";
        private static string sqlFile = "";
        internal static bool CheckForDB()
        {
            bool isThereAServer = CheckForDBServer();
            bool dbExists = false;

            string sql = $"SELECT COUNT(name) FROM master.dbo.sysdatabases WHERE name = '{DbName}'";
            if (ExecuteScalar(sql, "master") == 1) dbExists = true;
            return dbExists;
        }

        private static bool CheckForDBServer()
        {
            var serverExists = false;
            try
            {
                serverExists = ExecuteScalar("SELECT COUNT(@@VERSION)", "master") == 1;
            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine("Could not connect to SQL server, exiting application.");
                Hold();
                Environment.Exit(0);
            }
            return serverExists;
        }

        private static void ImportTable()
        {
            string sql = File.ReadAllText(sqlFile);
            Execute(sql);
            string file = ConfigurationManager.AppSettings["importedTables"];
            List<string> importedTables = new();
            if (File.Exists(file)) importedTables.AddRange(File.ReadAllLines(file));
            if (!importedTables.Contains(tableName)) importedTables.Insert(0, tableName);
            File.WriteAllLines(file, importedTables);
        }

        private static void CreateDB(string dbName)
        {
            string sql = $"IF NOT EXISTS (SELECT NAME FROM master.dbo.sysdatabases WHERE name = '{dbName}') CREATE DATABASE {dbName}";
            Execute(sql, "master");

        }
        

        private static bool CheckForOtherTables()
        {
            bool otherTableExists = false;
            var file = ConfigurationManager.AppSettings["importedTables"];
            List<string> tables = new();
            if (File.Exists(file)) tables.AddRange(File.ReadAllLines(file));
            if (tables.Count > 0)
            {
                SetTableName(tables[0]);
                otherTableExists = true;
            }
            return otherTableExists;
        }

        private static void SetTableName(string tableName)
        {
            DBHelpers.tableName = tableName;
            AccessDB.tableName = tableName;
            
        }

        internal static bool CheckForTable(string name = "")
        {
            bool tableExists = false;
            string localName = name == "" ? tableName : name;
            string sql = $"SELECT count(name) FROM dbo.sysobjects where name = '{localName}' and xtype = 'U'";
            if (ExecuteScalar(sql) == 1)
            {
                SetTableName(localName);
                tableExists = true;
            }
            return tableExists;
        }

        private static (bool, string) CheckForImportFile()
        {
            bool fileExists = false;
            string tableNameFromFile = "";
            string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["sqlDir"], "*.sql");
            if (files.Length > 0)
            {
                fileExists = true;
                sqlFile = files[0];
                foreach (var line in File.ReadLines(sqlFile))
                {
                    if (line.ToLower().Contains("create table"))
                    {
                        //todo: make nicer substring selection
                        tableNameFromFile = (line.Substring(13, line.IndexOf(' ', 13) - 13));
                        break;
                    }
                }
            }
            return (fileExists, tableNameFromFile);
        }

        private static void AskToImportTable(string nameFromFile)
        {
            string input = "";
            do
            {
                Console.Write($"Table {nameFromFile} doesn't exist, would you like to (c)reate it or (s)kip? ");
                input = GetUserString(true);
            } while (!(input == "c" || input == "create") && !(input == "s" || input == "skip"));
            if (input[0] == 'c')
            {
                SetTableName(nameFromFile);
                ImportTable();
            }
        }


        private static void AskToCreateDB()
        {
            string input = "";
            do
            {
                Console.Write($"Database {DbName} doesn't exist, would you like to (c)reate it or (s)kip? ");
                input = GetUserString(true);
            } while (!(input == "c" || input == "create") && !(input == "s" || input == "skip"));
            if (input[0] == 'c') CreateDB(DbName);
        }

        internal static (bool,string) CheckData()
        {
            (bool exists, string tableName) data = (false,"");
            (bool exists, string tableName) importFile = CheckForImportFile();
            if (!CheckForDB()) AskToCreateDB();
            if (CheckForDB() && importFile.exists && !CheckForTable(importFile.tableName)) AskToImportTable(importFile.tableName);
            if (CheckForDB() && !CheckForTable(importFile.tableName)) CheckForOtherTables();
            if (CheckForDB() && CheckForTable())
            {
                data.exists = true;
                data.tableName = tableName;
            }
            return data;
        }
        internal static void ExitCheckData()
        {
            Console.CursorVisible = true;
            if (CheckForDB() && CheckForTable()) AskToDeleteTable();
            if (!CheckForTable() && CheckForDB()) AskToDeleteDB();
            Console.CursorVisible = false;
        }
        private static void AskToDeleteDB()
        {
            string input = "";
            string sql = $"SELECT count(id) FROM dbo.sysobjects where xtype = 'U'";
            var numberOfTables = ExecuteScalar(sql);
            do
            {
                Console.WriteLine($"\nWould you like to delete the database {DbName} (there are {numberOfTables} tables in the DB)?");
                Console.Write("Please type the whole word \"delete\" to delete it, or \"c\" or \"cancel\" to abort: ");
                input = GetUserString(true);
            } while (!(input == "c" || input == "cancel") && !(input == "delete"));
            if (input == "delete") DeleteDB();
        }

        private static void DeleteDB()
        {
            File.WriteAllText(ConfigurationManager.AppSettings["importedTables"], "");
            var sql = $"ALTER DATABASE {DbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE {DbName}";
            Execute(sql, "master");
        }

        private static void AskToDeleteTable()
        {
            string input = "";
            do
            {
                Console.WriteLine($"\nWould you like to delete the table {tableName}?");
                Console.Write("Please type the whole word \"delete\" to delete it, or \"c\" or \"cancel\" to abort: ");
                input = GetUserString(true);
            } while (!(input == "c" || input == "cancel") && !(input == "delete"));
            if (input == "delete") DeleteTable();
        }

        private static void DeleteTable()
        {
            var file = ConfigurationManager.AppSettings["importedTables"];
            List<string> importedTables = new();
            if (File.Exists(file)) importedTables.AddRange(File.ReadAllLines(file));
            if (importedTables.Count > 0)
            {
                importedTables.RemoveAt(importedTables.FindIndex(x => x == tableName));
            }
            File.WriteAllLines(file, importedTables);

            var sql = $"DROP TABLE {tableName}";
            Execute(sql);
        }
    }

}
