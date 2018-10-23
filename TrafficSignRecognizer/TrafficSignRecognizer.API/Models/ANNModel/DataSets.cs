using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace TrafficSignRecognizer.API.Models.ANNModel
{
    public class DataSets
    {
        private const string urlMnist = @"http://yann.lecun.com/exdb/mnist/";
        private const string mnistFolder = @"..\Mnist\";
        private const string trainingLabelFile = "train-labels-idx1-ubyte.gz";
        private const string trainingImageFile = "train-images-idx3-ubyte.gz";
        private const string testingLabelFile = "t10k-labels-idx1-ubyte.gz";
        private const string testingImageFile = "t10k-images-idx3-ubyte.gz";

        public DataSet Train { get; set; }

        public DataSet Validation { get; set; }

        public DataSet Test { get; set; }

        public bool Load(IHostingEnvironment env, int validationSize = 1000)
        {
            // Load datasets
            var train_images = DataSetsReader.Read("/DataSets/Training", env).ToList();
            var testing_images = DataSetsReader.Read("/DataSets/Testing", env).ToList();

            var valiation_images = train_images.GetRange(train_images.Count - validationSize, validationSize);
            train_images = train_images.GetRange(0, train_images.Count - validationSize);

            Train = new DataSet(train_images);
            Validation = new DataSet(valiation_images);
            Test = new DataSet(testing_images);

            return true;
        }
    }
}
