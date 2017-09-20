using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Ace.Patterns;
using Ace.Replication;
using Ace.Serialization;

namespace Ace
{
    public class Memory : IMemoryBox
    {
        public static Memory ActiveBox { get; set; }

        public IStorage Storage { get; }
        public string KeyFormat { get; }

        public KeepProfile KeepProfile = new KeepProfile {MapPairSplitter = "\t", Delimiter = ""};
        public ReplicationProfile ReplicationProfile =
            new ReplicationProfile {SimplifyMaps = true, SimplifySets = true};
        
        public Memory(IStorage storage, string keyFormat = "{0}.json")
        {
            Storage = storage;
            KeyFormat = keyFormat;
        }

        protected static bool HasDataContract(Type type) =>
            Attribute.IsDefined(type, typeof(DataContractAttribute)) ||
            Attribute.IsDefined(type, typeof(CollectionDataContractAttribute));

        protected static object Activate(Type type, params object[] constructorArgs) =>
            Activator.CreateInstance(type, constructorArgs);

        public object Revive(string key, Type type, params object[] constructorArgs)
        {
            try
            {
                return HasDataContract(type) ? Decode(key, type) : Activate(type, constructorArgs);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
                return Activate(type, constructorArgs);
            }
        }

        public void Keep<TValue>(TValue item, string key = null)
        {
            try
            {
                if (HasDataContract(item.GetType())) Encode(item, key);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
            }
        }

        private object Decode(string key, Type type)
        {
            var storageKey = MakeStorageKey(key, type);
            //var data = File.ReadAllText(storageKey);
            using (var stream = Storage.GetReadStream(storageKey))
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                return streamReader.ReadToEnd().ParseSnapshot(ReplicationProfile, KeepProfile).ReplicateGraph(type);
        }

        private void Encode(object item, string key)
        {
            var snapshot = item.CreateSnapshot(ReplicationProfile);
            var data = snapshot.ToString(KeepProfile);
            var storageKey = MakeStorageKey(key, item.GetType());
            //File.WriteAllText(storageKey, data);
            using (var stream = Storage.GetWriteStream(storageKey))
            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                streamWriter.Write(data);
        }

        public bool Check<TValue>(string key = null) => Storage.HasKey(MakeStorageKey(key, typeof(TValue)));
        public void Destroy<TValue>(string key = null) => Storage.DeleteKey(MakeStorageKey(key, typeof(TValue)));
        private string MakeStorageKey(string key, Type type) => string.Format(KeyFormat ?? "{0}", key ?? type.Name);
    }
}