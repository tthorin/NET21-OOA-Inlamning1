﻿namespace OOA_Inlamning1.Helpers
{
    using System;
    using System.Collections.Generic;
using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class ConsolePrintHelpers
    {
        static internal void Hold()
        {
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }
        internal static void Wait(bool holdAtEnd = true, bool onlyDots = false)
        {
            if (!onlyDots) Console.Write("Press any key to continue");
            (int x, int y) = Console.GetCursorPosition();
            while (Console.KeyAvailable == false)
            {

                for (int i = 0; i < 10; i++)
                {
                    if (i == 0 || i == 5) Console.SetCursorPosition(x, y);
                    if (i < 5) Console.Write(".");
                    else Console.Write(" ");
                    Thread.Sleep(50);
                }
            }
            Console.WriteLine();
            if (holdAtEnd) Console.ReadKey(true);
        }
        internal static void PrintPeopleList(List<Person> people)
        {
            if (people.Count == 0)
            {
                Console.WriteLine("Nothing matched your query.");
                Hold();
                return;
            }
            int counter = 1;
            foreach (var person in people)
            {
                Console.WriteLine($"{counter,-3}) (id:{person.id,4}) {person.FullName}");
                counter++;
            }
            Hold();
        }
        internal static void PrintPeopleFullInfoList(List<Person> people)
        {
            if (people.Count == 0)
            {
                Console.WriteLine("Nothing matched your query.");
                Hold();
                return;
            }

            int counter = 1;
            Console.WriteLine($"{"",5} {"Id",-4} {"First name",-10} {"Last name",-14} {"Email",-30} {"Username",-13} {"Password",-13} {"Country",-15}");
            foreach (var person in people)
            {
                Console.WriteLine($"{counter,-3}) {person.AllInfo}");
                counter++;
            }
            Hold();
        }
        internal static void PrintDtSingleColumns(string columnName, DataTable dt)
        {
            Console.WriteLine("\n"+columnName+":");
            Console.WriteLine(new String('-',columnName.Length+1));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0] != null) Console.WriteLine(row[0]);
                }
                Console.WriteLine();
            }
            else Console.WriteLine("\nNothing found.");
            Hold();
        }
        internal static string GetUserString(bool toLower = false)
        {
            Console.CursorVisible = true;
            var input = Console.ReadLine().Trim();
            if (toLower) input = input.ToLower();
            Console.CursorVisible=false;
            return input;
        }
    }
}

