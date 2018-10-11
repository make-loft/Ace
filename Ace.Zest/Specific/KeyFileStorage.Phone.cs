using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using Ace.Patterns;

namespace Ace.Specific
{
	public class KeyFileStorage : IStorage
	{
		private const string DataFolder = "data";
		private static readonly IsolatedStorageFile Storage = IsolatedStorageFile.GetUserStoreForApplication();

		static KeyFileStorage()
		{
			if (Storage.DirectoryExists(DataFolder)) return;
			Storage.CreateDirectory(DataFolder);
			Application.Current.Exit += (sender, args) => Storage.Dispose();
		}

		public Stream GetReadStream(string key)
		{
			var path = Path.Combine(DataFolder, key);
			return Storage.FileExists(path)
				? Storage.OpenFile(path, FileMode.Open)
				: null;
		}

		public Stream GetWriteStream(string key)
		{
			var path = Path.Combine(DataFolder, key);
			return Storage.OpenFile(path, FileMode.Create);
		}

		public void DeleteKey(string key)
		{
			var path = Path.Combine(DataFolder, key);
			if (Storage.FileExists(path)) Storage.DeleteFile(path);
		}

		public bool HasKey(string key)
		{
			var path = Path.Combine(DataFolder, key);
			return Storage.FileExists(path);
		}
	}
}