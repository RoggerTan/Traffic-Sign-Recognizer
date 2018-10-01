namespace TrafficSignRecognizer.Interfaces.Entities
{
    public class MatrixToken
    {
        public string Value { get; set; }
        public int RowCount { get; set; }
        public int ColCount { get; set; }
        public int CurrentRow { get; set; }

        public class MatrixRow
        {
            public int[] Value { get; set; }
        }
    }
}
