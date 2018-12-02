using Accord.Imaging;
using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TrafficSignRecognizer.API.Models.Entities;
using static TrafficSignRecognizer.Utils.BitmapUtils;

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
            _featurePoints = new List<(string id, IEnumerable<SpeededUpRobustFeaturePoint> featurePoints)>();
        }

        public void Train(string id, Bitmap img)
        {
            var featurePoint = _surf.Transform(img);

            //A feature point should stands on a pixel which color is red, white, yellow, blue or black. These colors are traditionally used by most traffic signs. We should filter by selecting only suitable feature points.

            var filteredFeaturePoint = FilteringPointsByColor(img, 40000, featurePoint, Color.Red, Color.Blue, Color.Yellow);

            if (filteredFeaturePoint.Count() == 0) return;
            _featurePoints.Add((id, filteredFeaturePoint));
        }

        public IEnumerable<Bitmap> GetCroppedImages(Bitmap bitmap)
        {
            //Remove noise color
            ReplaceNoisePixelColor(bitmap, Color.Black, 40000, Color.Red, Color.Blue, Color.Yellow, Color.White);

            var pointData = _surf.Transform(bitmap);

            var lstMatchPoints = new List<BitmapMatchPoints>();

            //Find the maximum match point quantity
            foreach (var trainedMatchPoint in _featurePoints)
            {
                var matchPoints = _matcher.Match(trainedMatchPoint.featurePoints, pointData);

                var estimator = new RANSACEstimator();

                //if (matchPoints.Any(x => x.Length < 4))
                //    continue;

                try
                {
                    estimator.Fit(matchPoints[0], matchPoints[1]);

                    // matchPoints[1] means a collection of match points with coordinates based on the pointData
                    lstMatchPoints.Add(new BitmapMatchPoints
                    {
                        Id = trainedMatchPoint.id,
                        Points = estimator.PlotMinimumArea(matchPoints[1], 8)
                    });
                }
                catch
                {

                }
            }

            IEnumerable<BitmapMatchPoints> allFoundMatchPoints = null;

            //Mean Shift filter
            //var meanshiftfilter = new meanshiftfilter(4, 2);
            //var meanshiftfilterresult = new list<bitmapmatchpoints>(lstmatchpoints.count);

            //foreach (var points in lstmatchpoints)
            //{
            //    meanshiftfilter.fit(points.points);
            //    if (meanshiftfilter.predict())
            //        meanshiftfilterresult.add(points);
            //}

            //allfoundmatchpoints = meanshiftfilterresult;

            //KMeans++ clustering
            try
            {
                var model = new KMeansClusterModel<MatchPointsClusterData, BitmapMatchPointPrediction>(2);
                model.Train(lstMatchPoints.Select(x => x.ToClusterData()));

                var maxQuantity = lstMatchPoints.Max(y => y.Points.Length);

                var predictResult = model.Predict(lstMatchPoints.First(x => x.Points.Length == maxQuantity).ToClusterData());

                // For each Id (traffic sign type), we get the point group that has the most points using group by.
                allFoundMatchPoints = lstMatchPoints
                    .Where(x => model.Predict(x.ToClusterData())
                    .SelectedClusterId == predictResult.SelectedClusterId)
                    .GroupBy(x => x.Id, x => x.Points, (Id, points) =>
                    {
                        var maxPointLength = points.Max(x => x.Length);

                        return new BitmapMatchPoints
                        {
                            Id = Id,
                            Points = points.First(x => x.Length == maxPointLength)
                        };
                    });
            }
            // Catch when too few training data for kmeans. In that case, select all
            catch (InvalidOperationException)
            {
                allFoundMatchPoints = lstMatchPoints
                    .GroupBy(x => x.Id, x => x.Points, (Id, points) =>
                    {
                        var maxPointLength = points.Max(x => x.Length);

                        return new BitmapMatchPoints
                        {
                            Id = Id,
                            Points = points.First(x => x.Length == maxPointLength)
                        };
                    });
            }

            // The rectangle area is actually defined by X, Y, Width, Height. X is the x-axis of the most left-located, while Y is the y-axis of the most top-located. We can find the size by the most right and most bottom. 

            foreach (var (currentMatchPoints, rectangle) in GetRectangles(allFoundMatchPoints))
            {
                var croppedBitmap = new Crop(rectangle).Apply(bitmap);

                //croppedBitmap = new PointsMarker(currentMatchPoints.Points).Apply(croppedBitmap);

                var meanShift = new MeanShiftObjectDetection(Color.Black);

                meanShift.Fit(croppedBitmap);

                foreach (var shiftBitmap in meanShift.Predict(croppedBitmap))
                {
                    var blobShape = new BlobShapeDetection(50);

                    blobShape.Fit(shiftBitmap);

                    foreach(var blobBitmap in blobShape.GetBitmapsCroppedByShape())
                    {
                        yield return blobBitmap;
                    }
                }
            }
        }

        private static IEnumerable<(BitmapMatchPoints, Rectangle)> GetRectangles(IEnumerable<BitmapMatchPoints> matchPointsCollection)
        {
            foreach (var matchPoints in matchPointsCollection)
            {
                int recX = 0, recY = 0, otherRecX = 0, otherRecY = 0;

                foreach(var matchPoint in matchPoints.Points)
                {
                    if (matchPoint.X < recX)
                    {
                        recX = matchPoint.X;
                    }

                    if (matchPoint.Y < recY)
                    {
                        recY = matchPoint.Y;
                    }

                    if (matchPoint.X > otherRecX)
                    {
                        otherRecX = matchPoint.X;
                    }

                    if (matchPoint.Y > otherRecY)
                    {
                        otherRecY = matchPoint.Y;
                    }
                }

                yield return (matchPoints, new Rectangle(recX, recY, otherRecX - recX, otherRecY - recY));
            }
        }
    }
}
