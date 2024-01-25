using MS.CodingContest.Helpers;
using MS.CodingContest.Helpers.Fence;
using MS.CodingContest.Interfaces;
using MS.CodingContest.Interfaces.Fence;
using MS.CodingContest.Runner.Mef;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MS.CodingContest.Runner.Fence
{
    /// <summary>
    /// Support for 
    /// - loading the map of the contest
    /// - loading the MEF implementations
    /// - executing the logic (several times)
    /// - ordering the results
    /// </summary>
    public static class FenceHelper
    {
        #region Member variables
        /// <summary>
        /// Counter for the number of executions in order to have better statistical results
        /// </summary>
        private static int _cycles = 1;
        #endregion

        #region APIs
        /// <summary>
        /// Main execution logic of the contest
        /// </summary>
        /// <param name="args">
        /// Command line arguments received from teh main program
        /// </param>
        /// <seealso cref="IFenceBuilderParticipant"/>
        public static void RunWithMefLoadedType(string[] args)
        {
            ForestProviderContainer forestProviderContainer = CreateForestProviderContainer(args);
            IEnumerable<IFenceBuilderParticipant> participants = MefHelper.GetMefParticipants<IFenceBuilderParticipant>(args);
            IDurationListener durationListener = new DurationListener();
            string results = string.Empty;

            foreach(IForestProvider forestProvider in forestProviderContainer.ForestProviders)
            {
                double referenceFenceLength = 0;
                if(forestProvider is IFenceSolutionProvider)
                {
                    referenceFenceLength = (forestProvider as IFenceSolutionProvider).ReferenceFenceLength;

                    Console.WriteLine($"\r\n\r\n{Path.GetFileName(forestProvider.ForestFilePath)} - ReferenceFenceLength: {referenceFenceLength}");
                }

                double fenceLength = 0;
                double fenceLengthMin = double.MaxValue;

                foreach(IFenceBuilderParticipant participant in participants)
                {
                    try
                    {
                        fenceLength = Run(participant, forestProvider, durationListener);
                        
                        if(fenceLength.CompareTo(referenceFenceLength, 8) < 0)
                        {
                            ColoredConsole.WriteLineHappy($"Potential winner {participant.FantasyName} for {Path.GetFileName(forestProvider.ForestFilePath)} with fence length {fenceLength,20:0.0000000000000} shorter than Reference fence length {referenceFenceLength,20:0.0000000000000}");
                        }
                        if(fenceLength.CompareTo(fenceLengthMin, 8) < 0)
                        {
                            fenceLengthMin = fenceLength;
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                if(fenceLengthMin.CompareTo(referenceFenceLength, 8) < 0)
                {
                    ColoredConsole.WriteLineHappy($"Best fence length: {fenceLengthMin}");
                }

                ColoredConsole.WriteLine("\r\n*********************************************");
                ColoredConsole.WriteLine("*****          PARTIAL SUMMARY          *****");
                ColoredConsole.WriteLine("*********************************************");
                results = durationListener.ToString();
                ColoredConsole.WriteLine(results);
                ColoredConsole.WriteLine("\r\n*********************************************");
            }

            ColoredConsole.WriteLine("\r\n*********************************************");
            ColoredConsole.WriteLine("*****              SUMMARY              *****");
            ColoredConsole.WriteLine("*********************************************");
            results = durationListener.ToString();
            ColoredConsole.WriteLine(results);
            ColoredConsole.WriteLine("\r\n*********************************************");

            ScoreCalculator scoreCalculator = new ScoreCalculator(forestProviderContainer, (durationListener as DurationListener).Summary);
            ColoredConsole.WriteLine(scoreCalculator.ToString());
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Loads every map available (IForestProvider) and converts them into a map collection
        /// </summary>
        /// <param name="args">
        /// Command line arguments received form the main program
        /// </param>
        /// <returns>
        /// Collection of contest maps
        /// </returns>
        /// <seealso cref="ForestProviderContainer"/>
        internal static ForestProviderContainer CreateForestProviderContainer(string[] args)
        {
            string pathsDirectory = Path.Combine(Environment.CurrentDirectory, @"TestData");

            if(args != null && args.Length > 0)
            {
                pathsDirectory = args[0];
            }

            ForestProviderContainer forestProviderContainer = new ForestProviderContainer(pathsDirectory);

            return forestProviderContainer;
        }

        /// <summary>
        /// Execute the contest logic for a given competitor and a concrete map
        /// </summary>
        /// <param name="participant">
        /// The competitor executiong the contest logic on the given map
        /// </param>
        /// <param name="forestProvider">
        /// Abstraction of the contest map, competitors can fetch the data via this interface
        /// </param>
        /// <param name="durationListener">
        /// A central instance of the time measurement logic
        /// </param>
        /// <returns></returns>
        internal static double Run(IFenceBuilderParticipant participant, IForestProvider forestProvider, IDurationListener durationListener)
        {
            double fenceLengthTemp = 0;
            double fenceLength = 0;
            string durationIdentifier = Path.GetFileName(forestProvider.ForestFilePath) + " - " + participant.FantasyName;
            bool slowParticipant = false; // organizer knows with the help of the weekly test runs which implementations are slow, so they would be marked as such for the maps they cannot process fast enough

            Console.WriteLine();
            Console.WriteLine(durationIdentifier + $" ({DateTime.Now})"); // ToDo: why does it add spaces after year, month and day?

            try
            {
                for (int cycle = 0; cycle < _cycles; ++cycle)
                {
                    forestProvider.Reset();

                    // workaround ffor filtering out those implementations which are suboptimal for large maps
                    if(!slowParticipant)
                    {
                        using(DurationMonitor durationMonitor = new DurationMonitor(durationIdentifier, durationListener))
                        {
                            fenceLength = participant.Encircle(forestProvider);
                        }
                    }
                    else
                    {
                        using(DurationMonitor durationMonitor = new DurationMonitor(durationIdentifier, durationListener))
                        {
                            fenceLength = participant.GetHashCode(); // providing a random value, probably an invalid one, so that the result would be invalidated while verifying it
                        }
                    }

                    fenceLengthTemp = fenceLength;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"ERROR: {e}");

                fenceLength = Math.Abs(e.HResult); // providing some value, probably an invalid one, so that the result would be invalidated while verifying it
            }

            durationListener.AddResult(durationIdentifier, fenceLength);

            if(!slowParticipant)
            {
                try
                {
                    List<int> solution = participant.ShowSolution();

                    // ToDo: remove the display logic from the ShowSolution method and invoke it here (should not rely on the competitor displaying the map)
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return fenceLength;
        }
        #endregion
    }
}