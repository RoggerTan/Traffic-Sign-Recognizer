using Microsoft.ML.Runtime.Api;

namespace TrafficSignRecognizer.API.Models.Entities
{
    public class MatchPointsClusterData
    {
        [Column("0")]
        [ColumnName("Label")]
        public string Id;

        [Column("1")]
        [ColumnName("Quantity")]
        public float PointQuantity;
    }
}
