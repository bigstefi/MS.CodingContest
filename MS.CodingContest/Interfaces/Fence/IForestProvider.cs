using MS.CodingContest.Helpers.Fence;
using System.Collections;
using System.Collections.Generic;

namespace MS.CodingContest.Interfaces.Fence
{
    public interface IForestProvider
    {
        string ForestFilePath { get; set; }
        IList<Cell> Trees { get; }
        void Reset();
    }
}