using MS.CodingContest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace MS.CodingContest.Helpers
{
    public class DurationListener : IDurationListener
    {
        private static readonly string _key = "DurationListener_Initializer";
        private static readonly int _size = 128;
        private Dictionary<string, List<TimeSpan>> _durations = new Dictionary<string, List<TimeSpan>>(2*_size);
        private Dictionary<string, double> _results = new Dictionary<string, double>(2 * _size);
        private Dictionary<string, Tuple<double, TimeSpan>> _summary = new Dictionary<string, Tuple<double, TimeSpan>>();

        public Dictionary<string, Tuple<double, TimeSpan>> Summary
        {
            get
            {
                CreateSummary();

                return _summary;
            }
        }

        public DurationListener() 
        {
            for(int i=0; i<2*_size; ++i)
            {
                this.AddDuration(_key + i.ToString(), new TimeSpan(1));
                this.AddResult(_key + i.ToString(), 1.0);
            }

            _durations.Clear();
            _results.Clear();
        }

        public void AddDuration(string logicalIdentifier, TimeSpan duration)
        {
            lock(_durations)
            {
                if(!_durations.ContainsKey(logicalIdentifier))
                {
                    _durations.Add(logicalIdentifier, new List<TimeSpan>());
                }

                _durations[logicalIdentifier].Add(duration);
            }
        }

        public void AddResult(string logicalIdentifier, double result)
        {
            lock(_results)
            {
                if (_results.ContainsKey(logicalIdentifier))
                {
                    return;
                }

                _results.Add(logicalIdentifier, result);
            }
        }

        private void CreateSummary()
        {
            TimeSpan averageDuration = new TimeSpan(0);
            Dictionary<string, TimeSpan> averageDurations = new Dictionary<string, TimeSpan>();

            lock (_durations)
            {
                foreach (string key in _durations.Keys)
                {
                    if (key.StartsWith(_key))
                    {
                        continue;
                    }

                    lock (_summary)
                    {
                        if (_summary.ContainsKey(key))
                        {
                            continue;
                        }

                        averageDuration = new TimeSpan(0);

                        _durations[key].ForEach(value => averageDuration += value);
                        averageDuration /= _durations[key].Count;

                        averageDurations.Add(key, averageDuration);
                    }
                }
            }

            var orderedAverageDurations = averageDurations.OrderBy(SecurityElement => SecurityElement.Value);

            lock(_summary)
            {
                lock(_results)
                {
                    foreach(var orderedAverageDuration in orderedAverageDurations)
                    {
                        _summary.Add(orderedAverageDuration.Key, new Tuple<double, TimeSpan>(_results[orderedAverageDuration.Key], orderedAverageDuration.Value));
                    }
                }
            }
        }

        public override string ToString()
        {
            lock(_summary)
            {
                CreateSummary();

                StringBuilder sb = new StringBuilder();

                foreach(string key in _summary.Keys)
                {
                    if(_summary[key].Item1 == 0) // remmove candidates, whcih did not complete the execution
                    {
                        sb.AppendFormat($"\r\n{key,-60}");
                    }
                    else
                    {
                        sb.AppendFormat($"\r\n{key,-60} {_summary[key].Item1,12} { _summary[key].Item2,20}");
                    }
                }

                return sb.ToString();
            }
        }
    }
}