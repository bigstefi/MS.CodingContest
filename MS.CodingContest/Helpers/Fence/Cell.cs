namespace MS.CodingContest.Helpers.Fence
{
    /// <summary>
    /// Representation of the position of a tree within a forest (map)
    /// </summary>
    /// <remarks>
    /// Imagine an Excel sheet with rows and columns, so start top-left, stay in the first row and move right column by column, then move to the next row, a.s.o.
    /// This is why the representation is (row, column), not exactly as in Excel (B10 would practically be (9,1) as C# is using 0-based indexing)
    /// </remarks>
    public class Cell
    {
        #region Properties
        /// <summary>
        /// Row
        /// </summary>
        public int Row { get; set; }
        public int Column { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="row">
        /// Row (0-based)
        /// </param>
        /// <param name="column">
        /// Column (0-based)
        /// </param>
        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }
        #endregion

        #region Object overrides
        /// <summary>
        /// Equals generic implementation
        /// </summary>
        /// <param name="obj">
        /// The object to be compared with the current one
        /// </param>
        /// <returns>
        /// Flag reflecting identity (true) or difference (false)
        /// </returns>
        public override bool Equals(object obj)
        {
            if(obj is not Cell)
            {
                return false;
            }

            return Equals(obj as Cell);
        }

        public override int GetHashCode()
        {
            return Row + Column << 7;
        }

        /// <summary>
        /// An easy-to-understand representation of the data, useful in debugger and for logs
        /// </summary>
        /// <returns>
        /// Textual representation of the cell, (Row,Column) format
        /// </returns>
        public override string ToString()
        {
            return $"({Row},{Column})";
        }
        #endregion

        #region APIs
        /// <summary>
        /// Equality for objects of identical type (has sense to compare)
        /// </summary>
        /// <param name="cell">
        /// The onject to be compared with the current one
        /// </param>
        /// <returns>
        /// Flag reflecting identity (true) or difference (false)
        /// </returns>
        public bool Equals(Cell cell)
        {
            if (cell == null)
            {
                return false;
            }

            return Row == cell.Row && Column == cell.Column;
        }
        #endregion

        // ToDo: provide == operator override, IComparable, <, >, ...
    }
}