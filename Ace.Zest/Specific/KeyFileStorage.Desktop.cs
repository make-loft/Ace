using System.IO;
using Ace.Patterns;

namespace Ace.Specific
{
	public class KeyFileStorage : IStorage
	{
		public Stream GetReadStream(string key) => File.OpenRead(key);
		
		public Stream GetWriteStream(string key) => File.Open(key, FileMode.Create);
		
		public bool HasKey(string key) => File.Exists(key);

		public void DeleteKey(string key)
		{
			if (File.Exists(key)) File.Delete(key);
		}
	}
}