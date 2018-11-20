using Accord;

namespace TrafficSignRecognizer.API.Models.Entities
{
    public class BitmapMatchPoints
    {
        public BitmapMatchPoints(string id, IntPoint[] points)
        {
            Points = points;
            Id = id;
        }

        public int Quantity => Points.Length;
        public string Id { get; set; }
        public IntPoint[] Points { get; }
    }
}
