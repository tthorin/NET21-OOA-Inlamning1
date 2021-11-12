namespace OOA_Inlamning1.Helpers
{
    using System;
    using System.IO;
    using static Helpers.SqlHelpers;

    internal static class DBHelpers
    {
        private static string dbName = "TT_Net21_People";
        private static string tableName = "FakePeople";
        private static string sqlFile = "";
        internal static bool CheckForDB()
        {
            bool dbExists = false;
            string sql = $"SELECT COUNT(name) FROM master.dbo.sysdatabases WHERE name = '{dbName}'";
            if (ExecuteScalar(sql, "master") == 1) dbExists = true;
            return dbExists;
        }

        private static void CreateTable()
        {
            string sql = File.ReadAllText(sqlFile);
            Execute(sql);
        }

        private static void CreateDB(string dbName)
        {
            string sql = $"IF NOT EXISTS (SELECT NAME FROM master.dbo.sysdatabases WHERE name = '{dbName}') CREATE DATABASE {dbName}";
            Execute(sql, "master");

        }
        internal static bool CheckForTable()
        {
            bool tableExists = false;
            string[] files = Directory.GetFiles(@"..\..\..\SQL\", "*.sql");
            sqlFile = files[0];
            foreach (var line in File.ReadLines(sqlFile))
            {
                if (line.ToLower().Contains("create table"))
                {
                    tableName = line.Substring(13, line.IndexOf(' ', 13) - 13);
                    SqlAnswers.tableName = tableName;
                    break;
                }
            }
            string sql = $"SELECT count(name) FROM dbo.sysobjects where name = '{tableName}' and xtype = 'U'";
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
            if (input[0] == 'c') CreateTable();
        }


        private static void AskToCreateDB()
        {
            string input = "";
            do
            {
                Console.Write($"Database {dbName} doesn't exist, would you like to (c)reate it or (e)xit? ");
                input = Console.ReadLine().Trim().ToLower();
            } while (!(input == "c" || input == "create") && !(input == "e" || input == "exit"));
            if (input[0] == 'c') CreateDB(dbName);
        }

        internal static bool CheckData()
        {
            Console.CursorVisible = true;
            if (!CheckForDB()) AskToCreateDB();
            if (CheckForDB() && !CheckForTable()) AskToCreateTable();
            Console.CursorVisible = false;
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
                input = Console.ReadLine().Trim().ToLower();
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
                input = Console.ReadLine().Trim().ToLower();
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
