namespace MS.CodingContest.Interfaces.Fence
{
    /// <summary>
    /// Extending the information of the forest-map with the currently known fence length
    /// </summary>
    public interface IFenceSolutionProvider
    {
        #region Properties
        /// <summary>
        /// A reference solution of the current problem (map)
        /// </summary>
        double ReferenceFenceLength
        { 
            get; set; 
        }
        #endregion
    }
}