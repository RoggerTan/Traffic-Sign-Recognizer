using System;
using System.Collections.Generic;

namespace TrafficSignRecognizer.API.Models.Storages
{
    public static class MatrixStorage
    {
        private static Dictionary<string, IEnumerable<IEnumerable<int>>> _storage;

        public static string Add(IEnumerable<IEnumerable<int>> imageMatrix)
        {
            if (_storage == null) _storage = new Dictionary<string, IEnumerable<IEnumerable<int>>>();
            var guid = Guid.NewGuid().ToString();
            _storage.Add(guid, imageMatrix);

            return guid;
        }

        public static IEnumerable<IEnumerable<int>> Get(string guid)
        {
            if (_storage == null || !_storage.ContainsKey(guid))
                return null;
            return _storage[guid];
        }
    }
}
