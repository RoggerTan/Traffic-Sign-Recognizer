using Accord;
using Accord.Imaging;
using Accord.Math;

namespace TrafficSignRecognizer.API.Models
{
    public class RANSACEstimator
    {
        private RansacHomographyEstimator _ransac;

        public RANSACEstimator()
        {
            _ransac = new RansacHomographyEstimator(0.001, 0.99);
        }

        public void Estimate(IntPoint[] points1, IntPoint[] points2)
        {
            _ransac.Estimate(points1, points2);
        }

        public IntPoint[] Plot(IntPoint[] points)
        {
            return points.Get(_ransac.Inliers);
        }
    }
}
