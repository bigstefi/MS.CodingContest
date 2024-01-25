using System;

namespace MS.CodingContest.Interfaces
{
    /// <summary>
    /// Interface to a container storing the results and execution times of the competitors on a given map
    /// </summary>
    public interface IDurationListener
    {
        #region APIs
        /// <summary>
        /// Adds one execution time of a given competitor
        /// </summary>
        /// <param name="logicalIdentifier">
        /// Identifier of the competitor & map
        /// </param>
        /// <param name="duration">
        /// Execution time of the algorithm
        /// </param>
        void AddDuration(string logicalIdentifier, TimeSpan duration);

        /// <summary>
        /// Stores the calculated result of a given competitor on a concrete map
        /// </summary>
        /// <param name="logicalIdentifier">
        /// Identifier of the competitor & map
        /// </param>
        /// <param name="result">
        /// The value calculated by the algorithm implemented by the competitor
        /// </param>
        void AddResult(string logicalIdentifier, double result);
        #endregion
    }
}