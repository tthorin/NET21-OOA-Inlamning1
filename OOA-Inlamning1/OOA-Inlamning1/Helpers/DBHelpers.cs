namespace OOA_Inlamning1.Helpers
{
    using System;
    using System.IO;
    using static Helpers.SqlHelpers;
    using static Helpers.ConsolePrintHelpers;
    using System.Configuration;

    internal static class DBHelpers
    {
        private static string dbName = "TT_Net21_People";
        private static string tableName = "FakePeople";
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

        private static void CreateTable()
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
        internal static bool CheckForTable(bool verboose = false)
        {
            bool tableExists = false;
            if (CheckForImportFile()) tableExists = CheckForImportTable();
            else tableExists = verboose ? CheckForOtherTables(true) : tableExists = CheckForOtherTables();
            return tableExists;
        }

        private static bool CheckForOtherTables(bool verboose = false)
        {
            if (verboose) Console.WriteLine($"No .sql file found in {ConfigurationManager.AppSettings["sqlDir"]}, looking for other tables in {dbName}...");
            bool tableExists = false;
            string sql = "SELECT count(name) FROM dbo.sysobjects where xtype = 'U'";
            if (ExecuteScalar(sql) >= 1)
            {
                tableExists = true;
                var getTableName = "SELECT top 1 name FROM dbo.sysobjects where xtype = 'U'";
                var tableList = QueryTupleStringInt(getTableName);
                tableName = tableList[0].Item1;
                AccessDB.tableName = tableName;
            }
            else if (verboose) Console.WriteLine("No other tables found...");
            return tableExists;
        }

        private static bool CheckForImportTable()
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
                        tableName = line.Substring(13, line.IndexOf(' ', 13) - 13);
                        AccessDB.tableName = tableName;
                        break;
                    }
                }
            }
            return fileExists;
        }

        private static void AskToCreateTable()
        {
            string input = "";
            do
            {
                Console.Write($"Table {tableName} doesn't exist, would you like to (c)reate it or (e)xit? ");
                input = GetUserString(true);
            } while (!(input == "c" || input == "create") && !(input == "e" || input == "exit"));
            if (input[0] == 'c') CreateTable();
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
            if (CheckForDB() && CheckForImportFile() && !CheckForTable(true)) AskToCreateTable();
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
            var sql = $"DROP TABLE {tableName}";
            Execute(sql);
        }
    }

}
