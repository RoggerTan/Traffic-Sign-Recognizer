using Accord;
using Accord.Imaging;
using Accord.Math;
using TrafficSignRecognizer.Utils;

namespace TrafficSignRecognizer.API.Models
{
    public class RANSACEstimator
    {
        private RansacHomographyEstimator _ransac;
        private IntPoint[] _points1;
        private IntPoint[] _points2;

        public RANSACEstimator()
        {
            _ransac = new RansacHomographyEstimator(0.001, 0.99);
        }

        public void Fit(IntPoint[] points1, IntPoint[] points2)
        {
            _points1 = points1;
            _points2 = points2;
        }

        public IntPoint[] PlotMinimumArea(IntPoint[] points, int loopCount)
        {
            IntPoint[] minPoint = null;
            int minArea = int.MaxValue;

            for (var i = 0; i < loopCount; i++)
            {
                //Estimate
                _ransac.Estimate(_points1, _points2);

                var curPoints = points.Get(_ransac.Inliers);
                var curArea = GetArea(curPoints);

                if (minArea > curArea)
                {
                    minPoint = curPoints;
                    minArea = curArea;
                }
            }

            return minPoint;
        }

        private int GetArea(IntPoint[] points)
        {
            var rectangle = PointUtils.GetRectangle(points);

            return rectangle.Width * rectangle.Height;
        }
    }
}
