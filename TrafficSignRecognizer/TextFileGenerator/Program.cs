using System;
using System.IO;
using System.Drawing;

namespace TextFileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var textFile = File.CreateText($@"{args[0]}\positive.txt");

            foreach (var file in Directory.EnumerateFiles(args[0], "*.png", SearchOption.TopDirectoryOnly))
            {
                var bitmap = Image.FromFile(file);

                int width = bitmap.Width;
                int height = bitmap.Height;

                textFile.WriteLine($"{args[1]}/{file.Substring(file.LastIndexOf('\\') + 1)} 1 0 0 {width} {height}");
            }

            textFile.Close();
        }
    }
}
