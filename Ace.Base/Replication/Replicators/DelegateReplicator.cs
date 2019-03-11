using System;
using System.Collections.Generic;
using Ace.Replication.Models;

namespace Ace.Replication.Replicators
{
	public class DelegateReplicator : ACachingReplicator<Delegate>
	{
		public string TargetKey = "#c_Target";
		public string MethodNameKey = "#c_Method_Name";
		public string InvocationListKey = "#c_InvocationList";

		public bool SkipMonocastInvokationList { get; set; } = true;

		public override void FillMap(Map map, ref Delegate instance, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null)
		{
			map.Add(TargetKey, instance.Target ?? instance.Method.DeclaringType);
			map.Add(MethodNameKey, instance.Method.Name);
			if (instance.Is(out MulticastDelegate m))
			{
				var invocationList = m.GetInvocationList();
				if (SkipMonocastInvokationList && invocationList.Length.Is(1)) return;
				var snapshot = profile.Translate<Delegate[]>(invocationList, idCache);
				map.Add(InvocationListKey, snapshot);
			}
		}

		public override Delegate ActivateInstance(Map map, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null) => map[TargetKey].To(out var o).Is(out Type t)
			? Delegate.CreateDelegate(baseType, t, (string)map[MethodNameKey])
			: Delegate.CreateDelegate(baseType, o, (string)map[MethodNameKey]);

		public override void FillInstance(Map map, ref Delegate instance, ReplicationProfile profile, IDictionary<int, object> idCache, Type baseType = null)
		{
			if (map.TryGetValue(InvocationListKey, out var snapshot))
			{
				var invocationList = profile.Replicate<Delegate[]>(snapshot, idCache);
				instance = Delegate.Combine(invocationList);
			}
		}

		public override bool CanReplicate(object value, ReplicationProfile profile, IDictionary<int, object> idCache, Type baseType = null) =>
			TypeOf<Delegate>.Raw.IsAssignableFrom(baseType);
	}
}
