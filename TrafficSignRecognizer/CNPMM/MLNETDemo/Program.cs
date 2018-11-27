using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Transforms.Normalizers;
using System;
using System.Linq;

namespace MLNETDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var mlContext = new MLContext();

            var reader = mlContext.Data.TextReader(new TextLoader.Arguments
            {
                Column = new[] {
                    new TextLoader.Column("Features", DataKind.R4, 0, 3),
                    new TextLoader.Column("Label", DataKind.TX, 4),
                },
                Separator = ","
            });

            var dataPath = "iris-data.csv";
            var trainData = reader.Read(dataPath);

            var pipeline =
                mlContext.Transforms.Normalize(
                    new NormalizingEstimator.MinMaxColumn("Features", "MinMaxNormalized", fixZero: true),
                    new NormalizingEstimator.MeanVarColumn("Features", "MeanVarNormalized", fixZero: true));

            var normalizedData = pipeline.Fit(trainData).Transform(trainData);

            var minMaxValues = normalizedData.GetColumn<float[]>(mlContext, "MinMaxNormalized").ToArray();
            var meanVarValues = normalizedData.GetColumn<float[]>(mlContext, "MeanVarNormalized").ToArray();
        }
    }
}
