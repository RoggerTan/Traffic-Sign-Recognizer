using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        public static int[][] AsMatrix(this Bitmap bitmap, int divider = 1)
        {
            var listColor = new List<List<int>>();

            for (var i = 0; i < bitmap.Height; i++)
            {
                listColor.Add(new List<int>());
                for (var j = 0; j < bitmap.Width; j++)
                {
                    listColor[i].Add(bitmap.GetPixel(j, i).R);
                }
            }

            return listColor
                .Select(x => x.ToArray())
                .ToArray();
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
            MemoryStream stream = null;
            binaryFormatter.Serialize(stream, bitmap);
            return new Base64Image
            {
                Base64 = Convert.ToBase64String(stream.ToArray())
            };
        }
    }
}
