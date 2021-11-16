namespace OOA_Inlamning1
{

    using OOA_Inlamning1.Helpers;
    using System;
    using static Helpers.ConsolePrintHelpers;

    internal static class SqlAnswers
    {
        internal static void Start()
        {
            (bool dataExists, string name) = DBHelpers.CheckData();
            if (dataExists) AnswerMenu(name);
            else Console.WriteLine("Could not locate any data to query, exiting...");
        }

        private static void AnswerMenu(string tableName)
        {
            Console.CursorVisible = false;
            ConsoleKeyInfo input = new();
            do
            {
                Console.Clear();
                Console.WriteLine($"Table: {tableName}");
                Console.WriteLine("Please select which question you would like the answer to:");
                Console.WriteLine("\n1) How many diffrent countries are represented?");
                Console.WriteLine("2) Are all usernames and passwords unique?");
                Console.WriteLine("3) How many people are from Norden and Scandinavia, respectivly?");
                Console.WriteLine("4) Which is the most represented country?");
                Console.WriteLine("5) List first 10 users with a last name beginning with \"L\".");
                Console.WriteLine("6) List all users with name and last name beginning with the same letter.");
                Console.WriteLine("\n7) Do a custom search, all info reply.");
                Console.WriteLine("8) Do a custom search, single column reply.");
                Console.WriteLine("\nE or Escape) Exit application.");
                Wait(false, true);
                input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.D1 or ConsoleKey.NumPad1: AccessDB.DiffrentCountries(); break;
                    case ConsoleKey.D3 or ConsoleKey.NumPad3: AccessDB.FromNordenVsScandinavia(); break;
                    case ConsoleKey.D4 or ConsoleKey.NumPad4: AccessDB.MostRepresentedCountry(); break;
                    case ConsoleKey.D5 or ConsoleKey.NumPad5: AccessDB.FirstTenLastNameStartWithL(); break;
                    case ConsoleKey.D6 or ConsoleKey.NumPad6: AccessDB.FirstLastAlliteration(); break;
                    case ConsoleKey.D2 or ConsoleKey.NumPad2: AccessDB.UsernameAndPassword(); break;
                    case ConsoleKey.D7 or ConsoleKey.NumPad7: AccessDB.DoQuery(); break;
                    case ConsoleKey.D8 or ConsoleKey.NumPad8: AccessDB.DoSingleColumnQuery(); break;
                    default: break;
                }
            } while (input.Key != ConsoleKey.Escape && input.Key != ConsoleKey.E);
            ExitMenu();
        }
        private static void ExitMenu()
        {
            ConsoleKeyInfo input = new();
            do
            {
                Console.Clear();
                if (DBHelpers.CheckForDB() && DBHelpers.CheckForTable()) Console.WriteLine("D) Press D if you wish to delete the datatable and database.");
                Console.WriteLine("E or Escape) Exit application.");
                Wait(false, true);
                input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.D: DBHelpers.ExitCheckData(); break;
                    default: break;
                }
            } while (input.Key != ConsoleKey.Escape && input.Key != ConsoleKey.E);
            Console.CursorVisible = true;
        }


    }

}
