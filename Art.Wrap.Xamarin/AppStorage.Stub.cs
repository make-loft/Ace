using System.IO;
using Aero.Patterns;

namespace Aero
{
    public class AppStorage : IStorage
    {
        public Stream GetReadStream(string key)
        {
            return null;
        }

        public Stream GetWriteStream(string key)
        {
            return null;
        }

        public void DeleteKey(string key)
        {

        }

        public bool HasKey(string key)
        {
            return false;
        }
    }
}