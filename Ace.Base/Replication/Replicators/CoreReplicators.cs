using System;
using System.Collections.Generic;
using System.Linq;

namespace Ace.Replication.Replicators
{
	public class CoreReplicator : Replicator
	{
		protected virtual bool CanApply(object value) =>
			value is null || value is string || value.GetType().IsPrimitive;

		public override bool CanTranslate(object value, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null) => CanApply(value);

		public override bool CanReplicate(object value, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null) => CanApply(value);

		public override object Translate(object value, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null) => value;

		public override object Replicate(object value, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null) => value.IsNot() || baseType.IsNot() || baseType.IsInstanceOfType(value)
			? value
			: profile.ImplicitConverters
			.Select(c => c.Decode(value.To<string>(), baseType)).FirstOrDefault(v => v.IsNot(Serialization.Converter.Undefined));
	}

	public class CoreReplicator<TValue> : CoreReplicator
	{
		protected override bool CanApply(object value) => value is TValue;
	}

	public class TimeCoreReplicator : CoreReplicator
	{
		protected override bool CanApply(object value) =>
			value is DateTime || value is TimeSpan || value is DateTimeOffset;
	}
}
