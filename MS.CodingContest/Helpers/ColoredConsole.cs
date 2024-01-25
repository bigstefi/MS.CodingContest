using System;

namespace MS.CodingContest.Helpers
{
    public class ColoredConsole
    {
        public static void WriteLine(string format, params object[] args)
        {
            using(ColoredConsoleRestorer ccr = new ColoredConsoleRestorer())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.WriteLine(format, args);
            }
        }

        public static void WriteLineError(string format, params object[] args) 
        {
            using(ColoredConsoleRestorer ccr = new ColoredConsoleRestorer())
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                Console.WriteLine(format, args);
            }
        }

        public static void WriteLineWarning(string format, params object[] args)
        {
            using (ColoredConsoleRestorer ccr = new ColoredConsoleRestorer())
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                Console.WriteLine(format, args);
            }
        }

        public static void WriteLineHappy(string format, params object[] args)
        {
            using (ColoredConsoleRestorer ccr = new ColoredConsoleRestorer())
            {
                Console.ForegroundColor = ConsoleColor.Magenta;

                Console.WriteLine(format, args);
            }
        }

        public static void WriteHappy(string format, params object[] args)
        {
            using (ColoredConsoleRestorer ccr = new ColoredConsoleRestorer())
            {
                Console.ForegroundColor = ConsoleColor.Magenta;

                Console.Write(format, args);
            }
        }
    }
}