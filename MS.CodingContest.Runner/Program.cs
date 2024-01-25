using MS.CodingContest.Helpers;
using MS.CodingContest.Interfaces;
using MS.CodingContest.Runner.Fence;

namespace MS.CodingContest.Runner
{
    public class Program
    {
        static void Main(string[] args)
        {
            WarmUpTimers(10);

            FenceHelper.RunWithMefLoadedType(args);
        }

        public static void WarmUpTimers(int competitorsCount)
        {
            for(int i = 0; i< competitorsCount; ++i)
            {
                using (DurationMonitor dm = new DurationMonitor(i.ToString() + "WarmUpTimers"))
                {
                }
            }

            TestTimers();
        }

        private static void TestTimers()
        {
            IDurationListener listener = new DurationListener();
            string id;
            double result = 0;

            for(int i=1; i<=5; ++i)
            {
                for (int j = 1; j <= 10; ++j)
                {
                    id = $"{i}*{j}";
                    result = i * j;

                    using(DurationMonitor mon = new DurationMonitor(id, listener))
                    {
                        listener.AddResult(id, result);
                    }
                }

            }
        }
    }
}
