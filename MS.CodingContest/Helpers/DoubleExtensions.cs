using System;

namespace MS.CodingContest.Helpers
{
    public static class DoubleExtensions
    {
        public static bool Equals(this double d1, double d2, int precision = 8)
        {
            return Math.Abs(d1 - d2) < Math.Pow(10, -precision);
        }

        public static int CompareTo(this double first, double second, int precision = 8)
        {
            if(first.Equals(second, precision))
            {
                return 0;
            }
            else
            {
                int ret = first > second ? 1 : -1;

                return ret;
            }
        }
    }
}