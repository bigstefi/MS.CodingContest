using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MS.CodingContest.Helpers.Fence
{
    public class FenceDisplayer
    {
        public static void ShowSolution(string fantasyName, double fenceLength, IList<Cell> trees, IList<int> indexes)
        {
            Console.WriteLine(fantasyName);
            Console.WriteLine(fenceLength);

            if(fenceLength == 0)
            {
                return;
            }

            if(indexes == null || trees == null)
            {
                return;
            }

            List<Cell> treesBorder = GetBorderTrees(trees, indexes);

            if (trees.Count <= 500) // display the map only if not too many trees
            {
                int rowMin = 0;
                int columnMin = 0;
                int rowMax = trees.Select(t => t.Row).Max();
                int columnMax = trees.Select(t => t.Column).Max();

                for(int row = rowMin; row<=rowMax; ++row)
                {
                    for(int column = columnMin; column<=columnMax; ++column)
                    {
                        if(treesBorder.Contains(new Cell(row, column)))
                        {
                            ColoredConsole.WriteHappy("O");
                        }
                        else if(trees.Contains(new Cell(row, column)))
                        {
                            Console.Write("o");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }

                    Console.WriteLine();
                }
            }

            double distance = 0;
            double fenceLengthCalculated = 0;

            for(int i=0; i<treesBorder.Count-1; ++i)
            {
                distance = GetDistance(treesBorder[i], treesBorder[i + 1]);
                fenceLengthCalculated += distance;
            }

            distance = GetDistance(treesBorder[treesBorder.Count - 1], treesBorder[0]);
            fenceLengthCalculated += distance;

            Console.WriteLine(fenceLengthCalculated);

            if(!fenceLength.Equals(fenceLengthCalculated, 5))
            {
                ColoredConsole.WriteLineWarning($"WARNING: probably incorrect indexes provided. Distance: {fenceLength,20:0.0000000000000}, Calculated Distance: {fenceLengthCalculated,20:0.0000000000000}");
            }

            if(indexes.Count != indexes.Distinct().Count())
            {
                ColoredConsole.WriteLineError($"ERROR: same tree is used several times");
            }
        }

        public static double GetDistance(Cell tree1, Cell tree2)
        {
            return Math.Sqrt((tree2.Column - tree1.Column) * (tree2.Column - tree1.Column) + (tree2.Row - tree1.Row) * (tree2.Row - tree1.Row));
        }

        public static List<Cell> GetBorderTrees(IList<Cell> trees, IList<int> indexes)
        {
            List<Cell> treesBorder = new List<Cell>(indexes.Count);

            foreach(int idx in indexes)
            {
                treesBorder.Add(trees[idx]);    
            }

            return treesBorder;
        }
    }
}