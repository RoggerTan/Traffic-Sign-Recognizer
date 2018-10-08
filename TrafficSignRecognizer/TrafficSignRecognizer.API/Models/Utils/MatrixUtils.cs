using System.Collections.Generic;
using System.Linq;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.API.Models.Utils
{
    public static class MatrixUtils
    {
        public static Matrix<int> ConvoluteWith(this Matrix<int> matrix, int[][] filter)
        {
            //Result matrix's row count
            var rowCount = matrix.Height / filter.Length;

            //Result matrix's column count
            var colCount = matrix.Width / filter[0].Length;

            //Rent array as result
            var result = System.Buffers.ArrayPool<IEnumerable<int>>.Shared.Rent(rowCount);

            int[] currentRow = null;

            for (var i = 0; i < matrixHeight; i += filter[0].Length)
            {
                currentRow = System.Buffers.ArrayPool<int>.Shared.Rent(colCount);
                for (var j = 0; j < matrixWidth; j += filter[0].Length)
                {
                    currentRow[j] = MultiplyMatrix(j, i);
                }
                result[i] = currentRow.Take(filter[0].Length);
            }

            return new Matrix<int>(result.Take(matrix.Height), matrix.Width, matrix.Height);

            //Local functions
            int MultiplyMatrix(int positionX, int positionY)
            {
                int multiplyResult = 1;

                int loopLimitX = filter[0].Length + positionX;
                int loopLimitY = filter[0].Length + positionY;

                for (var i = positionY; i < loopLimitY; i++)
                {
                    for (var j = positionX; j < loopLimitX; j++)
                    {
                        multiplyResult += matrix[i][j] * filter[i - positionY][j - positionX];
                    }
                }

                return multiplyResult;
            }
        }
    }
}
