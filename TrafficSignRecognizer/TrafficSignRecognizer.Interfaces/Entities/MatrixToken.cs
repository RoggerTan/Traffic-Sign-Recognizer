namespace TrafficSignRecognizer.Interfaces.Entities
{
    public class MatrixToken
    {
        public string Value { get; set; }
        public int RowCount { get; set; }

        public class MatrixRow
        {
            public int[] Value { get; set; }
            public bool EndRow { get; set; }
        }
    }
}
