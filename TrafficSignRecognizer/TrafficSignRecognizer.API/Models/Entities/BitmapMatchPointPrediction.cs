using Microsoft.ML.Runtime.Api;

namespace TrafficSignRecognizer.API.Models.Entities
{
    public class BitmapMatchPointPrediction
    {

        [ColumnName("PredictedLabel")]
        public uint SelectedClusterId;

        [ColumnName("Score")]
        public float[] Distance;
    }
}
