namespace OOA_Inlamning1.Helpers
{
    using System;
    using System.Collections.Generic;
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
        internal static void Wait(bool holdAtEnd=true,bool onlyDots=false)
        {
            if (!onlyDots) Console.Write("Press any key to continue");
            (int x, int y) = Console.GetCursorPosition();
            while (Console.KeyAvailable == false)
            {

                for (int i = 0; i < 10; i++)
                {
                    if(i==0 || i==5)Console.SetCursorPosition(x, y);
                    if (i < 5) Console.Write(".");
                    else Console.Write(" ");
                    Thread.Sleep(50);
                }
            }
            Console.WriteLine();
            if (holdAtEnd)Console.ReadKey(true);
        }
        internal static void PrintPeopleList(List<Person> people)
        {
            int counter = 1;
            foreach (var person in people)
            {
                Console.WriteLine($"{counter,-3}) (id:{person.id,4}) {person.FullName}");
                counter++;
            }
            Hold();
        }
    }
}
