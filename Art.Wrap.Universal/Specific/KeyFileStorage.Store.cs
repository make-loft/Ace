using System;
using System.IO;
using System.Linq;
using Windows.Storage;
using Aero.Patterns;

namespace Aero.Specific
{
    public class KeyFileStorage : IStorage
    {
        private static readonly StorageFolder Storage = ApplicationData.Current.LocalFolder;

        public Stream GetReadStream(string key)
        {
            if (Storage.GetFilesAsync().Await().All(f => f.Name != key)) return null;
            var storageFile = Storage.GetFileAsync(key).Await();
            return storageFile == null ? null : storageFile.OpenStreamForReadAsync().Await();
        }

        public Stream GetWriteStream(string key)
        {
            var storageFile = Storage.CreateFileAsync(key, CreationCollisionOption.OpenIfExists).Await();
            if (storageFile == null) throw new Exception("Can't write to file!");
            var stream = storageFile.OpenStreamForWriteAsync().Await();
            if (storageFile == null) throw new Exception("Can't get file stream!");
            return stream;
        }

        public void DeleteKey(string key)
        {
            if (Storage.GetFilesAsync().Await().All(f => f.Name != key)) return;
            var storageFile = Storage.GetFileAsync(key).Await();
            if (storageFile != null) storageFile.DeleteAsync().AsTask().Wait();
        }

        public bool HasKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}