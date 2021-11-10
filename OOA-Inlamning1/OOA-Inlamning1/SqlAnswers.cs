namespace OOA_Inlamning1
{
    
    using OOA_Inlamning1.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using static Helpers.SqlHelpers;

    internal static class SqlAnswers
    {
        static List<Person> people = new();

        internal static void Start()
        {
            if (Helpers.DBHelpers.CheckData()) AnswerMenu();
        }

        private static void AnswerMenu()
        {
            /*Hur många olika länder finns representerade?
            • Är alla username och password unika?
            • Hur många är från Norden respektive Skandinavien?
            • Vilket är det vanligaste landet?
            • Lista de 10 första användarna vars efternamn börjar på bokstaven L
            • Visa alla användare vars för- och efternamn har samma begynnelsebokstav (ex Peter
            Parker, Bruce Banner, Janis Joplin)*/
            Console.CursorVisible = false;
            ConsoleKeyInfo input = new();
            do
            {
                Console.Clear();
                Console.WriteLine("Please select which question you would like the answer to:");
                Console.WriteLine("1) How many diffrent countries are represented?");
                Console.WriteLine("2) How many people are from Norden and Scandinavia, respectivly?");
                Console.WriteLine("3) Which is the most represented country?");
                Console.WriteLine("4) List first 10 users with a last name beginning with \"L\".");
                Console.WriteLine("5) List all users with name and last name beginning with the same letter.");
                Console.WriteLine("E or Escape) Exit application.");
                Helpers.ConsolePrintHelpers.Wait(false,true);
                input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.D1 or ConsoleKey.NumPad1: DiffrentCountries(); break;
                    case ConsoleKey.D2 or ConsoleKey.NumPad2: FromNordenVsScandinavia(); break;
                    case ConsoleKey.D3 or ConsoleKey.NumPad3: MostRepresentedCountry(); break;
                    case ConsoleKey.D4 or ConsoleKey.NumPad5: FirstTenLastNameStartWithL(); break;
                    case ConsoleKey.D5 or ConsoleKey.NumPad5: FirstLastAlliteration(); break;
                    default:break;
                }
            } while (input.Key != ConsoleKey.Escape && input.Key != ConsoleKey.E);
        }

        private static void FirstLastAlliteration()
        {
            string sql = "SELECT id,first_name,last_name FROM FakePeople WHERE LEFT(first_name,1) = LEFT(last_name,1) GROUP BY first_name,last_name,id";
            people = Query(sql);
            PrintPeopleList();
        }

        private static void PrintPeopleList()
        {
            int counter = 1;
            foreach (var person in people)
            {
                Console.WriteLine($"{counter,-3}) (id:{person.id,4}) {person.FullName}");
                counter++;
            }
            Helpers.ConsolePrintHelpers.Wait();
        }

        private static void FirstTenLastNameStartWithL()
        {
            string sql = "SELECT TOP 10 id,first_name,last_name FROM FakePeople WHERE LOWER(last_name) LIKE 'l%'";
            people = Query(sql);
            PrintPeopleList();
        }

        private static void MostRepresentedCountry()
        {
            string sql = "SELECT TOP 1 country,COUNT(country) FROM FakePeople GROUP BY country ORDER BY COUNT(country) DESC";
            people = Query(sql);
            Console.WriteLine($"\nMost represented country is: {people[0].country}");
            ConsolePrintHelpers.Wait();

        }

        private static void FromNordenVsScandinavia()
        {
            throw new NotImplementedException();
        }

        private static void DiffrentCountries()
        {
            throw new NotImplementedException();
        }
    }

}
