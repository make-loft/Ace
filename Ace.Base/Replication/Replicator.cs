﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Ace.Replication.Models;

namespace Ace.Replication
{
	public class Replicator
	{
		public virtual bool CanTranslate(object value, ReplicationProfile replicationProfile,
			IDictionary<object, int> idCache, Type baseType = null) => true;

		public virtual bool CanReplicate(object value, ReplicationProfile replicationProfile,
			IDictionary<int, object> idCache, Type baseType = null) => true;

		public virtual object Translate(object value, ReplicationProfile replicationProfile,
			IDictionary<object, int> idCache, Type baseType = null) => value?.ToString();

		public virtual object Replicate(object value, ReplicationProfile replicationProfile,
			IDictionary<int, object> idCache, Type baseType = null) => value?.ToString();

		public virtual List<MemberInfo> GetDataMembers(Type type, Func<MemberInfo, bool> filter) =>
			throw new NotSupportedException();

		public virtual string GetDataKey(MemberInfo member) =>
			throw new NotSupportedException();
	}

	public class Replicator<TValue> : Replicator
	{
		public readonly Type ActiveType = TypeOf<TValue>.Raw;

		public override bool CanTranslate(object value, ReplicationProfile replicationProfile,
			IDictionary<object, int> idCache, Type baseType = null) => value is TValue;

		public override bool CanReplicate(object value, ReplicationProfile replicationProfile,
			IDictionary<int, object> idCache, Type baseType = null) =>
			TypeOf.Object.Raw.Is(ActiveType) || baseType.Is(ActiveType) || value is Map map &&
			map.TryGetValue(replicationProfile.TypeKey, out var v) && v.Is(ActiveType);
	}
}
