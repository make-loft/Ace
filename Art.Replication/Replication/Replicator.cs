using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication
{
    public class Replicator
    {
        public ReplicationProfile ActiveProfile { get; set; } = new ReplicationProfile();

        public object TranscribeSnapshotFrom(object master, Type baseType = null)
        {
            if (ActiveProfile.IsSimplex(master)) return master;

            if (ActiveProfile.SnapshotToIdCache.TryGetValue(master, out int id))
                return new Map {{ActiveProfile.IdKey, id}};

            id = ActiveProfile.SnapshotToIdCache.Count;
            ActiveProfile.SnapshotToIdCache.Add(master, id);

            var type = master.GetType();
            var snapshot = new Map();

            if (ActiveProfile.AttachId) snapshot.Add(ActiveProfile.IdKey, id);

            if (ActiveProfile.AttachTypeInfo || master is IEnumerable || baseType != type)
                snapshot.Add(ActiveProfile.TypeKey, type.AssemblyQualifiedName);

            if (master is IDictionary map && type.IsGenericType && type.GetGenericArguments().First() == typeof(string))
            {
                var m = new Map(map.Cast<DictionaryEntry>()
                    .ToDictionary(p => (string) p.Key, p => TranscribeSnapshotFrom(p.Value)));
                if (ActiveProfile.SimplifyMaps) return m;
                snapshot.Add(ActiveProfile.MapKey, m);
            }
            else if (master is IEnumerable set)
            {
                var s = new Set(set.Cast<object>().Select(i => TranscribeSnapshotFrom(i)));
                if (ActiveProfile.SimplifySets) return s;
                snapshot.Add(ActiveProfile.SetKey, s);
            }

            var members = ActiveProfile.Schema.GetDataMembers(type);
            members.ForEach(m => snapshot.Add(ActiveProfile.Schema.GetDataKey(m),
                TranscribeSnapshotFrom(m.GetValue(master), m.GetMemberType())));

            return snapshot;
        }

        public object TranslateReplicaFrom(object masterSnapshot, Type baseType = null)
        {
            if (ActiveProfile.IsSimplex(masterSnapshot)) return masterSnapshot;

            var snapshot = (Map) masterSnapshot;
            var id = snapshot.TryGetValue(ActiveProfile.IdKey, out var key)
                ? (int) key
                : ActiveProfile.IdToReplicaCache.Count;
            if (ActiveProfile.IdToReplicaCache.TryGetValue(id, out object replica)) return replica;

            var type = snapshot.TryGetValue(ActiveProfile.TypeKey, out var typeName)
                ? Type.GetType(typeName.ToString(), true)
                : baseType ?? throw new Exception("Missed type info.");

            replica = type.IsArray
                ? ((IEnumerable) snapshot[ActiveProfile.SetKey]).Cast<object>()
                .Select(i => TranslateReplicaFrom(i))
                .ToArray()
                : ActiveProfile.IdToReplicaCache[id] = Activator.CreateInstance(type);

            if (replica is IDictionary<string, object> map)
            {
                var m = (Map) (ActiveProfile.SimplifyMaps ? snapshot : snapshot[ActiveProfile.MapKey]);
                m.ToList().ForEach(p => map.Add(p.Key, p.Value));
                if (ActiveProfile.SimplifyMaps) return m;
            }

            if (replica is IList set && !set.GetType().IsArray)
            {
                var s = (IEnumerable) (ActiveProfile.SimplifySets ? snapshot : snapshot[ActiveProfile.SetKey]);
                s.Cast<object>()
                    .Select(i => TranslateReplicaFrom(i))
                    .ToList()
                    .ForEach(i => set.Add(i));
                if (ActiveProfile.SimplifySets) return set;
            }

            var members = ActiveProfile.Schema.GetDataMembers(type);
            members.ForEach(m => m.SetValue(replica,
                TranslateReplicaFrom(snapshot[ActiveProfile.Schema.GetDataKey(m)], m.GetMemberType())));

            return replica;
        }
    }
}
