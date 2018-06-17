using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Ace.Replication.Models;

namespace Ace.Replication.Replicators
{
	public abstract class ACachingReplicator<T> : Replicator<T>
	{
		public abstract T ActivateInstance(Map map, ReplicationProfile replicationProfile,
			IDictionary<int, object> idCache, Type baseType = null);

		public virtual void FillMap(Map map, T instance, ReplicationProfile replicationProfile,
			IDictionary<object, int> idCache, Type baseType = null) => Const.Stub();

		public virtual void FillInstance(Map map, T instance, ReplicationProfile replicationProfile,
			IDictionary<int, object> idCache, Type baseType = null) => Const.Stub();

		public override object Translate(object value, ReplicationProfile replicationProfile,
			IDictionary<object, int> idCache, Type baseType = null)
		{
			if (idCache.TryGetValue(value, out var id)) return new Map {{replicationProfile.IdKey, id}};
			id = idCache.Count;
			idCache.Add(value, id);

			var map = new Map();
			if (replicationProfile.AttachId) map.Add(replicationProfile.IdKey, id);
			if (replicationProfile.AttachType) map.Add(replicationProfile.TypeKey, value.GetType().AssemblyQualifiedName);
			FillMap(map, (T) value, replicationProfile, idCache, baseType);
			var snapshot = Simplify(map, value, replicationProfile, baseType);
			return snapshot;
		}

		public override object Replicate(object value, ReplicationProfile replicationProfile,
			IDictionary<int, object> idCache, Type baseType = null)
		{
			var map = CompleteMapIfRequried(value, replicationProfile, baseType);
			var hasKey = map.TryGetValue(replicationProfile.IdKey, out var key);
			var id = hasKey ? (int) key : idCache.Count;
			if (idCache.TryGetValue(id, out var replica) && hasKey && map.Count == 1) return replica;
			replica = idCache[id] = replica ?? ActivateInstance(map, replicationProfile, idCache, baseType);
			if (replica != null) FillInstance(map, (T) replica, replicationProfile, idCache, baseType);
			return replica;
		}

		protected object Simplify(Map map, object instance, ReplicationProfile profile, Type baseType) =>
			instance.GetType().Is(out var type) && type.IsNot(baseType) ? map :
			profile.SimplifySets && instance is IList ? map[profile.SetKey] :
			profile.SimplifyMaps && instance is IDictionary && type.IsGenericDictionaryWithKey<string>() ? map[profile.MapKey] :
			map;

		protected Map CompleteMapIfRequried(object state, ReplicationProfile profile, Type baseType) =>
			profile.SimplifySets && state is Set ? new Map
			{
				{profile.TypeKey, (baseType ?? TypeOf<object[]>.Raw).AssemblyQualifiedName},
				{profile.SetKey, state}
			} :
			profile.SimplifyMaps && state is Map && baseType != null &&
			baseType.IsGenericDictionaryWithKey<string>() ? new Map
			{
				{profile.TypeKey, baseType.AssemblyQualifiedName},
				{profile.MapKey, state}
			} :
			(Map) state;
	}
}