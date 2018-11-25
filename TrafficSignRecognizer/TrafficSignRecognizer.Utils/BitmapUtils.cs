using Accord;
using Accord.Imaging;
using ConvNetSharp.Volume;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.Utils
{
    public static class BitmapUtils
    {
        private static readonly BinaryFormatter _Bf;

        static BitmapUtils()
        {
            _Bf = new BinaryFormatter();
        }

        //Return matrix of pixel for grayscaled bitmap
        public static Matrix<int> AsMatrix(this Bitmap bitmap, int dividend = 1)
        {
            var pixelRows = System.Buffers.ArrayPool<IEnumerable<int>>.Shared.Rent(bitmap.Height);
            int[] pixelCols = null;

            for (var i = 0; i < bitmap.Height; i++)
            {
                pixelCols = System.Buffers.ArrayPool<int>.Shared.Rent(bitmap.Width);
                for (var j = 0; j < bitmap.Width; j++)
                {
                    pixelCols[j] = bitmap.GetPixel(j, i).R / dividend;
                }
                pixelRows[i] = pixelCols.Take(bitmap.Width);
            }

            return new Matrix<int>(pixelRows.Take(bitmap.Height), bitmap.Width, bitmap.Height);
        }

        public static Bitmap ToGrayScale(this Bitmap bitmap)
        {
            var newImage = new Bitmap(bitmap.Width, bitmap.Height);
            for (var i = 0; i < bitmap.Height; i++)
            {
                for (var j = 0; j < bitmap.Width; j++)
                {
                    var pixel = bitmap.GetPixel(j, i);
                    var grayColor = (pixel.R + pixel.G + pixel.B) / 3;
                    newImage.SetPixel(j, i, Color.FromArgb(grayColor, grayColor, grayColor));
                }
            }
            return newImage;
        }

        public static Bitmap ToBitmap(this string base64) => new Bitmap(new MemoryStream(Convert.FromBase64String(base64)));

        public static Bitmap ToBitmap(this int[][] matrix)
        {
            var image = new Bitmap(matrix.Length, matrix[0].Length);

            for (var i = 0; i < matrix.Length; i++)
            {
                for (var j = 0; j < matrix[0].Length; j++)
                {
                    image.SetPixel(i, j, Color.FromArgb(matrix[i][j], matrix[i][j], matrix[i][j]));
                }
            }

            return image;
        }

        public static Bitmap ToBitmap(this Matrix<int> matrix)
        {
            var image = new Bitmap(matrix.Width, matrix.Height);

            var rowEnumerator = matrix.GetEnumerator();
            IEnumerator<int> colEnumerator = null;

            for (var i = 0; i < matrix.Height && rowEnumerator.MoveNext(); i++)
            {
                colEnumerator = rowEnumerator.Current.GetEnumerator();
                for (var j = 0; j < matrix.Width && colEnumerator.MoveNext(); j++)
                {
                    var color = colEnumerator.Current > 255 ?
                        255 :
                        colEnumerator.Current < 0 ?
                        0 :
                        colEnumerator.Current;
                    image.SetPixel(j, i, Color.FromArgb(color, color, color));
                }
            }

            return image;
        }

        public static Base64Image ToBase64Image(this Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);

                return new Base64Image
                {
                    Base64 = Convert.ToBase64String(stream.ToArray())
                };
            }
        }

        public static byte[] GetColorBytesFromBitmapUrl(string relativePath, IHostingEnvironment env, int width = -1, int height = -1)
        {
            var absolutePath = $"{env.WebRootPath}{relativePath.Replace('/', '\\')}";

            var img = System.Drawing.Image.FromFile(absolutePath) as Bitmap;

            return GetColorBytesFromBitmap(img, env, width, height);
        }

        public static byte[] GetColorBytesFromBitmap(this Bitmap img, IHostingEnvironment env, int width = -1, int height = -1)
        {
            var grayscaledImg = img.ToGrayScale();

            if (width != -1)
            {
                grayscaledImg = new Bitmap(grayscaledImg, width, height);
            }

            var result = System.Buffers.ArrayPool<byte>.Shared.Rent(grayscaledImg.Width * grayscaledImg.Height);

            for (var i = 0; i < grayscaledImg.Height; i++)
            {
                for (var j = 0; j < grayscaledImg.Width; j++)
                {
                    result[j + i * grayscaledImg.Width] = grayscaledImg.GetPixel(j, i).R;
                }
            }

            return result;
        }

        public static void AddImgToVolume(string imgUrl, int width, int height, int currentBatch, Volume<double> dataVolume, IHostingEnvironment env)
        {
            var j = 0;

            var img = GetColorBytesFromBitmapUrl(imgUrl, env, width, height);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    dataVolume.Set(x, y, 0, currentBatch, img[j++] / 255.0);
                }
            }
        }

        public static Bitmap GetBitmapFromBitmapUrl(string imgUrl, IHostingEnvironment env)
        {
            var absolutePath = $"{env.WebRootPath}{imgUrl.Replace('/', '\\')}";

            return System.Drawing.Image.FromFile(absolutePath) as Bitmap;
        }

        private static double RedDistanceLimit = 0;
        private static double WhiteDistanceLimit = 0;
        private static double BlueDistanceLimit = 0;
        private static double YellowDistanceLimit = 0;
        private static double BlackDistanceLimit = 0;
        private const double DistanceLimit = 10000;

        public static IEnumerable<SpeededUpRobustFeaturePoint> FilteringPointsByColor(Bitmap bitmap, IEnumerable<SpeededUpRobustFeaturePoint> points, params Color[] colors)
        {
            //Calculate color distance limit first
            if (RedDistanceLimit == 0)
            {
                RedDistanceLimit = ColorDistance(Color.FromArgb(255, 122, 76), Color.FromArgb(236, 29, 36));
                WhiteDistanceLimit = ColorDistance(Color.FromArgb(167, 167, 167), Color.FromArgb(255, 255, 255));
                BlueDistanceLimit = ColorDistance(Color.FromArgb(0, 255, 246), Color.FromArgb(0, 121, 193));
                YellowDistanceLimit = ColorDistance(Color.FromArgb(216, 255, 0), Color.FromArgb(247, 212, 23));
                BlackDistanceLimit = ColorDistance(Color.FromArgb(89, 89, 89), Color.FromArgb(0, 0, 0));
            }

            foreach(var point in points)
            {
                var pointColor = bitmap.GetPixel((int)point.X, (int)point.Y);

                foreach(var color in colors)
                {
                    if (color == Color.Red)
                    {
                        if (ColorDistance(pointColor, Color.FromArgb(236, 29, 36)) <= DistanceLimit)
                        {
                            yield return point;
                            break;
                        }
                    }
                    else if (color == Color.White)
                    {
                        if (ColorDistance(pointColor, Color.FromArgb(255, 255, 255)) <= DistanceLimit)
                        {
                            yield return point;
                            break;
                        }
                    }
                    else if (color == Color.Blue)
                    {
                        if (ColorDistance(pointColor, Color.FromArgb(0, 121, 193)) <= DistanceLimit)
                        {
                            yield return point;
                            break;
                        }
                    }
                    else if (color == Color.Yellow)
                    {
                        if (ColorDistance(pointColor, Color.FromArgb(247, 212, 23)) <= DistanceLimit)
                        {
                            yield return point;
                            break;
                        }
                    }
                    else if (color == Color.Black)
                    {
                        if (ColorDistance(pointColor, Color.FromArgb(0, 0, 0)) <= DistanceLimit)
                        {
                            yield return point;
                            break;
                        }
                    }
                }
            }

            double ColorDistance(Color colorA, Color colorB)
            {
                // Reference: https://en.wikipedia.org/wiki/Color_difference
                return 2 * Math.Pow(colorA.R - colorB.R, 2) + 4 * Math.Pow(colorA.G - colorB.G, 2) + 3 * Math.Pow(colorA.B - colorB.B, 2);
            }
        }

        public static Bitmap Resize(this Bitmap bitmap, int width, int height)
        {
            return new Bitmap(bitmap, new Size(width, height));
        }
    }
}
