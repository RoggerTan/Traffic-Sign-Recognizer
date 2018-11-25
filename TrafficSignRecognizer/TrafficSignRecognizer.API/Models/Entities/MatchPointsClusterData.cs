using Microsoft.ML.Runtime.Api;

namespace TrafficSignRecognizer.API.Models.Entities
{
    public class MatchPointsClusterData
    {
        [Column("0", "Label")]
        public string Id { get; set; }

        [Column("1", "Quantity")]
        public float PointQuantity { get; set; }
    }
}
