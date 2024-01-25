using MS.CodingContest.Helpers.Fence;
using MS.CodingContest.Interfaces.Fence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MS.CodingContest.Helpers.Fence
{
    public class ForestProvider : IForestProvider, IFenceSolutionProvider
    {
        protected List<Cell> _trees = null;

        #region IForestProvider
        public string ForestFilePath
        {
            get; set;
        }

        public IList<Cell> Trees
        {
            get { return _trees; }
        }
        public void Reset()
        {
        }
        #endregion

        #region IFenceSolutionProvider
        public double ReferenceFenceLength
        {
            get; set;
        } = double.MaxValue;
        #endregion

        public ForestProvider(string forestFilePath)
        {
            if (!File.Exists(forestFilePath))
            {
                throw new FileNotFoundException($"Could not find file {forestFilePath}", forestFilePath);
            }

            ForestFilePath = forestFilePath;

            try
            {
                string forestContent = File.ReadAllText(ForestFilePath);
                string[] treeTokens = forestContent.Split(new char[] { ' ', ',', ';', '|', '(', ')', '[', ']', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                bool hasReferenceFenceLength = treeTokens.Length % 2 == 1;
                if (hasReferenceFenceLength)
                {
                    ReferenceFenceLength = double.Parse(treeTokens[treeTokens.Length - 1]);
                }

                _trees = new List<Cell>(treeTokens.Length / 2);
                for (int coordinates = 0; coordinates < treeTokens.Length - (hasReferenceFenceLength ? 1 : 0); coordinates += 2)
                {
                    _trees.Add(new Cell(int.Parse(treeTokens[coordinates + 0]), int.Parse(treeTokens[coordinates + 1])));
                }

                if (_trees.Distinct().Count() < _trees.Count)
                {
                    string message = $"Invalid Forest file {ForestFilePath}. There are trees with teh same coordinates";

                    foreach (Cell t in _trees.Distinct().ToList())
                    {
                        _trees.Remove(t);
                    }
                    foreach (Cell t in _trees)
                    {
                        Console.WriteLine(t);
                    }

                    throw new InvalidDataException(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                throw;
            }
        }
    }
}