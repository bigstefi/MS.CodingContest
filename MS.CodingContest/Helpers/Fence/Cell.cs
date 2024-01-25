namespace MS.CodingContest.Helpers.Fence
{
    public class Cell
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            if(obj is Cell)
            {
                return Equals(obj as Cell);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Row + Column << 7;
        }

        public bool Equals(Cell cell)
        {
            if(cell == null)
            { 
                return false;
            }

            return Row == cell.Row && Column == cell.Column;
        }

        public override string ToString()
        {
            return @"({Row},{Column})";
        }
    }
}