using System;

using Ace.Replication.Models;

namespace Ace.Replication.Replicators;

public class DelegateReplicator : ACachingReplicator<Delegate>
{
	public string TargetKey = "#Target";
	public string MethodNameKey = "#MethodName";
	public string InvocationListKey = "#InvocationList";

	public bool SkipMonocastInvokationList { get; set; } = true;

	public override void FillMap(Map map, ref Delegate instance, TranslationArgs args)
	{
		args.Deconstruct(out var profile, out var cache);

		var target = instance.Target.Is()
			? args.Profile.Translate(instance.Target, cache)
			: instance.Method.DeclaringType
			;

		map.Add(TargetKey, target);
		map.Add(MethodNameKey, instance.Method.Name);

		if (instance.Is(out MulticastDelegate m))
		{
			var invocationList = m.GetInvocationList();
			if (SkipMonocastInvokationList && invocationList.Length.Is(1)) return;
			var snapshot = profile.Translate<Delegate[]>(invocationList, cache);
			map.Add(InvocationListKey, snapshot);
		}
	}

	public override Delegate ActivateInstance(Map map, ReplicationArgs args, Type baseType)
		=> map[TargetKey].To(out var o).Is(out Type t)
			? Delegate.CreateDelegate(baseType, t, (string)map[MethodNameKey])
			: Delegate.CreateDelegate(baseType, args.Profile.Replicate(o, args.Cache), (string)map[MethodNameKey])
			;

	public override void FillInstance(Map map, ref Delegate instance, ReplicationArgs args)
	{
		if (map.TryGetValue(InvocationListKey, out var snapshot))
		{
			var invocationList = args.Profile.Replicate<Delegate[]>(snapshot, args.Cache);
			instance = Delegate.Combine(invocationList);
		}
	}

	public override bool CanReplicate(object value, ReplicationArgs args, Type baseType)
		=> TypeOf<Delegate>.Raw.IsAssignableFrom(baseType);
}
