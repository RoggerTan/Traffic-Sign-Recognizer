using ConvNetSharp.Core;
using ConvNetSharp.Core.Layers.Double;
using ConvNetSharp.Core.Training.Double;

namespace TrafficSignRecognizer.API.Models.ANNModel.Utils
{
    public partial class CNNModel
    {
        /// <summary>
        /// Initialize CNN model.
        /// </summary>
        private void Initialize()
        {
            if (_DataSets == null)
            {
                _DataSets = new DataSets();
                _DataSets.Load(_TrainingPath, _TestingPath, _Env, _ImgWidth, _ImgHeight);
            }

            // Specifies a 2-layer neural network with one hidden layer of 20 neurons
            _Net = new Net<double>();

            // Depth corresponds to the different color channels of an image. Check out https://stackoverflow.com/questions/32294261/what-is-depth-of-a-convolutional-neural-network
            _Net.AddLayer(new InputLayer(_ImgWidth, _ImgHeight, 1));
            // the stride is the step length that we slide the filter. When the stride is 1 then we move the filters one pixel at a time. When the stride is 2 (or uncommonly 3 or more, though this is rare in practice) then the filters jump 2 pixels at a time as we slide them around. This will produce smaller output volumes spatially. See more here: http://cs231n.github.io/convolutional-networks/
            _Net.AddLayer(new ConvLayer(3, 3, 8) { Stride = 1, Pad = 0 });
            _Net.AddLayer(new ReluLayer());
            _Net.AddLayer(new PoolLayer(2, 2) { Stride = 1 });
            _Net.AddLayer(new ConvLayer(3, 3, 16) { Stride = 1, Pad = 0 });
            _Net.AddLayer(new ReluLayer());
            _Net.AddLayer(new PoolLayer(2, 2) { Stride = 1 });
            _Net.AddLayer(new FullyConnLayer(62));
            _Net.AddLayer(new SoftmaxLayer(62));

            //Batch size is the sample quantity that will be learnt at once.
            _Trainer = new SgdTrainer(_Net)
            {
                LearningRate = 0.01,
                BatchSize = _BatchSize,
                L2Decay = 0.001,
                Momentum = 0.9
            };
        }
    }
}
