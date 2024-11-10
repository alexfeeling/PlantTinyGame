namespace NByte.Transplanting
{
    public class FieldValue
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsObstacle { get; set; }
        public bool IsOrigin { get; set; }

        public FieldValue(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}