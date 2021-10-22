using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ace.Replication.MemberProviders
{
	public class MemberProvider
	{
		public bool PreferFullKeyWhenInheritance { get; set; }

		public virtual bool CanApply(Type type) => true;

		public virtual string GetCustomKey(MemberInfo member) => member.Name;

		private bool IsFullKeyRequried(MemberInfo member, IList<MemberInfo> members) =>
			PreferFullKeyWhenInheritance || members.Any(m => m.Name.Is(member.Name) && m.IsNot(member));

		public string GetDataKey(MemberInfo member, Type activeType, IList<MemberInfo> members) =>
			(member.DeclaringType.IsNot(activeType) && IsFullKeyRequried(member, members)
				? member.DeclaringType?.Name + "."
				: null)
			+ GetCustomKey(member);

		protected virtual IEnumerable<MemberInfo> GetDataMembersForCaching(Type type) => type.GetMembers();

		protected Dictionary<Type, List<MemberInfo>> TypeToMembers = new();

		public virtual List<MemberInfo> GetDataMembers(Type type) => TypeToMembers.TryGetValue(type, out var members)
			? members
			: TypeToMembers[type] = GetDataMembersForCaching(type).ToList();
	}
}
