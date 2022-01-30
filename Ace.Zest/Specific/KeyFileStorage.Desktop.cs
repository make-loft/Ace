using System.IO;
using Ace.Patterns;

namespace Ace.Specific
{
	public class KeyFileStorage : IStorage
	{
		public string PavePathFor(string key) => Path.GetDirectoryName(key).To(out var path).Length > 0
			? Directory.CreateDirectory(path).Put(key)
			: key;

		public long GetLength(string key) => new FileInfo(PavePathFor(key)).Length;
		public Stream GetReadStream(string key) => File.OpenRead(PavePathFor(key));
		
		public Stream GetWriteStream(string key) => File.Open(PavePathFor(key), FileMode.Create);
		
		public bool HasKey(string key) => File.Exists(key);

		public void DeleteKey(string key)
		{
			if (HasKey(key)) File.Delete(key);
		}
	}
}