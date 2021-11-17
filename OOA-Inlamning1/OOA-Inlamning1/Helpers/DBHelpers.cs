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
        internal static bool CheckForDB()
        {
            CheckForDBServer();
            bool dbExists = false;

            string sql = $"SELECT COUNT(name) FROM master.dbo.sysdatabases WHERE name = '{DbName}'";
            if (ExecuteScalar(sql, "master") == 1) dbExists = true;
            return dbExists;
        }

        private static void CheckForDBServer()
        {
            try
            {
                ExecuteScalar("SELECT COUNT(@@VERSION)", "master");
            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine("Could not connect to SQL server, exiting application.");
                Hold();
                Environment.Exit(0);
            }
        }

        private static void ImportTable(string importFile)
        {
            string sql = File.ReadAllText(importFile);
            Execute(sql);
            string file = ConfigurationManager.AppSettings["importedTables"];
            List<string> importedTables = new();
            if (File.Exists(file)) importedTables.AddRange(File.ReadAllLines(file));
            if (!importedTables.Contains(tableName)) importedTables.Add(tableName);
            File.WriteAllLines(file, importedTables);
        }

        private static void CreateDB(string dbName)
        {
            string sql = $"IF NOT EXISTS (SELECT NAME FROM master.dbo.sysdatabases WHERE name = '{dbName}') CREATE DATABASE {dbName}";
            Execute(sql, "master");

        }


        private static int CheckForOtherTables()
        {
            int numberOfTables = 0;
            var file = ConfigurationManager.AppSettings["importedTables"];
            List<string> tables = new();
            if (File.Exists(file)) tables.AddRange(File.ReadAllLines(file));
            if (tables.Count > 0 && tableName == "") SetTableName(tables[0]);
            if (tables.Count > 0) numberOfTables = tables.Count;
            return numberOfTables;
        }

        private static void SetTableName(string tableName)
        {
            DBHelpers.tableName = tableName;
            AccessDB.tableName = tableName;
            SqlAnswers.tableName = tableName;

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

        internal static void ChangeTable()
        {
            List<string> tables = new();
            string file = ConfigurationManager.AppSettings["importedTables"];
            if (File.Exists(file)) tables.AddRange(File.ReadAllLines(file));
            int choice = ChangeTableMenu(tables);
            if (choice != -1) SetTableName(tables[choice]);
        }

        private static int ChangeTableMenu(List<string> tables)
        {
            char input;
            int choice;
            bool inRange = false;
            Console.Clear();
            do
            {
                Console.WriteLine($"Current table is: {tableName}");
                Console.WriteLine();
                for (int i = 0; i < tables.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {tables[i]}");
                }
                Console.WriteLine("\nP) Previous menu.");
                input = Console.ReadKey(true).KeyChar;
                choice = char.IsDigit(input) ? Int32.Parse(input.ToString()) : -1;
                if (choice > 0 && choice <= tables.Count)
                {
                    choice--;
                    inRange = true;
                }

            } while (!inRange && input.ToString().ToLower() != "p");
            return choice;
        }

        private static void CheckForImportFiles()
        {
            string tableNameFromFile;
            string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["sqlDir"], "*.sql");
            if (files.Length > 0)
            {
                foreach (var file in files)
                {
                    tableNameFromFile = "";
                    foreach (var line in File.ReadLines(file))
                    {
                        if (line.ToLower().Contains("create table"))
                        {
                            int idxSecondSpace = line.Trim().IndexOf(' ', line.Trim().IndexOf(' ') + 1);
                            int length = line.Trim().IndexOf('(') - idxSecondSpace;
                            tableNameFromFile = line.Substring(idxSecondSpace, length).Trim();
                            break;
                        }
                    }
                    if (!CheckForTable(tableNameFromFile)) AskToImportTable(tableNameFromFile, file);
                }
            }
        }

        private static void AskToImportTable(string nameFromFile, string file)
        {
            string input;
            do
            {
                Console.Write($"Table {nameFromFile} doesn't exist, would you like to (i)mport it or (s)kip? ");
                input = GetUserString(true).ToLower().Trim();
            } while (!(input == "i" || input == "import") && !(input == "s" || input == "skip"));
            if (input[0] == 'i')
            {
                SetTableName(nameFromFile);
                ImportTable(file);
            }
        }


        private static void AskToCreateDB()
        {
            string input;
            do
            {
                Console.Write($"Database {DbName} doesn't exist, would you like to (c)reate it or (s)kip? ");
                input = GetUserString(true);
            } while (!(input == "c" || input == "create") && !(input == "s" || input == "skip"));
            if (input[0] == 'c') CreateDB(DbName);
        }

        internal static (bool, string, int) CheckData()
        {
            (bool exists, string tableName, int numberOfTables) data = (false, "", 0);
            if (!CheckForDB()) AskToCreateDB();
            if (CheckForDB())
            {
                CheckForImportFiles();
                data.numberOfTables = CheckForOtherTables();
            }
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
            string input;
            string sql = $"SELECT count(id) FROM dbo.sysobjects where xtype = 'U'";
            var numberOfTables = ExecuteScalar(sql);
            do
            {
                Console.WriteLine($"\nWould you like to delete the database {DbName} (there are {numberOfTables} tables in the DB)?");
                Console.Write("Please type the whole word \"delete\" to delete it, or \"c\" or \"cancel\" to abort: ");
                input = GetUserString(true);
            } while (!(input == "c" || input == "cancel") && !(input == "delete"));
            if (input == "delete" && numberOfTables == 0) DeleteDB();
            else if (input == "delete")
            {
                Console.WriteLine($"Are you sure? There are still {numberOfTables} tables in the database, their data will be lost.");
                Console.Write("Type the whole word \"delete\" again to delete, or anything else to cancel: ");
                if (GetUserString(true) == "delete") DeleteDB();
            }
        }

        private static void DeleteDB()
        {
            File.WriteAllText(ConfigurationManager.AppSettings["importedTables"], "");
            var sql = $"ALTER DATABASE {DbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE {DbName}";
            Execute(sql, "master");
        }

        private static void AskToDeleteTable()
        {
            List<string> tables = new();
            var tableFile = ConfigurationManager.AppSettings["importedTables"];
            if (File.Exists(tableFile)) tables.AddRange(File.ReadAllLines(tableFile));
            foreach (var table in tables)
            {
                string input;
                do
                {
                    Console.WriteLine($"\nWould you like to delete the table {table}?");
                    Console.Write("Please type the whole word \"delete\" to delete it, or \"c\" or \"cancel\" to abort: ");
                    input = GetUserString(true);
                } while (!(input == "c" || input == "cancel") && !(input == "delete"));
                if (input == "delete") DeleteTable(table);
            }
        }

        private static void DeleteTable(string table)
        {
            var file = ConfigurationManager.AppSettings["importedTables"];
            List<string> importedTables = new();
            if (File.Exists(file)) importedTables.AddRange(File.ReadAllLines(file));
            if (importedTables.Count > 0)
            {
                importedTables.RemoveAt(importedTables.FindIndex(x => x == table));
            }
            File.WriteAllLines(file, importedTables);

            var sql = $"DROP TABLE {table}";
            Execute(sql);
        }
    }

}
