using Accord;
using Accord.MachineLearning;
using Accord.Statistics.Distributions.DensityKernels;
using System.Linq;

namespace TrafficSignRecognizer.API.Models
{
    public class MeanShiftFilter
    {
        public MeanShift MeanShift { get; private set; }
        public int MinimumPointsInCluster { get; private set; }
        public double[][] Points { get; private set; }

        public MeanShiftFilter(int minimumPointsInCluster)
        {
            MinimumPointsInCluster = minimumPointsInCluster;
        }

        public void Fit(IntPoint[] points)
        {
            MeanShift = new MeanShift
            {
                Kernel = new UniformKernel(),
                Bandwidth = 30
            };

            Points = points.Select(x => new double[] { x.X, x.Y }).ToArray();
            MeanShift.Learn(Points).ToArray();
        }

        public bool Predict()
        {
            var maxProportion = MeanShift.Clusters.Max(x => x.Proportion);
            var maxDensityCluster = MeanShift.Clusters.First(x => x.Proportion == maxProportion);

            var pointsInMaxDensCluster = Points
                .Where(x => MeanShift.Clusters.Decide(x) == maxDensityCluster.Index);

            return pointsInMaxDensCluster.Count() >= MinimumPointsInCluster;
        }
    }
}
