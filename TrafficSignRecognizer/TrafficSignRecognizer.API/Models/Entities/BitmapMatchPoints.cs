using Accord;

namespace TrafficSignRecognizer.API.Models.Entities
{
    public class BitmapMatchPoints
    {
        public string Id { get; set; }
        public IntPoint[] Points { get; set; }

        public MatchPointsClusterData ToClusterData()
        {
            return new MatchPointsClusterData
            {
                Id = Id,
                PointQuantity = Points.Length
            };
        }
    }
}
