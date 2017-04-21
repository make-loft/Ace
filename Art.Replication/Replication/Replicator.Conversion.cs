using System;
using System.Collections;
using System.Linq;

namespace Art.Replication
{
    public static partial class Replicator
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
                if (replicationProfile.SimplifyMaps && type == baseType) return m;
                snapshot.Add(replicationProfile.MapKey, m);
            }
            else if (master is IEnumerable set)
            {
                var s = new Set(set.Cast<object>().Select(i => i.GetState(replicationProfile)));
                if (replicationProfile.SimplifySets && type == baseType && master is IList) return s; /* todo? */
                snapshot.Add(replicationProfile.SetKey, s);
            }

            var members = replicationProfile.Schema.GetDataMembers(type, replicationProfile.MembersFilter);
            members.ForEach(m => snapshot.Add(replicationProfile.Schema.GetDataKey(m),
                m.GetValue(master).GetState(replicationProfile, m.GetMemberType())));

            return snapshot;
        }
    }
}
