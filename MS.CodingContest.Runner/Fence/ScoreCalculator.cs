using MS.CodingContest.Helpers;
using MS.CodingContest.Helpers.Fence;
using MS.CodingContest.Interfaces.Fence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.CodingContest.Runner.Fence
{
    internal class ScoreCalculator
    {
        private ForestProviderContainer _forestProviderContainer = null;
        private Dictionary<string, Tuple<double, TimeSpan>> _summary = null;
        private double _toleranceAccepted = 0.1;
        private Dictionary<string, double> _summaryPoints = new Dictionary<string, double>();
        
        public ScoreCalculator(ForestProviderContainer forestProviderContainer, Dictionary<string, Tuple<double, TimeSpan>> summary)
        {
            _forestProviderContainer = forestProviderContainer;
            _summary = summary;
        }

        internal void CalculateScore()
        {
            if(_forestProviderContainer != null)
            {
                CalculateScore(_forestProviderContainer);
            }
        }

        private void CalculateScore(ForestProviderContainer forestProviderContainer)
        {
            Console.WriteLine();

            ColoredConsole.WriteLine("\r\n*********************************************");
            ColoredConsole.WriteLine("*****         RESULT VALIDATION         *****");
            ColoredConsole.WriteLine("*********************************************\r\n");

            List<string> wrongSummaryKeys = new List<string>();

            foreach(string key in forestProviderContainer.ForestProviders.Select(provider => Path.GetFileName(provider.ForestFilePath)))
            { 
                foreach(string summaryKey in _summary.Keys)
                {
                    if(!summaryKey.StartsWith(key))
                    {
                        continue;
                    }

                    IFenceSolutionProvider prov = forestProviderContainer.ForestProviders.Where(provider => Path.GetFileName(provider.ForestFilePath) == key).First() as IFenceSolutionProvider;

                    double tolerance = (double)(Math.Abs(_summary[summaryKey].Item1 - prov.ReferenceFenceLength)) / (double)prov.ReferenceFenceLength;
                    if(tolerance > _toleranceAccepted) 
                    {
                        if (_summary[summaryKey].Item1 != 0)
                        {
                            ColoredConsole.WriteLineError($"Disqualifying {summaryKey,-40} because of high deviation {tolerance,12:0.00} (> {_toleranceAccepted,4:0.00}). FenceLength: {_summary[summaryKey].Item1,12} vs ReferenceFenceLength: {prov.ReferenceFenceLength,8}");
                        }

                        wrongSummaryKeys.Add(summaryKey);
                    }
                }
            }

            foreach(string wrongSummaryKey in wrongSummaryKeys)
            {
                _summary.Remove(wrongSummaryKey);
            }

            ColoredConsole.WriteLine("\r\n*********************************************\r\n");
            
            double relevance = 0;
            double relevancePrevious = 0;
            double weight = 1;
            int point = 10;
            string participant = string.Empty;

            foreach(string summaryKey in _summary.Keys)
            {
                participant = summaryKey.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)[1];
                if(!_summaryPoints.ContainsKey(participant))
                {
                    _summaryPoints.Add(participant, 0.0);
                }

                relevance = double.Parse(summaryKey.Substring(0, 1));
                if(relevance != relevancePrevious)
                {
                    // reset everything
                    relevancePrevious = relevance;
                    point = 10;
                    weight = 1 + 0.1 * (double)relevance;
                }
                else
                {
                    if(point == 10 || point == 8)
                    {
                        point -= 2;
                    }
                    else if(point < 1)
                    {
                        point = 0;
                    }
                    else
                    {
                        --point;
                    }
                }

                _summaryPoints[participant] += (double)point * weight;
            }

            Dictionary<string, double> summaryPointsTemp = new Dictionary<string, double>();

            var summaryPointsOrdered = _summaryPoints.OrderByDescending(elem => elem.Value);

            foreach(var ordered in summaryPointsOrdered)
            {
                summaryPointsTemp.Add(ordered.Key, ordered.Value);
            }

            _summaryPoints = new Dictionary<string, double>(summaryPointsTemp);
        }

        public override string ToString()
        {
            CalculateScore();

            StringBuilder sb = new StringBuilder();

            sb.Append("\r\n*********************************************");
            sb.Append("\r\n*****               SCORE               *****");
            sb.Append("\r\n*********************************************\r\n\r\n");

            foreach(string participant in _summaryPoints.Keys)
            {
                sb.AppendFormat($"{participant,-20} \t {_summaryPoints[participant],20:0.00}\r\n");
            }

            sb.Append("\r\n*********************************************\r\n");

            return sb.ToString();
        }
    }
}
