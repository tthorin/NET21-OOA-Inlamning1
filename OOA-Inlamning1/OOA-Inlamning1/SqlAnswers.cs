namespace OOA_Inlamning1
{

    using OOA_Inlamning1.Helpers;
    using System;
    using System.Collections.Generic;
    using static Helpers.ConsolePrintHelpers;
    using static Helpers.SqlHelpers;

    internal static class SqlAnswers
    {
        static List<Person> people = new();
        static internal string tableName = "";

        internal static void Start()
        {
            if (Helpers.DBHelpers.CheckData()) AnswerMenu();
        }

        private static void AnswerMenu()
        {
            Console.CursorVisible = false;
            ConsoleKeyInfo input = new();
            do
            {
                Console.Clear();
                Console.WriteLine("Please select which question you would like the answer to:");
                Console.WriteLine("1) How many diffrent countries are represented?");
                Console.WriteLine("2) Are all usernames and passwords unique?");
                Console.WriteLine("3) How many people are from Norden and Scandinavia, respectivly?");
                Console.WriteLine("4) Which is the most represented country?");
                Console.WriteLine("5) List first 10 users with a last name beginning with \"L\".");
                Console.WriteLine("6) List all users with name and last name beginning with the same letter.");
                Console.WriteLine("E or Escape) Exit application.");
                Wait(false, true);
                input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.D1 or ConsoleKey.NumPad1: DiffrentCountries(); break;
                    case ConsoleKey.D3 or ConsoleKey.NumPad3: FromNordenVsScandinavia(); break;
                    case ConsoleKey.D4 or ConsoleKey.NumPad4: MostRepresentedCountry(); break;
                    case ConsoleKey.D5 or ConsoleKey.NumPad5: FirstTenLastNameStartWithL(); break;
                    case ConsoleKey.D6 or ConsoleKey.NumPad6: FirstLastAlliteration(); break;
                    case ConsoleKey.D2 or ConsoleKey.NumPad2: UsernameAndPassword(); break;
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
        private static void UsernameAndPassword()
        {
            var sql = $"SELECT COUNT(DISTINCT id),COUNT(DISTINCT username),COUNT(DISTINCT password) FROM {tableName}";
            var values = QueryTupleIntIntInt(sql);
            Console.WriteLine($"There are {values[0].Item1} unique users, {values[0].Item2} unique usernames and {values[0].Item3} unique passwords.");
            Wait();
        }

        private static void FirstLastAlliteration()
        {
            string sql = $"SELECT id,first_name,last_name FROM {tableName} WHERE LEFT(first_name,1) = LEFT(last_name,1) GROUP BY first_name,last_name,id";
            people = QueryPerson(sql);
            Console.WriteLine("Users with first and last name beginning with the same letter:");
            Console.WriteLine("---------------------------------------------------------------");
            PrintPeopleList(people);
        }



        private static void FirstTenLastNameStartWithL()
        {
            string sql = $"SELECT TOP 10 id,first_name,last_name FROM {tableName} WHERE LOWER(last_name) LIKE 'l%'";
            people = QueryPerson(sql);
            Console.WriteLine("First 10 users with last name beginning with the letter \"L\"");
            Console.WriteLine("-----------------------------------------------------------");
            PrintPeopleList(people);
        }

        private static void MostRepresentedCountry()
        {
            string sql = $"SELECT TOP 1 country,COUNT(country) FROM {tableName} GROUP BY country ORDER BY COUNT(country) DESC";
            var countryList = QueryTupleStringInt(sql);
            Console.WriteLine($"\nMost represented country is {countryList[0].Item1} with {countryList[0].Item2} unique users.");
            Wait();

        }

        private static void FromNordenVsScandinavia()
        {
            var sql = $"SELECT (SELECT COUNT(id) FROM {tableName} WHERE country IN ('Sweden','Denmark','Norway')),(SELECT COUNT(id) FROM {tableName} WHERE country IN ('Sweden','Denmark','Finland','Norway','Iceland'))";
            var usersFromDiffrentParts = QueryTupleIntIntInt(sql);
            Console.WriteLine($"\nThere are {usersFromDiffrentParts[0].Item1} users from Norden and {usersFromDiffrentParts[0].Item2} users from Scandinavia.");
            Wait();
        }

        private static void DiffrentCountries()
        {
            var sql = $"SELECT COUNT(DISTINCT country) FROM {tableName}";
            var numberOfDiffrentCountries = ExecuteScalar(sql);
            Console.WriteLine($"\nThere are people from {numberOfDiffrentCountries} diffrent countries in the table.");
            Wait();
        }
    }

}
