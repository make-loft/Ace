using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ace.Serialization;

namespace Ace.Replication.Replicators
{
	public class DeepReplicator : ACachingReplicator<object>
	{
		public override void FillMap(Map snapshot, object instance, ReplicationProfile replicationProfile,
			IDictionary<object, int> idCache, Type baseType = null)
		{
			var type = instance.GetType();

			if (instance is IDictionary map && type.IsGenericDictionaryWithKey<string>())
			{
				var items = new Map(map.Cast<DictionaryEntry>()
					.ToDictionary(p => (string) p.Key, p => replicationProfile.Translate(p.Value, idCache)));
				snapshot.Add(replicationProfile.MapKey, items);
			}
			else if (instance is IList set)
			{
				var items = new Set(set.Cast<object>().Select(i => replicationProfile.Translate(i, idCache)));
				if (instance is Array array && array.Rank > 1)
				{
					var dimensions = new List<int>();
					for (var i = 0; i < array.Rank; i++) dimensions.Add(array.GetLength(i));
					items = items.BoxMultidimensionArray(dimensions, e => new Set(e));
					snapshot.Add(replicationProfile.SetDimensionKey, new Set(dimensions.Cast<object>()));
				}

				snapshot.Add(replicationProfile.SetKey, items);
			}

			var memberProvider = replicationProfile.MemberProviders.First(p => p.CanApply(type));
			var members = memberProvider.GetDataMembers(type);
			members.ForEach(m =>
			{
				var key = memberProvider.GetDataKey(m, type);
				var value = replicationProfile.Translate(m.GetValue(instance), idCache, m.GetMemberType());
				snapshot.Add(key, value);
			});
		}

		public override void FillInstance(Map snapshot, object replica, ReplicationProfile replicationProfile,
			IDictionary<int, object> idCache, Type baseType = null)
		{
			var type = replica.GetType();

			if (replica is IDictionary map && type.IsGenericDictionaryWithKey<string>())
			{
				var items = (IDictionary) snapshot[replicationProfile.MapKey];
				items.Cast<DictionaryEntry>().ForEach(p => map.Add(p.Key, p.Value));
			}
			else if (replica is IList set)
			{
				var items = (Set) snapshot[replicationProfile.SetKey];
				if (replica is Array array)
				{
					var subtype = type.GetElementType();
					if (array.Rank > 1)
					{
						// var dimensions = ((Set) snapshot[replicationProfile.SetDimensionKey]).Cast<int>().ToArray();
						var dimensions = items.RestoreDimensions(array.Rank);
						var source = items.UnboxMultidimensionArray(array.Rank)
							.Select(i => replicationProfile.Replicate(i, idCache, subtype)).ToList();
						source.CopyToMultidimensionalArray(array, dimensions);
					}
					else
					{
						var source = items.Select(i => replicationProfile.Replicate(i, idCache, subtype)).ToArray();
						Array.Copy(source, array, source.Length); /* array [replica] is cached */
					}
				}
				else
				{
					set.Clear();
					var subtype = type.GetInterfaces().FirstOrDefault(i => i.Name == TypeOf.IList.Name)?
						.GetGenericArguments().FirstOrDefault();
					items.ForEach(i => set.Add(replicationProfile.Replicate(i, idCache, subtype)));
				}
			}

			var memberProvider = replicationProfile.MemberProviders.First(p => p.CanApply(type));
			var members = memberProvider.GetDataMembers(type);
			members.ForEach(m =>
			{
				var memberType = m.GetMemberType();
				/* should enumerate items at read-only members too */
				var key = memberProvider.GetDataKey(m, type);
				var value = replicationProfile.Replicate(snapshot[key], idCache, memberType);
				if (replicationProfile.TryRestoreTypeInfoImplicitly && value != null && memberType != value.GetType())
					value = ChangeType(value, memberType, replicationProfile);
				if (m.CanWrite()) m.SetValue(replica, value);
			});
		}

		private static object ChangeType(object value, Type targetType, ReplicationProfile replicationProfile) =>
			value is string
				? Revert(replicationProfile, (string) value, targetType.Name)
				: (targetType.IsPrimitive ? Convert.ChangeType(value, targetType, null) : value);

		private static object Revert(ReplicationProfile replicationProfile, string s, string typeCode) =>
			replicationProfile.ImplicitConverters.Select(c => c.Revert(s, typeCode))
				.First(v => v != Converter.NotParsed);

		public override object ActivateInstance(Map snapshot,
			ReplicationProfile replicationProfile, IDictionary<int, object> idCache, Type baseType = null)
		{
			Type type = null;

			try
			{
				type = RestoreType(snapshot, replicationProfile, baseType);
				return CreateInstance(type, snapshot, replicationProfile);
			}
			catch (Exception e)
			{
				throw new Exception($"{e.Message}\n{type?.FullName}\n{snapshot.SnapshotToString(Snapshot.DefaultKeepProfile)}", e);
			}
		}

		private static Type RestoreType(Map snapshot, ReplicationProfile replicationProfile, Type baseType) =>
			snapshot.TryGetValue(replicationProfile.TypeKey, out var typeName)
				? Type.GetType(typeName.ToString(), true)
				: baseType ?? throw new Exception("Missed type info. Can not restore implicitly.");

		private static object CreateInstance(Type type, Map snapshot, ReplicationProfile replicationProfile) =>
			type.IsArray && type.GetElementType() is Type elementType
				? (snapshot.TryGetValue(replicationProfile.SetDimensionKey, out var dimensions)
					? Array.CreateInstance(elementType, ((Set) dimensions).Cast<int>().ToArray())
					: Array.CreateInstance(elementType, ((Set) snapshot[replicationProfile.SetKey]).Count))
				: Activator.CreateInstance(type);
	}
}
  