using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConvNetSharp.Volume;
using ConvNetSharp.Volume.Double;
using Microsoft.AspNetCore.Hosting;
using TrafficSignRecognizer.API.Models.Utils;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.API.Models.ANNModel
{
    public class DataSet
    {
        private readonly List<TrafficSignInfo> _trainImages;
        private readonly Random _random = new Random(RandomUtilities.Seed);
        private int _start;
        private int _epochCompleted;

        public DataSet(List<TrafficSignInfo> trainImages)
        {
            _trainImages = trainImages;
        }

        public async Task<Tuple<Volume<double>, Volume<double>, int[]>> NextBatch(int batchSize, IHostingEnvironment env)
        {
            const int w = 28;
            const int h = 28;
            // Number of classes in the output layer.
            const int numClasses = 62;

            var dataShape = new Shape(w, h, 1, batchSize);
            var labelShape = new Shape(1, 1, numClasses, batchSize);
            var data = new double[dataShape.TotalLength];
            var label = new double[labelShape.TotalLength];
            var labels = new int[batchSize];

            // Shuffle for the first epoch
            if (_start == 0 && _epochCompleted == 0)
            {
                for (var i = _trainImages.Count - 1; i >= 0; i--)
                {
                    var j = _random.Next(i);
                    var temp = _trainImages[j];
                    _trainImages[j] = _trainImages[i];
                    _trainImages[i] = temp;
                }
            }

            var dataVolume = BuilderInstance.Volume.From(data, dataShape);

            for (var i = 0; i < batchSize; i++)
            {
                var entry = _trainImages[_start];

                labels[i] = entry.Label;

                var j = 0;
                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        dataVolume.Set(x, y, 0, i, (await BitmapUtils.GetBitmapFromUrl(entry.ImgUrl, env))[j++] / 255.0);
                    }
                }

                label[i * numClasses + entry.Label] = 1.0;

                _start++;
                if (_start == _trainImages.Count)
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
