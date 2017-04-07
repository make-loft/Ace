using System;
using System.IO;
using Art.Patterns;

namespace Art.Specific
{
    public class KeyFileStorage : IStorage
    {
        private static readonly string Storage = AppDomain.CurrentDomain.BaseDirectory;

        public Stream GetReadStream(string key)
        {
            key = Path.Combine(Storage, key);
            return File.OpenRead(key);
        }

        public Stream GetWriteStream(string key)
        {
            key = Path.Combine(Storage, key);
            return File.Open(key, FileMode.Create);
        }

        public void DeleteKey(string key)
        {
            key = Path.Combine(Storage, key);
            if (File.Exists(key)) File.Delete(key);
        }

        public bool HasKey(string key)
        {
            key = Path.Combine(Storage, key);
            return File.Exists(key);
        }
    }
}