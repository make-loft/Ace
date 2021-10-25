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
				var subtype = type.GetInterfaces()
					.FirstOrDefault(i => i.Name.Is(TypeOf.IDictionary.Name))?
					.GetGenericArguments()[1];
				var items = new Map(map.Cast<DictionaryEntry>()
					.ToDictionary(p => (string) p.Key, p => profile.Translate(p.Value, idCache, subtype)));
				snapshot.Add(profile.MapKey, items);
			}
			else if (instance is ICollection set)
			{
				var subtype = type.GetInterfaces()
					.FirstOrDefault(i => i.Name.Is(TypeOf.ICollection.Name))?
					.GetGenericArguments()[0];
				var items = new Set(set.Cast<object>().Select(i => profile.Translate(i, idCache, subtype)));
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
				var repeats = 0;
				Repeat:
				try
				{
					var key = memberProvider.GetDataKey(m, type, members);
					var value = profile.Translate(m.GetValue(instance), idCache, m.GetMemberType());
					snapshot.Add(key, value);
				}
				catch (Exception exception)
				{
					if (exception is InvalidOperationException && repeats < 8)
					{
						repeats++;
						goto Repeat;
					}
					throw new Exception($"{memberProvider.GetDataKey(m, type, members)}", exception);
				}
			}
		}

		public override void FillInstance(Map snapshot, ref object replica, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null)
		{
			var type = replica.GetType();

			if (replica is IDictionary map && type.IsGenericDictionaryWithKey<string>())
			{
				var subtype = type.GetInterfaces()
					.FirstOrDefault(i => i.Name.Is(TypeOf.IDictionary.Name))?.GetGenericArguments()[1];
				var pairs = (IDictionary) snapshot[profile.MapKey];
				foreach (DictionaryEntry pair in pairs)
					map.Add(pair.Key, profile.Replicate(pair.Value, idCache, subtype));
			}
			else if (replica is ICollection set)
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
				else if (replica is IList list)
				{
					list.Clear(); // for reconstruction
					var subtype = type.GetInterfaces()
						.FirstOrDefault(i => i.Name.Is(TypeOf.IList.Name))?
						.GetGenericArguments()[0];
					items.ForEach(i => list.Add(profile.Replicate(i, idCache, subtype)));
				}
				else if (replica is IDictionary dictionary)
				{
					dictionary.Clear(); // for reconstruction
					items.ForEach(i =>
					{
						var entry = (DictionaryEntry)profile.Replicate(i, idCache, TypeOf<DictionaryEntry>.Raw);
						dictionary.Add(entry.Key, entry.Value);
					});
				}
			}

			var memberProvider = profile.MemberProviders.First(p => p.CanApply(type));
			var members = memberProvider.GetDataMembers(type);
			foreach (var m in members)
			{
				try
				{
					var memberType = m.GetMemberType();
					/* should enumerate items at read-only members too */
					var key = memberProvider.GetDataKey(m, type, members);
					if (snapshot.TryGetValue(key, out var snapshotValue).Not()) continue;
					var value = profile.Replicate(snapshotValue, idCache, memberType);
					if (profile.TryRestoreTypeInfoImplicitly && value.Is() && value.GetType().IsNot(memberType))
						value = ChangeType(value, memberType, profile);
					if (m.CanWrite()) m.SetValue(replica, value);
				}
				catch (Exception excepton)
				{
					throw new Exception($"{memberProvider.GetDataKey(m, type, members)}", excepton);
				}
			}
		}

		private static object ChangeType(object value, Type targetType, ReplicationProfile profile) =>
			value is string @string
				? Decode(profile, @string, targetType.Name)
				: (targetType.IsPrimitive ? Convert.ChangeType(value, targetType, null) : value);

		private static object Decode(ReplicationProfile profile, string s, string typeKey) =>
			profile.ImplicitConverters.Select(c => c.Decode(s, typeKey))
				.First(v => v.IsNot(Converter.Undefined));

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
  