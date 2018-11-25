using ConvNetSharp.Core;
using ConvNetSharp.Core.Training.Double;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using TrafficSignRecognizer.Utils;
using TrafficSignRecognizer.Interfaces.Entities;
using ConvNetSharp.Volume.Double;
using ConvNetSharp.Volume;
using System.Linq;
using ConvNetSharp.Core.Serialization;

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
        public DataSets DataSets { get; set; }
        private IHostingEnvironment _Env;
        public bool IsTrained { get; private set; }

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

        public static CNNModel GetInstance(string trainingPath, string testingPath, string json, IHostingEnvironment env = null)
        {
            if (_Model == null) _Model = new CNNModel(trainingPath, testingPath, env);

            _Model.IsTrained = true;
            _Model._Net = SerializationExtensions.FromJson<double>(json);

            return _Model;
        }

        public TrafficSignInfo Predict(Bitmap img)
        {
            var volume = BuilderInstance.Volume.From(img.GetColorBytesFromBitmap(_Env, _ImgWidth, _ImgHeight).Select(x => (double)x).ToArray(), new Shape(_ImgWidth, _ImgHeight));

            _Net.Forward(volume);

            return new TrafficSignInfo
            {
                Label = _Net.GetPrediction()[0]
            };
        }

        public string GetSerializedNetwork()
        {
            return _Net.ToJson();
        }
    }
}
