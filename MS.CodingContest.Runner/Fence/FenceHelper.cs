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
    public static class FenceHelper
    {
        private static int _cycles = 1;

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

        public static ForestProviderContainer CreateForestProviderContainer(string[] args)
        {
            string pathsDirectory = Path.Combine(Environment.CurrentDirectory, @"TestData");

            if(args != null && args.Length > 0)
            {
                pathsDirectory = args[0];
            }

            ForestProviderContainer forestProviderContainer = new ForestProviderContainer(pathsDirectory);

            return forestProviderContainer;
        }

        private static double Run(IFenceBuilderParticipant participant, IForestProvider forestProvider, IDurationListener durationListener)
        {
            double fenceLengthTemp = 0;
            double fenceLength = 0;
            string durationIdentifier = Path.GetFileName(forestProvider.ForestFilePath) + " - " + participant.FantasyName;
            bool slowParticipant = false;

            Console.WriteLine();
            Console.WriteLine(durationIdentifier + $" ({DateTime.Now})");

            try
            {
                for (int cycle = 0; cycle < _cycles; ++cycle)
                {
                    forestProvider.Reset();

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
                            fenceLength = 0;
                        }
                    }

                    fenceLengthTemp = fenceLength;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"ERROR: {e}");

                fenceLength = Math.Abs(e.HResult);
            }

            durationListener.AddResult(durationIdentifier, fenceLength);

            if(!slowParticipant)
            {
                try
                {
                    List<int> solution = participant.ShowSolution();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return fenceLength;
        }
    }
}