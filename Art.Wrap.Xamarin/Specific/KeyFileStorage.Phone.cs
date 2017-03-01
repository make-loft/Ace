using System;
using System.IO;
using Aero.Patterns;

namespace Aero.Specific
{
    public class KeyFileStorage : IStorage
    {
        public Stream GetReadStream(string key)
        {
            throw new NotImplementedException();
        }

        public Stream GetWriteStream(string key)
        {
            throw new NotImplementedException();
        }

        public void DeleteKey(string key)
        {
            throw new NotImplementedException();
        }

        public bool HasKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}