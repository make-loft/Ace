using System;
using System.Collections;
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
            if (replicationProfile.AttachType) map.Add(replicationProfile.TypeKey, value.GetType().AssemblyQualifiedName);
            FillMap(map, (T)value, replicationProfile, idCache, baseType);
            var snapshot = Simplify(map, value, replicationProfile, baseType);
            return snapshot;
        }

        public override object Replicate(object value, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null)
        {
            var map = CompleteMapIfRequried(value, replicationProfile, baseType);
            var id = map.TryGetValue(replicationProfile.IdKey, out var key) ? (int)key : idCache.Count;
            if (idCache.TryGetValue(id, out object replica) && map.Count == 1) return replica;
            replica = idCache[id] = replica ?? ActivateInstance(map, replicationProfile, idCache, baseType);
            FillInstance(map, (T)replica, replicationProfile, idCache, baseType);
            return replica;
        }

        protected object Simplify(Map map, object instance, ReplicationProfile replicationProfile, Type baseType)
        {
            var type = instance.GetType();
            if (type != baseType) return map;

            if (replicationProfile.SimplifyMaps && instance is IDictionary && type.IsGenericDictionaryWithKey<string>())
                return map[replicationProfile.MapKey];

            if (replicationProfile.SimplifySets && instance is IList)
                return map[replicationProfile.SetKey];

            return map;
        }

        protected Map CompleteMapIfRequried(object state, ReplicationProfile replicationProfile, Type baseType) =>
            replicationProfile.SimplifySets && state is Set
                ? new Map
                {
                    {replicationProfile.TypeKey, (baseType ?? typeof(object[])).AssemblyQualifiedName},
                    {replicationProfile.SetKey, state}
                }
                : replicationProfile.SimplifyMaps && state is Map &&
                  baseType != null && baseType.IsGenericDictionaryWithKey<string>()
                    ? new Map
                    {
                        {replicationProfile.TypeKey, baseType.AssemblyQualifiedName},
                        {replicationProfile.MapKey, state}
                    }
                    : (Map)state;
    }
}
