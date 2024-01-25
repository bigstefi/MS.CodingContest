using System;

namespace MS.CodingContest.Interfaces
{
    public interface IDurationListener
    {
        void AddDuration(string logicalIdentifier, TimeSpan duration);
        void AddResult(string logicalIdentifier, double result);
    }
}