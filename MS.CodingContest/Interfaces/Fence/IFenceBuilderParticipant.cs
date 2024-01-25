using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace MS.CodingContest.Interfaces.Fence
{
    [InheritedExport]
    public interface IFenceBuilderParticipant
    {
        string FantasyName { get; }
        double Encircle(IForestProvider forestProvider);
        List<int> ShowSolution();
    }
}