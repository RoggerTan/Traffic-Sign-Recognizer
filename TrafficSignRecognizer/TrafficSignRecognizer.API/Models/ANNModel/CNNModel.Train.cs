using ConvNetSharp.Volume;
using System.Threading.Tasks;

namespace TrafficSignRecognizer.API.Models.ANNModel.Utils
{
    public partial class CNNModel
    {
        public async Task BeginTraining(int totalBatchSize)
        {
            var loopCount = 0;
            do
            {
                var trainSample = await _DataSet.Train.NextBatch(_BatchSize, _Env);
                Train(trainSample.Item1, trainSample.Item2);
            } while (loopCount < totalBatchSize/_BatchSize);
        }

        private void Train(Volume<double> x, Volume<double> y)
        {
            _Trainer.Train(x, y);
        }
    }
}
