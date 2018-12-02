using Accord;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TrafficSignRecognizer.Utils
{
    public static class PointUtils
    {
        public static IEnumerable<IntPoint> FindSurroundingPoints(int x, int y, int width, int height, int radius)
        {
            var radiusSquare = radius * radius;

            var topLeftX = x - radius;
            if (topLeftX < 0) topLeftX = 0;
            var topLeftY = y - radius;
            if (topLeftY < 0) topLeftY = 0;
            var bottomRightX = x + radius;
            if (bottomRightX >= width) bottomRightX = width - 1;
            var bottomRightY = y + radius;
            if (bottomRightY >= height) bottomRightY = height - 1;

            for (var i = topLeftX; i <= bottomRightX; i++)
            {
                for (var j = topLeftY; j <= bottomRightY; j++)
                {
                    yield return new IntPoint(i, j);
                }
            }
        }

        public static double EuclideDistance(int x1, int y1, int x2, int y2, bool isRooted)
        {
            var distance = Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2);
            return isRooted ? Math.Sqrt(distance) : distance;
        }

        public static Rectangle GetRectangle(IEnumerable<IntPoint> points)
        {
            int recX = int.MaxValue, recY = int.MaxValue, otherRecX = int.MinValue, otherRecY = int.MinValue;

            foreach (var matchPoint in points)
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

            return new Rectangle(recX, recY, otherRecX - recX, otherRecY - recY);
        }
    }
}
