using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication.Replicators
{
    public class ReflexiveReplicator : Replicator
    {
        public override object Translate(object master, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache, Type baseType = null)
        {
            if (idCache.TryGetValue(master, out int id)) return new Map { { replicationProfile.IdKey, id } };
            idCache.Add(master, idCache.Count);

            var type = master.GetType();
            var snapshot = new Map();

            if (replicationProfile.AttachId) snapshot.Add(replicationProfile.IdKey, id);

            if (replicationProfile.AttachType || master is IEnumerable || baseType != type)
                snapshot.Add(replicationProfile.TypeKey, type.AssemblyQualifiedName);

            if (master is IDictionary map && type.IsGenericDictionaryWithKey<string>())
            {
                var m = new Map(map.Cast<DictionaryEntry>()
                    .ToDictionary(p => (string)p.Key, p => p.Value.GetState(replicationProfile, idCache)));
                if (replicationProfile.SimplifyMaps && type == baseType) return m;
                snapshot.Add(replicationProfile.MapKey, m);
            }
            else if (master is IList set)
            {
                var s = new Set(set.Cast<object>().Select(i => i.GetState(replicationProfile, idCache)));
                if (replicationProfile.SimplifySets && type == baseType) return s; /* todo? */
                snapshot.Add(replicationProfile.SetKey, s);
            }

            var members = replicationProfile.Schema.GetDataMembers(type, replicationProfile.MembersFilter);
            members.ForEach(m => snapshot.Add(replicationProfile.Schema.GetDataKey(m),
                m.GetValue(master).GetState(replicationProfile, idCache, m.GetMemberType())));

            return snapshot;
        }

        public override object Replicate(object state, ReplicationProfile replicationProfile, 
            Dictionary<int, object> idCache, Type baseType = null)
        {
            var snapshot = CompleteMapIfRequried(state, replicationProfile, baseType);
            var id = snapshot.TryGetValue(replicationProfile.IdKey, out var key) ? (int)key : idCache.Count;
            if (idCache.TryGetValue(id, out object replica)) return replica;

            var type = snapshot.TryGetValue(replicationProfile.TypeKey, out var typeName)
                ? Type.GetType(typeName.ToString(), true)
                : baseType ?? throw new Exception("Missed type info. Can not restore implicitly.");

            replica = idCache[id] = type.IsArray
                ? Array.CreateInstance(type.GetElementType(), ((Set) snapshot[replicationProfile.SetKey]).Count)
                : Activator.CreateInstance(type);

            if (replica is IDictionary map && type.IsGenericDictionaryWithKey<string>())
            {
                var items = (IDictionary)snapshot[replicationProfile.MapKey];
                items.Cast<DictionaryEntry>().ForEach(p => map.Add(p.Key, p.Value));
            }
            else if (state is IList set)
            {
                var items = (Set)snapshot[replicationProfile.SetKey];
                if (replica is Array array)
                {
                    var source = items.Select(i => i.GetInstance(replicationProfile, idCache)).ToArray();
                    Array.Copy(source, array, items.Count);
                }
                else items.ForEach(i => set.Add(i.GetInstance(replicationProfile, idCache)));
            }

            var members = replicationProfile.Schema.GetDataMembers(type, replicationProfile.MembersFilter);
            members.ForEach(m => m.SetValueIfCanWrite(replica, /* should restore items at read-only members too */
                snapshot[replicationProfile.Schema.GetDataKey(m)].GetInstance(replicationProfile, idCache, m.GetMemberType())));

            return replica;
        }

        protected static Map CompleteMapIfRequried(object state, ReplicationProfile replicationProfile, Type baseType) =>
            replicationProfile.SimplifySets && state is Set
                ? new Map
                {
                    {replicationProfile.TypeKey, baseType ?? typeof(object[])},
                    {replicationProfile.SetKey, state}
                }
                : replicationProfile.SimplifyMaps && state is Map &&
                  baseType != null && baseType.IsGenericDictionaryWithKey<string>()
                    ? new Map
                    {
                        {replicationProfile.TypeKey, baseType},
                        {replicationProfile.MapKey, state}
                    }
                    : (Map)state;
    }
}
