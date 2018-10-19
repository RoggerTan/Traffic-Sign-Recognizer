using ConvNetSharp.Core;
using ConvNetSharp.Core.Layers.Double;
using ConvNetSharp.Core.Training.Double;

namespace TrafficSignRecognizer.API.Models.Utils
{
    public partial class CNNModel
    {
        private void Initialize()
        {
            // specifies a 2-layer neural network with one hidden layer of 20 neurons
            _Net = new Net<double>();

            _Net.AddLayer(new InputLayer(28, 28, 1));
            _Net.AddLayer(new ConvLayer(5, 5, 8) { Stride = 1, Pad = 2 });
            _Net.AddLayer(new ReluLayer());
            _Net.AddLayer(new PoolLayer(2, 2) { Stride = 2 });
            _Net.AddLayer(new ConvLayer(5, 5, 16) { Stride = 1, Pad = 2 });
            _Net.AddLayer(new ReluLayer());
            _Net.AddLayer(new PoolLayer(3, 3) { Stride = 3 });
            _Net.AddLayer(new FullyConnLayer(10));
            _Net.AddLayer(new SoftmaxLayer(10));

            _Trainer = new SgdTrainer(this._Net)
            {
                LearningRate = 0.01,
                BatchSize = 20,
                L2Decay = 0.001,
                Momentum = 0.9
            };
        }
    }
}
