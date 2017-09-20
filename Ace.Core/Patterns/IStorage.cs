using System.IO;

namespace Ace.Patterns
{
    public interface IStorage
    {
        Stream GetReadStream(string key);
        Stream GetWriteStream(string key);
        void DeleteKey(string key);
        bool HasKey(string key);
    }
}