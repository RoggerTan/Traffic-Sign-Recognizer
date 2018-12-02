using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TrafficSignRecognizer.Utils;

namespace TrafficSignRecognizer.API.Models
{
    public class BlobShapeDetection
    {
        public Bitmap Bitmap { get; set; }
        private BlobCounter _blobCounter;
        private IEnumerable<Blob> _blobs;
        private SimpleShapeChecker _shapeChecker;
        public int EdgePointAccuracy { get; private set; }

        public BlobShapeDetection(int edgePointAccuracy)
        {
            _blobCounter = new BlobCounter {
                MinWidth = 5,
                MinHeight = 5
            };
            _blobCounter.FilterBlobs = true;

            _shapeChecker = new SimpleShapeChecker();

            EdgePointAccuracy = edgePointAccuracy;
        }

        public void Fit(Bitmap bitmap)
        {
            var colorFilter = new ColorFiltering();

            colorFilter.Red = new IntRange(0, 64);
            colorFilter.Green = new IntRange(0, 64);
            colorFilter.Blue = new IntRange(0, 64);
            colorFilter.FillOutsideRange = false;

            colorFilter.ApplyInPlace(bitmap);

            _blobCounter.ProcessImage(bitmap);

            _blobs = _blobCounter.GetObjectsInformation();

            Bitmap = bitmap;
        }

        public IEnumerable<Bitmap> GetBitmapsCroppedByShape()
        {
            foreach (var blob in _blobs)
            {
                var edgePoints = _blobCounter.GetBlobsEdgePoints(blob);

                Accord.Point center;
                float radius;

                // is circle ?
                if (edgePoints.Count >= EdgePointAccuracy && _shapeChecker.IsCircle(edgePoints, out center, out radius))
                {
                    yield return new Crop(new Rectangle((int)(center.X - radius), (int)(center.Y - radius), (int)radius * 2, (int)radius * 2)).Apply(Bitmap);
                }
                else
                {
                    List<IntPoint> corners = null;

                    var result = false;

                    try
                    {
                        result = edgePoints.Count >= EdgePointAccuracy && _shapeChecker.IsConvexPolygon(edgePoints, out corners);
                    }
                    catch
                    {
                        continue;
                    }

                    // is triangle or quadrilateral
                    if (result)
                    {
                        if (corners.Count < 3 || corners.Count > 4) continue;
                        yield return new Crop(PointUtils.GetRectangle(corners)).Apply(Bitmap);
                    }
                }
            }
        }
    }
}
