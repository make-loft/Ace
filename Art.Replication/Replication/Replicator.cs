using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication
{
    public static class Replicator
    {
        public static object TranscribeStateFrom(this object master, ReplicationProfile activeProfile, Type baseType = null)
        {
            if (activeProfile.IsSimplex(master)) return master;

            if (activeProfile.SnapshotToIdCache.TryGetValue(master, out int id))
                return new Map {{activeProfile.IdKey, id}};

            id = activeProfile.SnapshotToIdCache.Count;
            activeProfile.SnapshotToIdCache.Add(master, id);

            var type = master.GetType();
            var snapshot = new Map();

            if (activeProfile.AttachId) snapshot.Add(activeProfile.IdKey, id);

            if (activeProfile.AttachTypeInfo || master is IEnumerable || baseType != type)
                snapshot.Add(activeProfile.TypeKey, type.AssemblyQualifiedName);

            if (master is IDictionary map && type.IsGenericType && type.GetGenericArguments().First() == typeof(string))
            {
                var m = new Map(map.Cast<DictionaryEntry>()
                    .ToDictionary(p => (string) p.Key, p => p.Value.TranscribeStateFrom(activeProfile)));
                if (activeProfile.SimplifyMaps) return m;
                snapshot.Add(activeProfile.MapKey, m);
            }
            else if (master is IEnumerable set)
            {
                var s = new Set(set.Cast<object>().Select(i => i.TranscribeStateFrom(activeProfile)));
                if (activeProfile.SimplifySets) return s;
                snapshot.Add(activeProfile.SetKey, s);
            }

            var members = activeProfile.Schema.GetDataMembers(type);
            members.ForEach(m => snapshot.Add(activeProfile.Schema.GetDataKey(m),
                m.GetValue(master).TranscribeStateFrom(activeProfile, m.GetMemberType())));

            return snapshot;
        }

        public static object TranslateReplicaFrom(this object state, ReplicationProfile activeProfile, Type baseType = null)
        {
            if (activeProfile.IsSimplex(state)) return state;

            var snapshot = state is Set && activeProfile.SimplifySets
                ? new Map
                {
                    {activeProfile.TypeKey, baseType ?? typeof(object[])},
                    {activeProfile.SetKey, state}
                }
                : (Map) state;

            var id = snapshot.TryGetValue(activeProfile.IdKey, out var key)
                ? (int) key
                : activeProfile.IdToReplicaCache.Count;
            if (activeProfile.IdToReplicaCache.TryGetValue(id, out object replica)) return replica;

            var type = snapshot.TryGetValue(activeProfile.TypeKey, out var typeName)
                ? Type.GetType(typeName.ToString(), true)
                : baseType ?? throw new Exception("Missed type info.");

            replica = activeProfile.IdToReplicaCache[id] = type.IsArray
                ? ((Set) snapshot[activeProfile.SetKey]).Select(i => i.TranslateReplicaFrom(activeProfile)).ToArray()
                : Activator.CreateInstance(type);

            if (replica is IDictionary<string, object> map)
            {
                var items = (IDictionary<string, object>) snapshot[activeProfile.MapKey];
                items.ToList().ForEach(p => map.Add(p.Key, p.Value));
            }
            else if (replica is IList set && !type.IsArray)
            {
                var items = ((IList) snapshot[activeProfile.SetKey]).Cast<object>();
                items.Select(i => i.TranslateReplicaFrom(activeProfile)).ToList().ForEach(i => set.Add(i));
            }

            var members = activeProfile.Schema.GetDataMembers(type);
            members.ForEach(m => m.SetValue(replica,
                snapshot[activeProfile.Schema.GetDataKey(m)].TranslateReplicaFrom(activeProfile, m.GetMemberType())));

            return replica;
        }
    }
}
