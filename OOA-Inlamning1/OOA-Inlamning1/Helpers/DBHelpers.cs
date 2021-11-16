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
        private static string dbName = "TT_Net21_People";
        private static string tableName = "";
        private static string sqlFile = "";
        internal static bool CheckForDB()
        {
            bool isThereAServer = CheckForDBServer();
            bool dbExists = false;

            string sql = $"SELECT COUNT(name) FROM master.dbo.sysdatabases WHERE name = '{dbName}'";
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
            //todo: implement msg on no import file
            string sql = File.ReadAllText(sqlFile);
            Execute(sql);
        }

        private static void CreateDB(string dbName)
        {
            string sql = $"IF NOT EXISTS (SELECT NAME FROM master.dbo.sysdatabases WHERE name = '{dbName}') CREATE DATABASE {dbName}";
            Execute(sql, "master");

        }
        //internal static bool CheckForTable(bool verboose = false)
        //{
        //    bool tableExists = false;
        //    if (CheckForImportFile()) tableExists = CheckForImportTable();
        //    else tableExists = verboose ? CheckForOtherTables(true) : CheckForOtherTables();
        //    return tableExists;
        //}

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
            string file = ConfigurationManager.AppSettings["importedTables"];
            List<string> importedTables = new();
            if (File.Exists(file)) importedTables.AddRange(File.ReadAllLines(file));
            if (!importedTables.Contains(tableName))importedTables.Insert(0, tableName);
            File.WriteAllLines(file, importedTables);
        }

        internal static bool CheckForTable()
        {
            string sql = $"SELECT count(name) FROM dbo.sysobjects where name = '{tableName}' and xtype = 'U'";
            return ExecuteScalar(sql) == 1;
        }

        private static bool CheckForImportFile()
        {
            bool fileExists = false;
            string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["sqlDir"], "*.sql");
            if (files.Length > 0)
            {
                fileExists = true;
                sqlFile = files[0];
                foreach (var line in File.ReadLines(sqlFile))
                {
                    if (line.ToLower().Contains("create table"))
                    {
                        SetTableName(line.Substring(13, line.IndexOf(' ', 13) - 13));
                        break;
                    }
                }
            }
            return fileExists;
        }

        private static void AskToImportTable()
        {
            string input = "";
            do
            {
                Console.Write($"Table {tableName} doesn't exist, would you like to (c)reate it or (e)xit? ");
                input = GetUserString(true);
            } while (!(input == "c" || input == "create") && !(input == "e" || input == "exit"));
            if (input[0] == 'c') ImportTable();
        }


        private static void AskToCreateDB()
        {
            string input = "";
            do
            {
                Console.Write($"Database {dbName} doesn't exist, would you like to (c)reate it or (e)xit? ");
                input = GetUserString(true);
            } while (!(input == "c" || input == "create") && !(input == "e" || input == "exit"));
            if (input[0] == 'c') CreateDB(dbName);
        }

        internal static bool CheckData()
        {
            if (!CheckForDB()) AskToCreateDB();
            //todo: fix this logic should be check db>check import file/table > ask to create if file and no table> check for any other table
            //todo: save last used table in file
            if (CheckForDB() && CheckForImportFile() && !CheckForTable()) AskToImportTable();
            if (CheckForDB() && !CheckForTable()) CheckForOtherTables();
            if (CheckForDB() && CheckForTable()) return true;
            else return false;
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
                Console.WriteLine($"\nWould you like to delete the database {dbName} (there are {numberOfTables} tables in the DB)?");
                Console.Write("Please type the whole word \"delete\" to delete it, or \"c\" or \"cancel\" to abort: ");
                input = GetUserString(true);
            } while (!(input == "c" || input == "cancel") && !(input == "delete"));
            if (input == "delete") DeleteDB();
        }

        private static void DeleteDB()
        {
            File.WriteAllText(ConfigurationManager.AppSettings["importedTables"], "");
            var sql = $"ALTER DATABASE {dbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE {dbName}";
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
