using MS.CodingContest.Helpers;
using MS.CodingContest.Interfaces;
using MS.CodingContest.Runner.Fence;
using System;
using System.Threading;

namespace MS.CodingContest.Runner
{
    /// <summary>
    /// This is the framework for running the implementations provided for the coding contest problem 
    /// and comparing the solutions either based on execution time or the score obtained when competing against each-other.
    /// 
    /// 5. 2024 - build a fence around a forest. Trees are specified as cells (row, column pairs). 
    ///    The task is finding the outer trees, calculate the fence length (minimal length is the winner)
    ///    and provide the indexes of the trees which should be used for placing the fence 
    ///    (minimum number is better, so in case of 3 co-linear trees, the middle one could be left out).
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            WarmUpTimers(10);

            // 5. 2024 - build a fence around a forest
            FenceHelper.RunWithMefLoadedType(args);
        }

        #region Helpers
        /// <summary>
        /// Workaround for starting the timers in advance, not when the competition is running.
        /// It looked like without this, the first time measurements show a high deviation
        /// </summary>
        /// <param name="competitorsCount">
        /// Number of expected competitors (could be any number)
        /// </param>
        private static void WarmUpTimers(int competitorsCount)
        {
            for(int i = 0; i< competitorsCount; ++i)
            {
                using (DurationMonitor dm = new DurationMonitor(i.ToString() + "WarmUpTimers"))
                {
                }
            }

            TestTimers();
        }

        /// <summary>
        /// Test method for showing how time measurement is done within the framework.
        /// Just activate the lines commented out and see how the results are ordered based on execution time
        /// </summary>
        private static void TestTimers()
        {
            IDurationListener listener = new DurationListener();
            string id;
            double result = 0;
            Random rnd = new Random(149);

            for(int i=1; i<=5; ++i)
            {
                for (int j = 1; j <= 10; ++j)
                {
                    id = $"{i}*{j}";

                    using (DurationMonitor mon = new DurationMonitor(id, listener))
                    {
                        // simulate logic which needs some execution time
                        result = i * j;
                        //Thread.Sleep(rnd.Next(50, 100));

                        listener.AddResult(id, result);

                        mon.ToString();
                    }
                }
            }

            //string contestResult = (listener as DurationListener).ToString();
            //Console.WriteLine(contestResult);
        }
        #endregion
    }
}
