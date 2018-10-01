using System;
using System.Collections.Generic;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.API.Models.Storages
{
    public static class MatrixStorage
    {
        private static Dictionary<string, ImageMatrix> _storage;

        public static string Add(ImageMatrix imageMatrix)
        {
            if (_storage == null) _storage = new Dictionary<string, ImageMatrix>();
            var guid = Guid.NewGuid().ToString();
            _storage.Add(guid, imageMatrix);

            return guid;
        }

        public static ImageMatrix Get(string guid)
        {
            if (_storage == null || !_storage.ContainsKey(guid))
                return null;
            return _storage[guid];
        }
    }
}
