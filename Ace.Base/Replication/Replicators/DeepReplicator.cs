using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ace.Replication.Models;
using Ace.Serialization;

namespace Ace.Replication.Replicators
{
	public class DeepReplicator : ACachingReplicator<object>
	{
		public override void FillMap(Map snapshot, ref object instance, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null)
		{
			var type = instance.GetType();

			if (instance is IDictionary map && type.IsGenericDictionaryWithKey<string>())
			{
				var items = new Map(map.Cast<DictionaryEntry>()
					.ToDictionary(p => (string) p.Key, p => profile.Translate(p.Value, idCache)));
				snapshot.Add(profile.MapKey, items);
			}
			else if (instance is IList set)
			{
				var items = new Set(set.Cast<object>().Select(i => profile.Translate(i, idCache)));
				if (instance is Array array && array.Rank > 1)
				{
					var dimensions = new List<int>();
					for (var i = 0; i < array.Rank; i++) dimensions.Add(array.GetLength(i));
					items = items.BoxMultidimensionArray(dimensions, e => new Set(e));
					snapshot.Add(profile.SetDimensionKey, new Set(dimensions.Cast<object>()));
				}

				snapshot.Add(profile.SetKey, items);
			}

			var memberProvider = profile.MemberProviders.First(p => p.CanApply(type));
			var members = memberProvider.GetDataMembers(type);
			foreach (var m in members)
			{
				var key = memberProvider.GetDataKey(m, type);
				var value = profile.Translate(m.GetValue(instance), idCache, m.GetMemberType());
				snapshot.Add(key, value);
			}
		}

		public override void FillInstance(Map snapshot, ref object replica, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null)
		{
			var type = replica.GetType();

			if (replica is IDictionary map && type.IsGenericDictionaryWithKey<string>())
			{
				var items = (IDictionary) snapshot[profile.MapKey];
				items.Cast<DictionaryEntry>().ForEach(p => map.Add(p.Key, p.Value));
			}
			else if (replica is IList set)
			{
				var items = (Set) snapshot[profile.SetKey];
				if (replica is Array array)
				{
					var subtype = type.GetElementType();
					if (array.Rank > 1)
					{
						// var dimensions = ((Set) snapshot[profile.SetDimensionKey]).Cast<int>().ToArray();
						var dimensions = items.RestoreDimensions(array.Rank);
						var source = items.EnumerateMultidimensionArray(array.Rank)
							.Select(i => profile.Replicate(i, idCache, subtype)).ToList();
						source.CopyToMultidimensionalArray(array, dimensions);
					}
					else
					{
						var source = items.Select(i => profile.Replicate(i, idCache, subtype)).ToArray();
						Array.Copy(source, array, source.Length); /* array [replica] is cached */
					}
				}
				else
				{
					set.Clear(); // for reconstruction
					var subtype = type.GetInterfaces().FirstOrDefault(i => i.Name.Is(TypeOf.IList.Name))?
						.GetGenericArguments().FirstOrDefault();
					items.ForEach(i => set.Add(profile.Replicate(i, idCache, subtype)));
				}
			}

			var memberProvider = profile.MemberProviders.First(p => p.CanApply(type));
			var members = memberProvider.GetDataMembers(type);
			foreach (var m in members)
			{
				var memberType = m.GetMemberType();
				/* should enumerate items at read-only members too */
				var key = memberProvider.GetDataKey(m, type);
				if (snapshot.TryGetValue(key, out var snapshotValue).Not()) return;
				var value = profile.Replicate(snapshotValue, idCache, memberType);
				if (profile.TryRestoreTypeInfoImplicitly && value.Is() && value.GetType().IsNot(memberType))
					value = ChangeType(value, memberType, profile);
				if (m.CanWrite()) m.SetValue(replica, value);
			}
		}

		private static object ChangeType(object value, Type targetType, ReplicationProfile profile) =>
			value is string
				? Revert(profile, (string) value, targetType.Name)
				: (targetType.IsPrimitive ? Convert.ChangeType(value, targetType, null) : value);

		private static object Revert(ReplicationProfile profile, string s, string typeKey) =>
			profile.ImplicitConverters.Select(c => c.Revert(s, typeKey))
				.First(v => v != Converter.Undefined);

		public override object ActivateInstance(Map snapshot,
			ReplicationProfile profile, IDictionary<int, object> idCache, Type baseType = null)
		{
			Type type = null;

			try
			{
				type = RestoreType(snapshot, profile, baseType);
				return CreateInstance(type, snapshot, profile);
			}
			catch (Exception e)
			{
				throw new Exception($"{e.Message}\n{type?.FullName}\n{snapshot.SnapshotToString(Snapshot.DefaultKeepProfile)}", e);
			}
		}

		private static Type RestoreType(Map snapshot, ReplicationProfile profile, Type baseType) =>
			snapshot.TryGetValue(profile.TypeKey, out var typeName)
				? Type.GetType(typeName.ToString(), true)
				: baseType ?? throw new Exception("Missed type info. Can not restore implicitly.");

		private static object CreateInstance(Type type, Map snapshot, ReplicationProfile profile) =>
			type.IsArray && type.GetElementType() is Type elementType
				? (snapshot.TryGetValue(profile.SetDimensionKey, out var dimensions)
					? Array.CreateInstance(elementType, ((Set) dimensions).Cast<int>().ToArray())
					: Array.CreateInstance(elementType, ((Set) snapshot[profile.SetKey]).Count))
				: Activator.CreateInstance(type);
	}
}
  