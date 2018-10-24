using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using TrafficSignRecognizer.API.Models.Utils;

namespace TrafficSignRecognizer.API.UnitTest
{
    [TestClass]
    public class BitmapUtilsTest
    {
        [TestMethod]
        public void BitmapToBytesTest()
        {
            var stream = File.OpenRead("00017_00000.png");

            var img = new Bitmap(stream);

            var result = BitmapUtils.ToBytes(img);

            result.ToList().ForEach(x => Console.WriteLine(x));
        }
    }
}
