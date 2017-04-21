﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication
{
    public static partial class Replicator
    {
        public static object GetInstance(this object state, ReplicationProfile replicationProfile,
            Dictionary<int, object> cache = null, Type baseType = null)
        {
            if (replicationProfile.IsSimplex(state)) return state;

            cache = cache ?? new Dictionary<int, object>();
            var snapshot = CompleteMapIfRequried(state, replicationProfile, baseType);
            var id = snapshot.TryGetValue(replicationProfile.IdKey, out var key) ? (int) key : cache.Count;
            if (cache.TryGetValue(id, out object replica)) return replica;

            var type = snapshot.TryGetValue(replicationProfile.TypeKey, out var typeName)
                ? Type.GetType(typeName.ToString(), true)
                : baseType ?? throw new Exception("Missed type info. Can not restore implicitly.");

            replica = cache[id] =
                replicationProfile.Activators.Select(a => a.CreateInstance(snapshot, type)).FirstOrDefault() ??
                (type.IsArray
                    ? Array.CreateInstance(type.GetElementType(), ((Set) snapshot[replicationProfile.SetKey]).Count)
                    : Activator.CreateInstance(type));

            if (replica is IDictionary<string, object> map)
            {
                var items = (IDictionary<string, object>) snapshot[replicationProfile.MapKey];
                items.ToList().ForEach(p => map.Add(p.Key, p.Value));
            }
            else if (replica is IList set)
            {
                var items = (Set) snapshot[replicationProfile.SetKey];
                if (type.IsArray)
                {
                    var source = items.Select(i => i.GetInstance(replicationProfile, cache)).ToArray();
                    Array.Copy(source, (Array) replica, items.Count);
                }
                else items.ForEach(i => set.Add(i.GetInstance(replicationProfile, cache)));
            }

            var members = replicationProfile.Schema.GetDataMembers(type, replicationProfile.MembersFilter);
            members.ForEach(m => m.SetValueIfCanWrite(replica, /* should restore items at read-only members too */
                snapshot[replicationProfile.Schema.GetDataKey(m)].GetInstance(replicationProfile, cache, m.GetMemberType())));

            return replica;
        }

        private static Map CompleteMapIfRequried(object state, ReplicationProfile replicationProfile, Type baseType) =>
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
                    : (Map) state;
    }
}
