using MS.CodingContest.Interfaces;
using System;
using System.Diagnostics;

namespace MS.CodingContest.Helpers
{
    public class DurationMonitor : IDisposable
    {
        private Stopwatch _stopwatch = null;
        private string _logicalIdentifier = null;
        private IDurationListener _durationListener = null;

        public DurationMonitor(string loficalidentifier, IDurationListener durationListener = null)
        {
            _logicalIdentifier = loficalidentifier;
            _durationListener = durationListener;

            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose() 
        {
            _stopwatch.Stop();

            if(_durationListener != null)
            {
                _durationListener.AddDuration(_logicalIdentifier, _stopwatch.Elapsed);
            }
        }
    }
}