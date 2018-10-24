using ConvNetSharp.Core;
using ConvNetSharp.Core.Training.Double;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using TrafficSignRecognizer.API.Models.Utils;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.API.Models.ANNModel.Utils
{
    public partial class CNNModel
    {
        private static CNNModel _Model;
        private Net<double> _Net;
        private SgdTrainer _Trainer;
        private const int _ImgWidth = 32;
        private const int _ImgHeight = 32;
        private readonly string _TrainingPath;
        private readonly string _TestingPath;
        private const int _BatchSize = 20;
        private DataSets _DataSets;
        private IHostingEnvironment _Env;
        public bool IsTrained { get; set; }

        private CNNModel(string trainingPath, string testingPath, IHostingEnvironment env)
        {
            _TrainingPath = trainingPath;
            _TestingPath = testingPath;
            _Env = env;

            Initialize();
        }

        public static CNNModel GetInstance(string trainingPath, string testingPath, IHostingEnvironment env = null)
        {
            if (_Model == null) _Model = new CNNModel(trainingPath, testingPath, env);
            return _Model;
        }

        public TrafficSignInfo Predict(Bitmap x)
        {
            //_Net.Forward(x.);

            return new TrafficSignInfo
            {
                Label = _Net.GetPrediction()[0]
            };
        }
    }
}
