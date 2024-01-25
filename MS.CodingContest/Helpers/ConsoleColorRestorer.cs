using System;

namespace MS.CodingContest.Helpers
{
    public class ColoredConsoleRestorer : IDisposable
    {
        ConsoleColor _foregroundColorOrig = Console.ForegroundColor;
        ConsoleColor _backgroundColorOrig = Console.BackgroundColor;

        internal ColoredConsoleRestorer()
        { }

        public void Dispose()
        {
            Console.ForegroundColor = _foregroundColorOrig; 
            Console.BackgroundColor = _backgroundColorOrig;
        }
    }
}