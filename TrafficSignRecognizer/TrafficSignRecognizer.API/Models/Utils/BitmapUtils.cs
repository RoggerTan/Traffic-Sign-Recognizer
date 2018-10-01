using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.API.Models.Utils
{
    public static class BitmapUtils
    {
        private static BinaryFormatter binaryFormatter;

        static BitmapUtils()
        {
            binaryFormatter = new BinaryFormatter();
        }

        //Return matrix of pixel for grayscaled bitmap
        public static int[][] AsMatrix(this Bitmap bitmap, int dividend = 1)
        {
            var pixelRows = System.Buffers.ArrayPool<int[]>.Shared.Rent(bitmap.Height);
            int[] pixelCols = null;

            for (var i = 0; i < bitmap.Height; i++)
            {
                pixelCols = System.Buffers.ArrayPool<int>.Shared.Rent(bitmap.Width);
                for (var j = 0; j < bitmap.Width; j++)
                {
                    pixelCols[j] = bitmap.GetPixel(j, i).R / dividend;
                }
                pixelRows[i] = pixelCols;
            }

            return pixelRows;
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
    }
}
