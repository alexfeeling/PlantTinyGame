namespace NByte.Transplanting
{
    public class FieldValue
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsBlocked { get; set; }
        public int Step { get; set; }

        public FieldValue(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}