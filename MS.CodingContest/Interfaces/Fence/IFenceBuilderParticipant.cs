using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace MS.CodingContest.Interfaces.Fence
{
    /// <summary>
    /// Interface to be implemented (inherit the competitor class from this) in order to have a 'competitor'
    /// </summary>
    /// <remarks>
    /// The framework will automatically handle the dll with the help of MEF, as the attribute on the interface makes it MEF-visible
    /// </remarks>
    [InheritedExport]
    public interface IFenceBuilderParticipant
    {
        #region Properties
        /// <summary>
        /// An identifier of your competitor. Nobody should know based on the results who has provided the solution. 
        /// It should be only you knowing how you rank in the test runs. The real owner will be revealed during the contest.
        /// </summary>
        string FantasyName { get; }
        #endregion

        #region APIs
        /// <summary>
        /// The implementation of the algorithm finding the outer trees of the forest and 
        /// calculating the length of the fence around it.
        /// </summary>
        /// <remarks>
        /// Execution time is measured by teh framework
        /// </remarks>
        /// <param name="forestProvider">
        /// Interface exposing APIs for fetching the position of the trees
        /// </param>
        /// <returns>
        /// The length of the fence around the forest
        /// </returns>
        /// <seealso cref="IForestProvider"/>
        double Encircle(IForestProvider forestProvider);

        /// <summary>
        /// In order to validate the results (especially in the case of better solutions than the reference length)
        /// you will be asked to specify exactly the trees holding the fence. The order should be clockwise or counter-clockwise
        /// <remarks>
        /// </remarks>
        /// The framework will validate that the length returned and the one which could be obtained with the trees specified with their indexes
        /// </summary>
        /// <returns>
        /// The indexes of the trees (from the original list) to be considered for the fence length calculation
        /// </returns>
        List<int> ShowSolution();
        #endregion
    }
}