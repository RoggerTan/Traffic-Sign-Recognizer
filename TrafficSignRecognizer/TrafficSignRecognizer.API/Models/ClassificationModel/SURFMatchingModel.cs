using Accord.Imaging;
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Accord.Imaging.Filters;
using Microsoft.ML.Trainers;
using TrafficSignRecognizer.API.Models.Entities;

namespace TrafficSignRecognizer.API.Models.ClassificationModel
{
    public sealed class SURFMatchingModel
    {
        private SpeededUpRobustFeaturesDetector _surf;
        private List<(string id, IEnumerable<SpeededUpRobustFeaturePoint> featurePoints)> _featurePoints;
        private KNearestNeighborMatching _matcher;

        public SURFMatchingModel()
        {
            _surf = new SpeededUpRobustFeaturesDetector();
            _matcher = new KNearestNeighborMatching(5);
        }

        public void Train(string id, Bitmap img)
        {
            _featurePoints.Add((id, _surf.Transform(img)));
        }

        public Bitmap GetCroppedImage(Bitmap bitmap)
        {
            var pointData = _surf.Transform(bitmap);

            var lstMatchPoints = new List<BitmapMatchPoints>();

            //Find the maximum match point quantity
            foreach (var trainedMatchPoint in _featurePoints)
            {
                var matchPoints = _matcher.Match(trainedMatchPoint.featurePoints, pointData);

                lstMatchPoints.Add(new BitmapMatchPoints(trainedMatchPoint.id, matchPoints[0]));
            }

            var model = new KMeansClusterModel<BitmapMatchPoints, BitmapMatchPointPrediction>(2);
            model.Train(lstMatchPoints);

            var maxQuantity = lstMatchPoints.Max(y => y.Quantity);

            var predictResult = model.Predict(lstMatchPoints.First(x => x.Quantity == maxQuantity));

            var allFoundMatchPoints = lstMatchPoints
                .Where(x => model.Predict(x)
                .SelectedClusterId == predictResult.SelectedClusterId);

            // The rectangle area is acually defined by X, Y, Width, Height. X is the x-axis of the most left-located, while Y is the y-axis of the most top-located. We can find the size by the most right and most bottom. 

        }
    }
}
