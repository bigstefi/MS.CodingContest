using MS.CodingContest.Helpers.Fence;
using System.Collections;
using System.Collections.Generic;

namespace MS.CodingContest.Interfaces.Fence
{
    /// <summary>
    /// Abstraction of the map with the trees on it.
    /// </summary>
    /// <remarks>
    /// Its implementation will deserialize a text file which contains the tree information in a visually imaginable format.
    /// See some simple examples in the TestData folder
    /// </remarks>
    /// <seealso cref="Cell"/>
    public interface IForestProvider
    {
        #region Properties
        /// <summary>
        /// The file path to the tree-map
        /// </summary>
        /// <remarks>
        /// Useful for debugging, just to know which file contains the tree-information being processed by your algorithm
        /// </remarks>
        string ForestFilePath { get; set; }
        
        /// <summary>
        /// The position of the trees on the map in (row,column) format
        /// </summary>
        /// <seealso cref="Cell"/>
        IList<Cell> Trees { get; }
        #endregion

        #region APIs
        /// <summary>
        /// Resetting the in case the file should be re-read
        /// </summary>
        /// <remarks>
        /// Invoked by the framework, not needed at all for the competitors
        /// </remarks>
        void Reset();
        #endregion
    }
}