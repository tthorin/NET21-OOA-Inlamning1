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
            Console.Write("Press any key to continue");
            Console.ReadKey(true);
        }
        internal static void Wait(bool holdAtEnd=true,bool onlyDots=false)
        {
            if (!onlyDots) Console.Write("Press any key to continue");
            (int x, int y) = Console.GetCursorPosition();
            while (Console.KeyAvailable == false)
            {

                for (int i = 0; i < 2; i++)
                {
                    Console.SetCursorPosition(x, y);
                    if (i == 0)
                    {
                        Console.SetCursorPosition(x, y);
                        for (int j = 0; j < 5; j++)
                        {
                            Console.Write(".");
                            Thread.Sleep(50);
                        }
                    }
                    else
                    {
                        Console.SetCursorPosition(x, y);
                        for (int k = 0; k < 5; k++)
                        {
                            Console.Write(" ");
                            Thread.Sleep(50);
                        }
                    }
                }
            }
            Console.WriteLine();
            if (holdAtEnd)Console.ReadKey(true);
        }
    }
}
