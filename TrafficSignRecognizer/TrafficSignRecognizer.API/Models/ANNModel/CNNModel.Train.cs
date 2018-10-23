using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrafficSignRecognizer.API.Models.ANNModel.Utils
{
    public partial class CNNModel
    {
        public void Train(double[] x, string[] label)
        {


            _Trainer.Train(x, y);
        }
    }
}
