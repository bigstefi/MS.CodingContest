using MS.CodingContest.Helpers.Fence;
using MS.CodingContest.Interfaces.Fence;
using System.Collections.Generic;
using System.Net;

namespace MS.CodingContest.Fence
{
    public class YourNextFenceBuilderParticipant : IFenceBuilderParticipant
    {
        private string _fantasyName = "YourNextFantasyName";
        private double _fenceLength = 4; // dummy initial value
        private List<Cell> _trees = null;

        #region IFenceBuilderParticipant
        public string FantasyName
        {
            get
            {
                return _fantasyName;
            }
        }

        public double Encircle(IForestProvider forestProvider)
        {
            _trees = new List<Cell>(forestProvider.Trees);

            // ToDo: add your implementation here...

            return _fenceLength;
        }

        public List<int> ShowSolution()
        {
            List<int> indexesOuterTrees = new List<int>();

            // this is a dummy implementation
            for(int i=0;i<_trees.Count;i++)
            {
                indexesOuterTrees.Add(i);
            }

            FenceDisplayer.ShowSolution(FantasyName, _fenceLength, _trees, indexesOuterTrees);

            return indexesOuterTrees;
        }
        #endregion
    }
}