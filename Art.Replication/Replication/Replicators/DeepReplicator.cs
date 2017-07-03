using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Art.Serialization;

namespace Art.Replication.Replicators
{
    public class DeepReplicator : ACachingReplicator<object>
    {
        public override void FillMap(Map snapshot, object instance, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache, Type baseType = null)
        {
            var type = instance.GetType();

            if (instance is IDictionary map && type.IsGenericDictionaryWithKey<string>())
            {
                var items = new Map(map.Cast<DictionaryEntry>()
                    .ToDictionary(p => (string) p.Key, p => replicationProfile.Translate(p.Value, idCache)));
                snapshot.Add(replicationProfile.MapKey, items);
            }
            else if (instance is IList set)
            {
                var items = new Set(set.Cast<object>().Select(i => replicationProfile.Translate(i, idCache)));
                if (instance is Array array && array.Rank > 1)
                {
                    var dimensions = new Set();
                    for (var i = 0; i < array.Rank; i++) dimensions.Add(array.GetLength(i));
                    snapshot.Add(replicationProfile.SetDimensionKey, dimensions);
                }
                snapshot.Add(replicationProfile.SetKey, items);
            }

            var memberProvider = replicationProfile.MemberProviders.First(p => p.CanApply(type));
            var members = memberProvider.GetDataMembers(type);
            members.ForEach(m => snapshot.Add(memberProvider.GetDataKey(m),
                replicationProfile.Translate(m.GetValue(instance), idCache, m.GetMemberType())));
        }

        public override void FillInstance(Map snapshot, object replica, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null)
        {
            var type = replica.GetType();

            if (replica is IDictionary map && type.IsGenericDictionaryWithKey<string>())
            {
                var items = (IDictionary) snapshot[replicationProfile.MapKey];
                items.Cast<DictionaryEntry>().ForEach(p => map.Add(p.Key, p.Value));
            }
            else if (replica is IList set)
            {
                var items = (Set) snapshot[replicationProfile.SetKey];
                if (replica is Array array)
                {
                    var source = items.Select(i => replicationProfile.Replicate(i, idCache)).ToArray();
                    if (array.Rank == 1) Array.Copy(source, array, source.Length); /* array [replica] is cached */
                    else
                    {
                        var dimensions = ((Set) snapshot[replicationProfile.SetDimensionKey]).Cast<int>().ToArray();
                        CopyToMultidimensionArray(dimensions, array, source);
                    }
                }
                else
                {
                    set.Clear();
                    var subtype = type.GetInterfaces().FirstOrDefault(i => i.Name == typeof(IList<>).Name)?
                        .GetGenericArguments().FirstOrDefault();
                    items.ForEach(i => set.Add(replicationProfile.Replicate(i, idCache, subtype)));
                }
            }

            var memberProvider = replicationProfile.MemberProviders.First(p => p.CanApply(type));
            var members = memberProvider.GetDataMembers(type);
            members.ForEach(m =>
            {
                var memberType = m.GetMemberType();
                var value = snapshot[memberProvider.GetDataKey(m)];
                if (replicationProfile.TryRestoreTypeInfoImplicitly && value != null && memberType != value.GetType())
                    value = RestoreOriginalType(value, memberType, replicationProfile);

                m.SetValueIfCanWrite(replica, /* should enumerate items at read-only members too */
                    replicationProfile.Replicate(value, idCache, m.GetMemberType()));
            });
        }

        private static void CopyToMultidimensionArray(IList<int> dimensions, Array target, IList<object> source)
        {
            var indices = new int[dimensions.Count];
            for (var i = 0; i < target.Length; i++)
            {
                var t = i;
                for (var j = indices.Length - 1; j >= 0; j--)
                {
                    indices[j] = t % dimensions[j];
                    t /= dimensions[j];
                }
                
                target.SetValue(source[i], indices);
            }
        }

        private static object RestoreOriginalType(object value, Type memberType, ReplicationProfile replicationProfile)
        {
            if (value is string s)
            {
                var typeCode = memberType.Name;
                value = replicationProfile.ImplicitConverters.Select(c => c.Revert(s, typeCode))
                    .First(v => v != Converter.NotParsed);
            }
            else if (memberType.IsPrimitive) value = Convert.ChangeType(value, memberType, null);
            return value;
        }

        public override object ActivateInstance(Map snapshot, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null)
        {
            var type = snapshot.TryGetValue(replicationProfile.TypeKey, out var typeName)
                ? Type.GetType(typeName.ToString(), true)
                : baseType ?? throw new Exception("Missed type info. Can not restore implicitly.");

            if (typeof(Delegate).IsAssignableFrom(type)) return null;

            return type.IsArray
                ? (snapshot.TryGetValue(replicationProfile.SetDimensionKey, out var dimensions)
                    ? Array.CreateInstance(type.GetElementType(), ((Set) dimensions).Cast<int>().ToArray())
                    : Array.CreateInstance(type.GetElementType(), ((Set) snapshot[replicationProfile.SetKey]).Count))
                : Activator.CreateInstance(type);
        }
    }
}
