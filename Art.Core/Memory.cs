using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Art.Patterns;

namespace Art
{
    public class Memory : IMemoryBox
    {
        public static Memory ActiveBox { get; set; }

        public IStorage Storage { get; }
        public string KeyFormat { get; }

        public Memory(IStorage storage, string keyFormat = "{0}.json")
        {
            Storage = storage;
            KeyFormat = keyFormat;
        }

        protected static bool HasDataContract(Type type) =>
            Attribute.IsDefined(type, typeof(DataContractAttribute)) ||
            Attribute.IsDefined(type, typeof(CollectionDataContractAttribute));
        
        protected static TValue Activate<TValue>(params object[] constructorArgs) =>
            (TValue) Activator.CreateInstance(typeof(TValue), constructorArgs);

        public TValue Revive<TValue>(string key = null, params object[] constructorArgs)
        {
            try
            {
                return HasDataContract(typeof(TValue)) ? Decode<TValue>(key) : Activate<TValue>(constructorArgs);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
                return Activate<TValue>(constructorArgs);
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
        
        private TValue Decode<TValue>(string key)
        {
            var storageKey = MakeStorageKey(key, typeof(TValue));
            var data = File.ReadAllText(storageKey);
            var snapshot = data.ParseSnapshot();
            return snapshot.ReplicateGraph<TValue>();
        }

        private void Encode<TValue>(TValue item, string key)
        {
            var snapshot = item.CreateSnapshot();
            var data = snapshot.ToString();
            var storageKey = MakeStorageKey(key, item.GetType());
            File.WriteAllText(storageKey, data);
        }

        public bool Check<TValue>(string key = null) => Storage.HasKey(MakeStorageKey(key, typeof(TValue)));
        public void Destroy<TValue>(string key = null) => Storage.DeleteKey(MakeStorageKey(key, typeof(TValue)));
        private string MakeStorageKey(string key, Type type) => string.Format(KeyFormat ?? "{0}", key ?? type.Name);
    }
}