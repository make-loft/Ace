using System;
using System.IO;
using System.Text;
using Ace.Patterns;
using Ace.Replication;
using Ace.Serialization;

namespace Ace
{
	public class Memory : IMemoryBox
	{
		public IStorage Storage { get; }
		public string KeyFormat { get; set; }

		public KeepProfile KeepProfile = new() {MapPairSplitter = "\t", Delimiter = ""};

		public ReplicationProfile ReplicationProfile = new() {SimplifyMaps = true, SimplifySets = true};

		public Memory(IStorage storage, string keyFormat = "{0}.json")
		{
			Storage = storage;
			KeyFormat = keyFormat;
		}

		protected static bool HasDataContract(Type type) =>
			Attribute.IsDefined(type, TypeOf<DataContractAttribute>.Raw) ||
			Attribute.IsDefined(type, TypeOf<CollectionDataContractAttribute>.Raw);

		public object Revive(string key, Type type, params object[] constructorArgs) =>
			HasDataContract(type) && Storage.Is() && Storage.HasKey(MakeStorageKey(key, type)) && TryDecode(key, type, out var item)
				? item
				: ActivationRequired?.Invoke(key, type, constructorArgs) ?? Activator.CreateInstance(type, constructorArgs);

		public TItem Revive<TItem>(string key = null) => (TItem)Revive(MakeStorageKey(key, TypeOf<TItem>.Raw), TypeOf<TItem>.Raw);

		public bool TryKeep(object item, string key = null) =>
			HasDataContract(item.GetType()) && Storage.Is() && TryEncode(key, item.GetType(), in item);

		private bool TryDecode(string key, Type type, out object item)
		{
			try
			{
				var storageKey = MakeStorageKey(key, type);
				using var stream = Storage.GetReadStream(storageKey);
				using var streamReader = new StreamReader(stream, Encoding.UTF8);
				var data = streamReader.ReadToEnd(); // var data = File.ReadAllText(storageKey);
				var snapshot = data.ParseSnapshot(ReplicationProfile, KeepProfile);
				item = snapshot.ReplicateGraph(type);
				return true;
			}
			catch (Exception exception)
			{
				DecodeFailed?.Invoke(key, type, exception);
				item = default;
				return false;
			}
		}

		private bool TryEncode(string key, Type type, in object item)
		{
			try
			{
				var storageKey = MakeStorageKey(key, item.GetType());
				using var stream = Storage.GetWriteStream(storageKey);
				using var streamWriter = new StreamWriter(stream, Encoding.UTF8);
				var snapshot = item.CreateSnapshot(ReplicationProfile, baseType: type);
				var data = snapshot.ToString(KeepProfile);
				streamWriter.Write(data); // File.WriteAllText(storageKey, data);
				return true;
			}
			catch (Exception exception)
			{
				EncodeFailed?.Invoke(key, item, exception);
				return false;
			}
		}

		public event ActivationQueryHandler ActivationRequired;
		public event DecodeFailedHandler DecodeFailed;
		public event EncodeFailedHandler EncodeFailed;
		public bool Check<TValue>(string key = null) => Storage.HasKey(MakeStorageKey(key, TypeOf<TValue>.Raw));
		public void Destroy<TValue>(string key = null) => Storage.DeleteKey(MakeStorageKey(key, TypeOf<TValue>.Raw));
		private string MakeStorageKey(string key, Type type) => string.Format(KeyFormat ?? "{0}", key ?? type.Name);
	}
}