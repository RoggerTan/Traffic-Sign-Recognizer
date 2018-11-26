using Accord;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TrafficSignRecognizer.Utils
{
    public static class PointUtils
    {
        public static Rectangle GetRectangle(IntPoint[] points)
        {
            int recX = 0, recY = 0, otherRecX = 0, otherRecY = 0;

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
