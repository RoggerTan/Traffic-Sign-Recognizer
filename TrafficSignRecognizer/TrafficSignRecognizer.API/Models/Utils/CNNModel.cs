using ConvNetSharp.Core;
using ConvNetSharp.Core.Layers.Double;
using ConvNetSharp.Core.Training.Double;
using ConvNetSharp.Volume;
using ConvNetSharp.Volume.Double;
using System;
using System.Drawing;
using System.IO;

namespace TrafficSignRecognizer.API.Models.Utils
{
    public partial class CNNModel
    {
        private CNNModel _Model;
        private Net<double> _Net;
        private SgdTrainer _Trainer;
        private const int _ImgWidth = 300;
        private const int _ImgHeight = 300;
        private readonly string _TrainingPath;
        private readonly string _TestingPath;

        private CNNModel(string trainingPath, string testingPath)
        {
            _TrainingPath = trainingPath;
            _TestingPath = testingPath;
            Initialize();
        }

        public CNNModel GetInstance(string trainingPath, string testingPath)
        {
            if (_Model == null) _Model = new CNNModel(trainingPath, testingPath);
            return _Model;
        }

        public void Train()
        {
            //var folders = Directory.
        }
    }
}
