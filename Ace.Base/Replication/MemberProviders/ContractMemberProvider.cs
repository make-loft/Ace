using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ace.Replication.MemberProviders
{
	public class ContractMemberProvider : CoreMemberProvider
	{
		public ContractMemberProvider(BindingFlags bindingFlags, Func<MemberInfo, bool> filter) :
			base(bindingFlags, filter)
		{
		}

		protected override IEnumerable<MemberInfo> GetDataMembersForCaching(Type type)
		{
			var members = base.GetDataMembersForCaching(type);
			var hasContract =
				type.IsDefined(TypeOf<DataContractAttribute>.Raw, true) ||
				type.IsDefined(TypeOf<CollectionDataContractAttribute>.Raw, true);
			return hasContract
				? members
					.ToDictionary(m => m, m => m.GetCustomAttribute<DataMemberAttribute>())
					.Where(a => a.Value.Is())
					.OrderBy(a => a.Value.Order)
					.Select(a => a.Key)
				: members;
		}

		protected readonly Dictionary<MemberInfo, string> MemberToKey = new();

		public override string GetCustomKey(MemberInfo member) => MemberToKey.TryGetValue(member, out var key)
			? key
			: MemberToKey[member] = member.GetCustomAttribute<DataMemberAttribute>()?.Name ?? member.Name;
	}
}
