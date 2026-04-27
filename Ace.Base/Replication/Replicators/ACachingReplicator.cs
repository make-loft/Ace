using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Ace.Replication.Models;

namespace Ace.Replication.Replicators;

public struct ReplicationArgs(ReplicationProfile profile, IDictionary<int, object> cache)
{
	public ReplicationProfile Profile { get; set; } = profile;
	public IDictionary<int, object> Cache { get; } = cache;
	public void Deconstruct(out ReplicationProfile profile, out IDictionary<int, object> cache)
	{
		profile = Profile;
		cache = Cache;
	}
}

public struct TranslationArgs(ReplicationProfile profile, IDictionary<object, int> cache)
{
	public ReplicationProfile Profile { get; set; } = profile;
	public IDictionary<object, int> Cache { get; } = cache;
	public void Deconstruct(out ReplicationProfile profile, out IDictionary<object, int> cache)
	{
		profile = Profile;
		cache = Cache;
	}
}

public abstract class ACachingReplicator<T> : Replicator<T>
{
	public abstract T ActivateInstance(Map map, ReplicationArgs args, Type baseType);

	public virtual void FillMap(Map map, ref T instance, TranslationArgs args) => Const.Stub();

	public virtual void FillInstance(Map map, ref T instance, ReplicationArgs args) => Const.Stub();

	public override object Translate(object value, TranslationArgs args, Type baseType)
	{
		args.Deconstruct(out var profile, out var cache);

		if (cache.TryGetValue(value, out var id)) return new Map { { profile.IdKey, id } };
		id = cache.Count;
		cache.Add(value, id);

		var map = new Map();
		if (profile.AttachId) map.Add(profile.IdKey, id);
		var valueType = value.GetType();
		if ((profile.AttachType is null && valueType.IsNot(baseType)) || profile.AttachType is true)
			map.Add(profile.TypeKey, valueType.GetFriendlyName());
		var typedValue = (T)value;
		FillMap(map, ref typedValue, args);
		var snapshot = Simplify(map, value, profile, baseType);
		return snapshot;
	}

	public override object Replicate(object value, ReplicationArgs args, Type baseType)
	{
		args.Deconstruct(out var profile, out var cache);

		var map = CompleteMapIfRequried(value, profile, baseType);
		var hasKey = map.TryGetValue(profile.IdKey, out var key);
		var id = hasKey ? (int)key : cache.Count;
		if (cache.TryGetValue(id, out var replica) && hasKey && map.Count.Is(1)) return replica;
		var isReusable = baseType.Is() && baseType.IsAssignableFrom(replica?.GetType());
		var typedReplica = (T)(cache[id] = isReusable ? replica : ActivateInstance(map, args, baseType));
		if (typedReplica.Is()) FillInstance(map, ref typedReplica, args);
		return typedReplica;
	}

	protected object Simplify(Map map, object instance, ReplicationProfile profile, Type baseType)
		=>
		instance.GetType().Is(out var type) && type.IsNot(baseType) ? map :
		profile.SimplifySets && instance is IList ? map[profile.SetKey] :
		profile.SimplifyMaps && instance is IDictionary && type.IsGenericDictionaryWithKey<string>() ? map[profile.MapKey] :
		map;

	protected Map CompleteMapIfRequried(object state, ReplicationProfile profile, Type baseType)
		=>
		profile.SimplifySets && state is Set ? new Map
		{
			{profile.TypeKey, (baseType ?? TypeOf<object[]>.Raw).GetFriendlyName()},
			{profile.SetKey, state}
		} 
		: profile.SimplifyMaps && state is Map && baseType.Is()
		&& baseType.IsGenericDictionaryWithKey<string>() ? new Map
		{
			{profile.TypeKey, baseType.GetFriendlyName()},
			{profile.MapKey, state}
		} 
		: (Map)state;
}
