using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ace.Replication.MemberProviders
{
	public class CoreMemberProviderForKeyValuePair : MemberProvider
	{
		public override bool CanApply(Type type) =>
			type.GetGenericTypeOrDefault().Is(TypeOf.Generic.KeyValuePair.Raw);

		protected override IEnumerable<MemberInfo> GetDataMembersForCaching(Type type) =>
			type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic).Where(m => m.Is<FieldInfo>());

		public override string GetCustomKey(MemberInfo member) => GetCustomKey(member.Name);

		private string GetCustomKey(string name) =>
			name.Is("value") ? "Value" :
			name.Is("key") ? "Key" :
			name;
	}

	public class CoreMemberProvider : MemberProvider
	{
		public BindingFlags BindingFlags { get; }

		public Func<MemberInfo, bool> Filter { get; }

		public CoreMemberProvider(BindingFlags bindingFlags, Func<MemberInfo, bool> filter)
		{
			BindingFlags = bindingFlags;
			Filter = filter;
		}

		protected override IEnumerable<MemberInfo> GetDataMembersForCaching(Type type) => type.EnumerateMembers(BindingFlags)
			.Where(Filter)
			.Where(m => !TypeOf<IEnumerable>.Raw.IsAssignableFrom(type) && m.Name.IsNot("Item"));
	}
}
