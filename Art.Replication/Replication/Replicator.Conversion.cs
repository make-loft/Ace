using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication
{
    public static partial class Replicator
    {
        public static object GetState(this object master, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache = null, Type baseType = null)
        {
            if (replicationProfile.IsSimplex(master)) return master;

            idCache = idCache ?? new Dictionary<object, int>();
            if (idCache.TryGetValue(master, out int id)) return new Map {{replicationProfile.IdKey, id}};
            idCache.Add(master, idCache.Count);

            var type = master.GetType();
            var snapshot = new Map();

            if (replicationProfile.AttachId) snapshot.Add(replicationProfile.IdKey, id);

            if (replicationProfile.AttachTypeInfo || master is IEnumerable || baseType != type)
                snapshot.Add(replicationProfile.TypeKey, type.AssemblyQualifiedName);

            if (master is IDictionary map && type.IsGenericDictionaryWithKey<string>())
            {
                var m = new Map(map.Cast<DictionaryEntry>()
                    .ToDictionary(p => (string) p.Key, p => p.Value.GetState(replicationProfile, idCache)));
                if (replicationProfile.SimplifyMaps && type == baseType) return m;
                snapshot.Add(replicationProfile.MapKey, m);
            }
            else if (master is IEnumerable set)
            {
                var s = new Set(set.Cast<object>().Select(i => i.GetState(replicationProfile, idCache)));
                if (replicationProfile.SimplifySets && type == baseType && master is IList) return s; /* todo? */
                snapshot.Add(replicationProfile.SetKey, s);
            }

            var members = replicationProfile.Schema.GetDataMembers(type, replicationProfile.MembersFilter);
            members.ForEach(m => snapshot.Add(replicationProfile.Schema.GetDataKey(m),
                m.GetValue(master).GetState(replicationProfile, idCache, m.GetMemberType())));

            return snapshot;
        }
    }
}
