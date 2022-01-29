using System;
using System.Collections.Generic;
using Ace.Replication.Models;

namespace Ace.Replication.Replicators
{
	public class DelegateReplicator : ACachingReplicator<Delegate>
	{
		public string TargetKey = "#Target";
		public string MethodNameKey = "#MethodName";
		public string InvocationListKey = "#InvocationList";

		public bool SkipMonocastInvokationList { get; set; } = true;

		public override void FillMap(Map map, ref Delegate instance, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null)
		{
			var target = instance.Target.Is()
				? profile.Translate(instance.Target, idCache)
				: instance.Method.DeclaringType;

			map.Add(TargetKey, target);
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
			: Delegate.CreateDelegate(baseType, profile.Replicate(o, idCache), (string)map[MethodNameKey]);

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
