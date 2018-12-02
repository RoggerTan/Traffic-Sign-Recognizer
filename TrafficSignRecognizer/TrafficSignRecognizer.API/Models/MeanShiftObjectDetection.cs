using Accord;
using Accord.Imaging.Filters;
using Accord.MachineLearning;
using Accord.Statistics.Distributions.DensityKernels;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TrafficSignRecognizer.Utils;

namespace TrafficSignRecognizer.API.Models
{
    public class MeanShiftObjectDetection
    {
        public Color BaseColor { get; set; }
        public MeanShift MeanShift { get; private set; }
        public IEnumerable<double[]> Points { get; private set; }

        public MeanShiftObjectDetection(Color baseColor)
        {
            BaseColor = baseColor;
        }

        public void Fit(Bitmap bitmap)
        {
            MeanShift = new MeanShift
            {
                Kernel = new UniformKernel(),
                Bandwidth = 30
            };

            Points = new List<double[]>(bitmap.Width * bitmap.Height);

            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    if (bitmap.GetPixel(i, j) == BaseColor) continue;
                    ((List<double[]>)Points).Add(new double[] { i, j });
                }
            }

            MeanShift.Learn(((List<double[]>)Points).ToArray());
        }

        public IEnumerable<Bitmap> Predict(Bitmap bitmap)
        {
            foreach(var cluster in MeanShift.Clusters)
            {
                var pointsInCluster = Points.Where(x => MeanShift.Clusters.Decide(x) == cluster.Index);

                if (pointsInCluster.Count() == 0) continue;

                var rectangle = PointUtils.GetRectangle(pointsInCluster.Select(x => new IntPoint((int)x[0], (int)x[1])));

                var croppedBitmap = new Crop(rectangle).Apply(bitmap);

                //Check if croppedBitmap is valid (contain any pixel other than chroma key green)

                var validImage = true;
                var greenCount = 0;
                var limitingGreenCount = (int)(0.9 * croppedBitmap.Width * croppedBitmap.Height);

                for (var i = 0; i < croppedBitmap.Width; i++)
                {
                    for (var j = 0; j < croppedBitmap.Height; j++)
                    {
                        if (croppedBitmap.GetPixel(i, j) == BaseColor)
                        {
                            greenCount++;
                        }

                        if (greenCount >= limitingGreenCount)
                        {
                            validImage = false;
                            break;
                        }
                    }
                    if (!validImage) break;
                }

                if (validImage) yield return new Crop(rectangle).Apply(bitmap);
            }
        }
    }
}
