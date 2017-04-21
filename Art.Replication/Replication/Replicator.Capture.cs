﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication
{
    public static partial class Replicator
    {
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
                ? Array.CreateInstance(type.GetElementType(), ((Set)snapshot[replicationProfile.SetKey]).Count)
                : Activator.CreateInstance(type);

            if (replica is IDictionary<string, object> map)
            {
                var items = (IDictionary<string, object>) snapshot[replicationProfile.MapKey];
                items.ToList().ForEach(p => map.Add(p.Key, p.Value));
            }
            else if (replica is IList set)
            {
                var items = (Set) snapshot[replicationProfile.SetKey];
                if (type.IsArray) Array.Copy(items.ToArray(), (Array) replica, items.Count);
                else items.ForEach(i => set.Add(i.GetInstance(replicationProfile)));
            }

            var members = replicationProfile.Schema.GetDataMembers(type, Member.CanWrite);
            members.ForEach(m => m.SetValue(replica,
                snapshot[replicationProfile.Schema.GetDataKey(m)]
                    .GetInstance(replicationProfile, m.GetMemberType())));

            return replica;
        }
    }
}
