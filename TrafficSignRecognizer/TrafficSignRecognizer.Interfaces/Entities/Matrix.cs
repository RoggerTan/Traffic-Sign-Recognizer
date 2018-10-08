using System.Collections;
using System.Collections.Generic;

namespace TrafficSignRecognizer.Interfaces.Entities
{
    public class Matrix<T> : IEnumerable<IEnumerable<T>>
    {
        public IEnumerable<IEnumerable<T>> Value { get; set; }
        public int Width;
        public int Height;
        public T CurrentCellValue => Value.GetEnumerator().Current.GetEnumerator().Current;

        public Matrix(IEnumerable<IEnumerable<T>> value, int width, int height)
        {
            Value = value;
            Width = width;
            Height = height;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }
    }
}
