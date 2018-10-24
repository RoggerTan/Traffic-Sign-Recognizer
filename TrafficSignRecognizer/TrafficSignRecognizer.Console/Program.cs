using System;
using System.Drawing;
using System.Threading.Tasks;
using TrafficSignRecognizer.API.Models.ANNModel.Utils;
using static System.Console;

namespace TrafficSignRecognizer.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var model = CNNModel.GetInstance("/DataSets/Training", "/DataSets/Testing", new HostingEnvironment());

            if (!model.IsTrained)
            {
                await model.BeginTraining(60);
            }

            var result = model.Predict(new Bitmap($"{Environment.CurrentDirectory}\00017_00000.png"));
        }
    }
}
