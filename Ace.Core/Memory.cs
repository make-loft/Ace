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

		public KeepProfile KeepProfile = new KeepProfile {MapPairSplitter = "\t", Delimiter = ""};

		public ReplicationProfile ReplicationProfile =
			new ReplicationProfile {SimplifyMaps = true, SimplifySets = true};

		public Memory(IStorage storage, string keyFormat = "{0}.json")
		{
			Storage = storage;
			KeyFormat = keyFormat;
		}

		protected static bool HasDataContract(Type type) =>
			Attribute.IsDefined(type, TypeOf<DataContractAttribute>.Raw) ||
			Attribute.IsDefined(type, TypeOf<CollectionDataContractAttribute>.Raw);

		public object Revive(string key, Type type, params object[] constructorArgs) =>
			(HasDataContract(type) && Storage.Is() && Storage.HasKey(MakeStorageKey(key, type)) ? Decode(key, type) : null)
			?? ActivationRequired?.Invoke(key, type, constructorArgs)
			?? Activator.CreateInstance(type, constructorArgs);

		public TItem Revive<TItem>(string key = null) => (TItem)Revive(MakeStorageKey(key, TypeOf<TItem>.Raw), TypeOf<TItem>.Raw);

		public void Keep<TValue>(TValue item, string key = null)
		{
			if (HasDataContract(item.GetType()) && Storage.Is()) Encode(item, key);
		}

		private object Decode(string key, Type type)
		{
			try
			{
				var storageKey = MakeStorageKey(key, type);
				using var stream = Storage.GetReadStream(storageKey);
				using var streamReader = new StreamReader(stream, Encoding.UTF8);
				var data = streamReader.ReadToEnd(); // var data = File.ReadAllText(storageKey);
				var snapshot = data.ParseSnapshot(ReplicationProfile, KeepProfile);
				var item = snapshot.ReplicateGraph(type);
				return item;
			}
			catch (Exception exception)
			{
				DecodeFailed?.Invoke(key, type, exception);
				return default;
			}
		}

		private void Encode(object item, string key)
		{
			try
			{
				var storageKey = MakeStorageKey(key, item.GetType());
				using var stream = Storage.GetWriteStream(storageKey);
				using var streamWriter = new StreamWriter(stream, Encoding.UTF8);
				var snapshot = item.CreateSnapshot(ReplicationProfile);
				var data = snapshot.ToString(KeepProfile);
				streamWriter.Write(data); // File.WriteAllText(storageKey, data);
			}
			catch (Exception exception)
			{
				EncodeFailed?.Invoke(key, item, exception);
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