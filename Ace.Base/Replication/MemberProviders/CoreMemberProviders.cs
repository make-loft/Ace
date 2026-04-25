using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ace.Replication.MemberProviders;

public class CoreMemberProviderForKeyValuePair
	: MemberProvider
{
	public override bool CanApply(Type type)
		=> type.GetGenericTypeOrDefault().Is(TypeOf.Generic.KeyValuePair.Raw);

	protected override IEnumerable<MemberInfo> GetDataMembersForCaching(Type type)
		=> type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic).Where(m => m.Is<FieldInfo>());

	public override string GetCustomKey(MemberInfo member) => GetCustomKey(member.Name);

	private string GetCustomKey(string name) => name switch
	{
		"value" => "Value",
		"key" => "Key",
		_ => name
	};
}

public class CoreMemberProvider(BindingFlags bindingFlags, Func<MemberInfo, bool> filter)
	: MemberProvider
{
	public BindingFlags BindingFlags { get; } = bindingFlags;

	public Func<MemberInfo, bool> Filter { get; } = filter;

	protected override IEnumerable<MemberInfo> GetDataMembersForCaching(Type type) => type.EnumerateMembers(BindingFlags)
		.Where(Filter)
		.Where(m => !TypeOf<IEnumerable>.Raw.IsAssignableFrom(type) && m.Name.IsNot("Item"))
		;
}
