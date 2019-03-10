using System;
using System.Collections.Generic;
using Ace.Replication.Models;

namespace Ace.Replication.Replicators
{
	public class DelegateReplicator : ACachingReplicator<Delegate>
	{
		public string TargetKey = "#c_Target";
		public string MethodKey = "#c_Method";

		public override void FillMap(Map map, Delegate instance, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null)
		{
			map.Add(TargetKey, instance.Target ?? instance.Method.DeclaringType);
			map.Add(MethodKey, instance.Method.Name);
		}

		public override Delegate ActivateInstance(Map map, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null) => map[TargetKey].To(out var o) is Type t
			? Delegate.CreateDelegate(baseType, t, (string)map[MethodKey])
			: Delegate.CreateDelegate(baseType, o, (string)map[MethodKey]);

		public override bool CanReplicate(object value, ReplicationProfile profile, IDictionary<int, object> idCache, Type baseType = null) =>
			TypeOf<Delegate>.Raw.IsAssignableFrom(baseType);
	}
}
