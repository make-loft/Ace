using System;
using System.Collections.Generic;

namespace Art.Replication.Replicators
{
    public abstract class ACachingReplicator<T> : Replicator<T>
    {
        public abstract T ActivateInstance(Map map, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null);

        public virtual void FillMap(Map map, T instance, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache, Type baseType = null)
        {
            
        }

        public virtual void FillInstance(Map map, T instance, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null)
        {
        }

        public override object Translate(object value, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache, Type baseType = null)
        {
            if (idCache.TryGetValue(value, out int id)) return new Map { { replicationProfile.IdKey, id } };
            id = idCache.Count;
            idCache.Add(value, id);

            var map = new Map();
            if (replicationProfile.AttachId) map.Add(replicationProfile.IdKey, id);
            if (replicationProfile.AttachType) map.Add(replicationProfile.TypeKey, value.GetType());
            FillMap(map, (T)value, replicationProfile, idCache, baseType);
            return map;
        }

        public override object Replicate(object value, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null)
        {
            var map = CompleteMapIfRequried(value, replicationProfile, baseType);
            var id = map.TryGetValue(replicationProfile.IdKey, out var key) ? (int)key : idCache.Count;
            if (idCache.TryGetValue(id, out object replica)) return replica;
            replica = idCache[id] = ActivateInstance(map, replicationProfile, idCache, baseType);
            FillInstance(map, (T)replica, replicationProfile, idCache, baseType);
            return replica;
        }

        protected Map CompleteMapIfRequried(object state, ReplicationProfile replicationProfile, Type baseType) =>
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
