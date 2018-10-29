using System;
using System.Linq;
using ConvNetSharp.Core;
using ConvNetSharp.Core.Layers.Double;
using ConvNetSharp.Core.Training;
using ConvNetSharp.Volume;

namespace TrafficSignDemo
{
    internal class Program
    {
        private readonly CircularBuffer<double> _testAccWindow = new CircularBuffer<double>(100);
        private readonly CircularBuffer<double> _trainAccWindow = new CircularBuffer<double>(100);
        private Net<double> _net;
        private int _stepCount;
        private SgdTrainer<double> _trainer;

        private static void Main()
        {
            var program = new Program();
            program.TrafficSignDemo();
        }

        private void TrafficSignDemo()
        {
            var env = new HostingEnvironment();

            var datasets = new DataSets();
            if (!datasets.Load("/DataSets/Training", "/DataSets/Testing", env, 32, 32))
            {
                return;
            }

            // Create network
            _net = new Net<double>();
            _net.AddLayer(new InputLayer(32, 32, 1));
            _net.AddLayer(new ConvLayer(5, 5, 8) { Stride = 1, Pad = 2 });
            _net.AddLayer(new ReluLayer());
            _net.AddLayer(new PoolLayer(2, 2) { Stride = 2 });
            _net.AddLayer(new ConvLayer(5, 5, 16) { Stride = 1, Pad = 2 });
            _net.AddLayer(new ReluLayer());
            _net.AddLayer(new PoolLayer(3, 3) { Stride = 3 });
            _net.AddLayer(new FullyConnLayer(10));
            _net.AddLayer(new SoftmaxLayer(10));

            _trainer = new SgdTrainer<double>(_net)
            {
                LearningRate = 0.01,
                BatchSize = 20,
                L2Decay = 0.001,
                Momentum = 0.9
            };

            Console.WriteLine("Convolutional neural network learning...[Press any key to stop]");
            do
            {
                var trainSample = datasets.Train.NextBatch(_trainer.BatchSize, env);
                Train(trainSample.Item1, trainSample.Item2, trainSample.Item3);

                var testSample = datasets.Test.NextBatch(_trainer.BatchSize, env);
                Test(testSample.Item1, testSample.Item3, _testAccWindow);

                //var trainSample = datasets.Train.NextBatch(_trainer.BatchSize);
                //Train(trainSample.Item1, trainSample.Item2, trainSample.Item3);

                //var testSample = datasets.Test.NextBatch(_trainer.BatchSize);
                //Test(testSample.Item1, testSample.Item3, _testAccWindow);

                Console.WriteLine("Loss: {0} Train accuracy: {1}% Test accuracy: {2}%", _trainer.Loss,
                    Math.Round(_trainAccWindow.Items.Average() * 100.0, 2),
                    Math.Round(_testAccWindow.Items.Average() * 100.0, 2));

                Console.WriteLine("Example seen: {0} Fwd: {1}ms Bckw: {2}ms", _stepCount,
                    Math.Round(_trainer.ForwardTimeMs, 2),
                    Math.Round(_trainer.BackwardTimeMs, 2));
            } while (!Console.KeyAvailable);
        }

        private void Test(Volume<double> x, int[] labels, CircularBuffer<double> accuracy, bool forward = true)
        {
            if (forward)
            {
                _net.Forward(x);
            }

            var prediction = _net.GetPrediction();

            for (var i = 0; i < labels.Length; i++)
            {
                accuracy.Add(labels[i] == prediction[i] ? 1.0 : 0.0);
            }
        }

        private void Train(Volume<double> x, Volume<double> y, int[] labels)
        {
            _trainer.Train(x, y);

            Test(x, labels, _trainAccWindow, false);

            _stepCount += labels.Length;
        }
    }
}