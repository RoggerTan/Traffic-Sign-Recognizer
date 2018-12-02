using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TrafficSignRecognizer.API.Models;
using TrafficSignRecognizer.Utils;
using System.Linq;

namespace TrafficSignRecognizer.API.UnitTest
{
    [TestClass]
    public class BlobTest
    {
        [TestMethod]
        public void BlobTest1()
        {
            var blob = new BlobShapeDetection(0);

            blob.Fit(BitmapUtils.ToBitmap(Convert.ToBase64String(Properties.Resources.untitled4)));

            var result = blob.GetBitmapsCroppedByShape().Select(x => x.ToBase64Image().Base64).ToArray();
        }
    }
}
