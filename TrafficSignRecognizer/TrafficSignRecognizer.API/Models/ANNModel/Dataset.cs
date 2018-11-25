using ConvNetSharp.Volume;
using ConvNetSharp.Volume.Double;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using TrafficSignRecognizer.Interfaces.Entities;
using TrafficSignRecognizer.Utils;

namespace TrafficSignRecognizer.API.Models.ANNModel
{
    public class DataSet
    {
        public List<TrafficSignInfo> TrainImages { get; private set; }
        private readonly Random _random = new Random(RandomUtilities.Seed);
        private int _start;
        private int _epochCompleted;
        private int _Width;
        private int _Height;

        public DataSet(List<TrafficSignInfo> trainImages, int width, int height)
        {
            TrainImages = trainImages;
            _Width = width;
            _Height = height;
        }

        public Tuple<Volume<double>, Volume<double>, int[]> NextBatch(int batchSize, IHostingEnvironment env)
        {
            // Number of classes in the output layer.
            const int numClasses = 62;

            var dataShape = new Shape(_Width, _Height, 1, batchSize);
            var labelShape = new Shape(1, 1, numClasses, batchSize);
            var data = new double[dataShape.TotalLength];
            var label = new double[labelShape.TotalLength];
            var labels = new int[batchSize];

            // Shuffle for the first epoch
            if (_start == 0 && _epochCompleted == 0)
            {
                for (var i = TrainImages.Count - 1; i >= 0; i--)
                {
                    var j = _random.Next(i);
                    var temp = TrainImages[j];
                    TrainImages[j] = TrainImages[i];
                    TrainImages[i] = temp;
                }
            }

            var dataVolume = BuilderInstance.Volume.From(data, dataShape);

            for (var i = 0; i < batchSize; i++)
            {
                var entry = TrainImages[_start];

                labels[i] = entry.Label;

                BitmapUtils.AddImgToVolume(entry.ImgUrl, _Width, _Height, i, dataVolume, env);

                label[i * numClasses + entry.Label] = 1.0;

                _start++;
                if (_start == TrainImages.Count)
                {
                    _start = 0;
                    _epochCompleted++;
                    Console.WriteLine($"Epoch #{_epochCompleted}");
                }
            }


            var labelVolume = BuilderInstance.Volume.From(label, labelShape);

            return new Tuple<Volume<double>, Volume<double>, int[]>(dataVolume, labelVolume, labels);
        }
    }
}
