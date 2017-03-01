using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using Aero.Patterns;
using Art.Replication;

namespace Aero
{
    public class Memory : IMemoryBox
    {
        public static Memory ActiveBox { get; set; }

        public IStorage Storage { get; private set; }
        public string KeyFormat { get; private set; }

        public Replicator Replicator;

        public KeepProfile KeepProfile;

        public Memory(IStorage storage, string keyFormat = "{0}.json")
        {
            Storage = storage;
            KeyFormat = keyFormat;
            Replicator = new Replicator();
            KeepProfile = KeepProfile.GetFormatted();
        }

        public TValue Revive<TValue>(string key = null, params object[] constructorArgs)
        {
            try
            {
                var type = typeof (TValue);
                var hasDataContract =
                    Attribute.IsDefined(type, typeof (DataContractAttribute)) ||
                    Attribute.IsDefined(type, typeof (CollectionDataContractAttribute));
                if (!hasDataContract) return (TValue) Activator.CreateInstance(type, constructorArgs);

                var storageKey = MakeStorageKey(key, typeof (TValue));
                //using (var stream = Storage.GetReadStream(storageKey))
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                    try
                    {
                        var data = File.ReadAllText(storageKey);
                        int i = 0;
                        var snapshot = data.Capture(KeepProfile, ref i);
                        var item = (TValue)Replicator.TranslateReplicaFrom(snapshot);
                        if (Equals(item, null)) throw new Exception();
                        return item;
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception.ToString());
                        return (TValue) Activator.CreateInstance(type, constructorArgs);
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture;
                    }
                }
            }
            catch
            {
                return (TValue) Activator.CreateInstance(typeof (TValue), constructorArgs);
            }
        }

        public void Keep<TValue>(TValue item, string key = null)
        {
            try
            {
                var type = item.GetType();
                var hasDataContract =
                    Attribute.IsDefined(type, typeof (DataContractAttribute)) ||
                    Attribute.IsDefined(type, typeof (CollectionDataContractAttribute));
                if (!hasDataContract) return;

                var storageKey = MakeStorageKey(key, type);
                //using (var stream = Storage.GetWriteStream(storageKey))
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                    try
                    {
                        var snapshot = Replicator.TranscribeSnapshotFrom(item);
                        var data = new StringBuilder().Append(snapshot, KeepProfile).ToString();
                        File.WriteAllText(storageKey, data);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception.ToString());
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture;
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
            }
        }

        public bool Check<TValue>(string key = null)
        {
            return Storage.HasKey(MakeStorageKey(key, typeof (TValue)));
        }

        public void Destroy<TValue>(string key = null)
        {
            Storage.DeleteKey(MakeStorageKey(key, typeof (TValue)));
        }

        private string MakeStorageKey(string key, Type type)
        {
            return string.Format(KeyFormat ?? "{0}", key ?? type.Name);
        }
    }
}
