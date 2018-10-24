using ConvNetSharp.Volume;
using System.Threading.Tasks;

namespace TrafficSignRecognizer.API.Models.ANNModel.Utils
{
    public partial class CNNModel
    {
        public async Task BeginTraining(int totalBatchSize)
        {
            IsTrained = true;

            if (_DataSets == null)
            {
                _DataSets = new DataSets();
                _DataSets.Load(_TrainingPath, _TestingPath, _Env, _ImgWidth, _ImgHeight);
            }

            var loopCount = 0;

            do
            {
                var trainSample = await _DataSets.Train.NextBatch(_BatchSize, _Env);
                Train(trainSample.Item1, trainSample.Item2);
            } while (loopCount < totalBatchSize/_BatchSize);
        }

        private void Train(Volume<double> x, Volume<double> y)
        {
            _Trainer.Train(x, y);
        }
    }
}
