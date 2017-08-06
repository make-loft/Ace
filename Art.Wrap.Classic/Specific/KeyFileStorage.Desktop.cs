using System;
using System.IO;
using Art.Patterns;

namespace Art.Specific
{
    public class KeyFileStorage : IStorage
    {
        private static readonly string Storage = AppDomain.CurrentDomain.BaseDirectory;

        public Stream GetReadStream(string key) => File.OpenRead(Path.Combine(Storage, key));
        
        public Stream GetWriteStream(string key) => File.Open(Path.Combine(Storage, key), FileMode.Create);
        
        public bool HasKey(string key) => File.Exists(Path.Combine(Storage, key));

        public void DeleteKey(string key)
        {
            key = Path.Combine(Storage, key);
            if (File.Exists(key)) File.Delete(key);
        }
    }
}