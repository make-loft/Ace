using System;
using System.Collections.Generic;
using Ace.Replication.Models;

namespace Ace.Replication.Replicators
{
	public class DelegateReplicator : ACachingReplicator<Delegate>
	{
		public string TargetKey = "#c_Target";
		public string MethodNameKey = "#c_MethodName";
		public string InvokationListKey = "#c_InvokationList";

		public override void FillMap(Map map, ref Delegate instance, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null)
		{
			if (instance is MulticastDelegate m)
			{
				var value = profile.Translate(m.GetInvocationList(), idCache, baseType);
				map.Add(InvokationListKey, value);
			}

			map.Add(TargetKey, instance.Target ?? instance.Method.DeclaringType);
			map.Add(MethodNameKey, instance.Method.Name);
		}

		public override Delegate ActivateInstance(Map map, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null) => map[TargetKey].To(out var o).Is(out Type t)
			? Delegate.CreateDelegate(baseType, t, (string)map[MethodNameKey])
			: Delegate.CreateDelegate(baseType, o, (string)map[MethodNameKey]);

		public override void FillInstance(Map map, ref Delegate instance, ReplicationProfile profile, IDictionary<int, object> idCache, Type baseType = null)
		{
			if (map.TryGetValue(InvokationListKey, out var value))
			{
				var invokationList = profile.Replicate(value, idCache, value.GetType()).To<Delegate[]>();
				instance = Delegate.Combine(invokationList);
			}
		}

		public override bool CanReplicate(object value, ReplicationProfile profile, IDictionary<int, object> idCache, Type baseType = null) =>
			TypeOf<Delegate>.Raw.IsAssignableFrom(baseType);
	}
}
