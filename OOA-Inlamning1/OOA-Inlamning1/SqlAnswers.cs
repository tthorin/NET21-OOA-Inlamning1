namespace OOA_Inlamning1
{
    using System;

    internal static class SqlAnswers
    {
        private static string dbName = "TT_Net21_People";
        private static string tableName = "FakePeople";
        internal static void Start()
        {
            if (!Helpers.DBHelpers.CheckForDB(dbName)) AskToCreateDB();
            if (!Helpers.DBHelpers.CheckForTable(tableName)) AskToCreateTable();
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
    }

}
