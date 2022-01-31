using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Ace.Replication.Models;

namespace Ace.Replication.Replicators
{
	public abstract class ACachingReplicator<T> : Replicator<T>
	{
		public abstract T ActivateInstance(Map map, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null);

		public virtual void FillMap(Map map, ref T instance, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null) => Const.Stub();

		public virtual void FillInstance(Map map, ref T instance, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null) => Const.Stub();

		public override object Translate(object value, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null)
		{
			if (idCache.TryGetValue(value, out var id)) return new Map { { profile.IdKey, id } };
			id = idCache.Count;
			idCache.Add(value, id);

			var map = new Map();
			if (profile.AttachId) map.Add(profile.IdKey, id);
			var valueType = value.GetType();
			if ((profile.AttachType is null && valueType.IsNot(baseType)) || profile.AttachType is true)
				map.Add(profile.TypeKey, valueType.GetFriendlyName());
			var typedValue = (T)value;
			FillMap(map, ref typedValue, profile, idCache, baseType);
			var snapshot = Simplify(map, value, profile, baseType);
			return snapshot;
		}

		public override object Replicate(object value, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null)
		{
			var map = CompleteMapIfRequried(value, profile, baseType);
			var hasKey = map.TryGetValue(profile.IdKey, out var key);
			var id = hasKey ? (int)key : idCache.Count;
			if (idCache.TryGetValue(id, out var replica) && hasKey && map.Count.Is(1)) return replica;
			var typedReplica = (T)(idCache[id] = replica ?? ActivateInstance(map, profile, idCache, baseType));
			if (typedReplica.Is()) FillInstance(map, ref typedReplica, profile, idCache, baseType);
			return typedReplica;
		}

		protected object Simplify(Map map, object instance, ReplicationProfile profile, Type baseType) =>
			instance.GetType().Is(out var type) && type.IsNot(baseType) ? map :
			profile.SimplifySets && instance is IList ? map[profile.SetKey] :
			profile.SimplifyMaps && instance is IDictionary && type.IsGenericDictionaryWithKey<string>() ? map[profile.MapKey] :
			map;

		protected Map CompleteMapIfRequried(object state, ReplicationProfile profile, Type baseType) =>
			profile.SimplifySets && state is Set ? new Map
			{
				{profile.TypeKey, (baseType ?? TypeOf<object[]>.Raw).GetFriendlyName()},
				{profile.SetKey, state}
			} :
			profile.SimplifyMaps && state is Map && baseType.Is() &&
			baseType.IsGenericDictionaryWithKey<string>() ? new Map
			{
				{profile.TypeKey, baseType.GetFriendlyName()},
				{profile.MapKey, state}
			} :
			(Map)state;
	}
}