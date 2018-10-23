using ConvNetSharp.Core;
using ConvNetSharp.Core.Training.Double;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.API.Models.ANNModel.Utils
{
    public partial class CNNModel
    {
        private CNNModel _Model;
        private Net<double> _Net;
        private SgdTrainer _Trainer;
        private const int _ImgWidth = 100;
        private const int _ImgHeight = 100;
        private readonly string _TrainingPath;
        private readonly string _TestingPath;
        private const int _BatchSize = 20;
        private DataSets _DataSet;
        private IHostingEnvironment _Env;

        private CNNModel(string trainingPath, string testingPath, IHostingEnvironment env)
        {
            _TrainingPath = trainingPath;
            _TestingPath = testingPath;
            _Env = env;

            Initialize();
        }

        public CNNModel GetInstance(string trainingPath, string testingPath, IHostingEnvironment env = null)
        {
            if (_Model == null) _Model = new CNNModel(trainingPath, testingPath, env);
            return _Model;
        }

        public TrafficSignInfo Predict(Bitmap x)
        {
            return new TrafficSignInfo
            {
                Label = _Net.GetPrediction()[0]
            };
        }
    }
}
