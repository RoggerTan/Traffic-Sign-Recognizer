using ConvNetSharp.Core;
using ConvNetSharp.Core.Layers.Double;
using ConvNetSharp.Core.Training.Double;
using ConvNetSharp.Volume;
using ConvNetSharp.Volume.Double;
using System;
using System.Drawing;

namespace TrafficSignRecognizer.API.Models.Utils
{
    public partial class CNNModel
    {
        private CNNModel _Model;
        private Net<double> _Net;
        private SgdTrainer _Trainer;
        private const int _ImgWidth = 300;
        private const int _ImgHeight = 300;

        private CNNModel()
        {
            Initialize();
        }

        public CNNModel GetInstance()
        {
            if (_Model == null) _Model = new CNNModel();
            return _Model;
        }

        public void Train()
        {

        }
    }
}
