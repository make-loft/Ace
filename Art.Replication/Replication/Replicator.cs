using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication
{
    public static class Replicator
    {
        public static object GetState(this object master, ReplicationProfile replicationProfile, Type baseType = null)
        {
            if (replicationProfile.IsSimplex(master)) return master;

            if (replicationProfile.SnapshotToIdCache.TryGetValue(master, out int id))
                return new Map {{replicationProfile.IdKey, id}};

            id = replicationProfile.SnapshotToIdCache.Count;
            replicationProfile.SnapshotToIdCache.Add(master, id);

            var type = master.GetType();
            var snapshot = new Map();

            if (replicationProfile.AttachId) snapshot.Add(replicationProfile.IdKey, id);

            if (replicationProfile.AttachTypeInfo || master is IEnumerable || baseType != type)
                snapshot.Add(replicationProfile.TypeKey, type.AssemblyQualifiedName);

            if (master is IDictionary map && type.IsGenericDictionaryWithKey<string>())
            {
                var m = new Map(map.Cast<DictionaryEntry>()
                    .ToDictionary(p => (string) p.Key, p => p.Value.GetState(replicationProfile)));
                if (replicationProfile.SimplifyMaps) return m;
                snapshot.Add(replicationProfile.MapKey, m);
            }
            else if (master is IEnumerable set)
            {
                var s = new Set(set.Cast<object>().Select(i => i.GetState(replicationProfile)));
                if (replicationProfile.SimplifySets) return s;
                snapshot.Add(replicationProfile.SetKey, s);
            }

            var members = replicationProfile.Schema.GetDataMembers(type);
            members.ForEach(m => snapshot.Add(replicationProfile.Schema.GetDataKey(m),
                m.GetValue(master).GetState(replicationProfile, m.GetMemberType())));

            return snapshot;
        }

        public static object GetInstance(this object state, ReplicationProfile replicationProfile, Type baseType = null)
        {
            if (replicationProfile.IsSimplex(state)) return state;

            var snapshot = state is Set && replicationProfile.SimplifySets
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
                    : (Map) state;

            var id = snapshot.TryGetValue(replicationProfile.IdKey, out var key)
                ? (int) key
                : replicationProfile.IdToReplicaCache.Count;
            if (replicationProfile.IdToReplicaCache.TryGetValue(id, out object replica)) return replica;

            var type = snapshot.TryGetValue(replicationProfile.TypeKey, out var typeName)
                ? Type.GetType(typeName.ToString(), true)
                : baseType ?? throw new Exception("Missed type info. Try specify it explicitly.");

            replica = replicationProfile.IdToReplicaCache[id] = type.IsArray
                ? ((Set) snapshot[replicationProfile.SetKey]).Select(i => i.GetInstance(replicationProfile))
                .ToArray()
                : Activator.CreateInstance(type);

            if (replica is IDictionary<string, object> map)
            {
                var items = (IDictionary<string, object>) snapshot[replicationProfile.MapKey];
                items.ToList().ForEach(p => map.Add(p.Key, p.Value));
            }
            else if (replica is IList set && !type.IsArray)
            {
                var items = ((IList) snapshot[replicationProfile.SetKey]).Cast<object>();
                items.Select(i => i.GetInstance(replicationProfile)).ToList().ForEach(i => set.Add(i));
            }

            var members = replicationProfile.Schema.GetDataMembers(type);
            members.ForEach(m => m.SetValue(replica,
                snapshot[replicationProfile.Schema.GetDataKey(m)]
                    .GetInstance(replicationProfile, m.GetMemberType())));

            return replica;
        }
    }
}
