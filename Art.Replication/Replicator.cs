using System;
using System.Collections;
using System.Linq;

namespace Art.Replication
{
    public class Replicator
    {
        public ReplicationProfile ActiveProfile { get; set; } = new ReplicationProfile();

        public object TranscribeSnapshotFrom(object master)
        {
            if (ActiveProfile.IsSimplex(master)) return master;

            if (ActiveProfile.SnapshotToIdCache.TryGetValue(master, out int id))
                return new Map { { ActiveProfile.IdKey, id } };

            id = ActiveProfile.SnapshotToIdCache.Count;
            ActiveProfile.SnapshotToIdCache.Add(master, id);

            var type = master.GetType();
            var members = ActiveProfile.Schema.GetDataMembers(type);
            var snapshot = new Map();

            if (ActiveProfile.AttachId) snapshot.Add(ActiveProfile.IdKey, id);
            if (ActiveProfile.AttachTypeInfo || master is IEnumerable) snapshot.Add(ActiveProfile.TypeKey, type);
            if (master is IEnumerable set)
                snapshot.Add(ActiveProfile.SetKey, new Set(set.Cast<object>().Select(TranscribeSnapshotFrom)));

            members.ForEach(m => snapshot.Add(ActiveProfile.Schema.GetDataKey(m), TranscribeSnapshotFrom(m.GetValue(master))));

            return snapshot;
        }

        public object TranslateReplicaFrom(object masterSnapshot)
        {
            if (ActiveProfile.IsSimplex(masterSnapshot)) return masterSnapshot;

            var snapshot = (Map) masterSnapshot;
            var id = (int) snapshot[ActiveProfile.IdKey];
            if (ActiveProfile.IdToReplicaCache.TryGetValue(id, out object replica)) return replica;

            var type = (Type) snapshot[ActiveProfile.TypeKey];
            replica = type.IsArray
                ? ((IEnumerable) snapshot[ActiveProfile.SetKey]).Cast<object>().Select(TranslateReplicaFrom).ToArray()
                : ActiveProfile.IdToReplicaCache[id] = Activator.CreateInstance(type);
            var members = ActiveProfile.Schema.GetDataMembers(type);
            members.ForEach(m => m.SetValue(replica, TranslateReplicaFrom(snapshot[ActiveProfile.Schema.GetDataKey(m)])));

            if (replica is IList set && !set.GetType().IsArray)
                ((IEnumerable)snapshot[ActiveProfile.SetKey]).Cast<object>()
                    .Select(TranslateReplicaFrom)
                    .ToList()
                    .ForEach(i => set.Add(i));
            return replica;
        }
    }
}
