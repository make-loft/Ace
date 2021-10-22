using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ace.Replication.MemberProviders
{
	public class CoreMemberProviderForKeyValuePair : MemberProvider
	{
		public override bool CanApply(Type type) => type.Is(TypeOf.KeyValuePair.Raw) || type.Is(TypeOf.DictionaryEntry.Raw);

		protected override IEnumerable<MemberInfo> GetDataMembersForCaching(Type type) =>
			type.GetMembers().Where(m => m.Is<PropertyInfo>());
	}

	public class CoreMemberProvider : MemberProvider
	{
		public BindingFlags BindingFlags { get; }

		public Func<MemberInfo, bool> Filter { get; }

		public CoreMemberProvider(Adapters.BindingFlags bindingFlags, Func<MemberInfo, bool> filter)
		{
			BindingFlags = (BindingFlags) bindingFlags;
			Filter = filter;
		}

		protected override IEnumerable<MemberInfo> GetDataMembersForCaching(Type type) => type.EnumerateMembers(BindingFlags)
			.Where(Filter)
			.Where(m => !TypeOf<IEnumerable>.Raw.IsAssignableFrom(type) && m.Name.IsNot("Item"));
	}
}
