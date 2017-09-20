using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Ace.Replication.MemberProviders
{
    public class ContractMemberProvider : CoreMemberProvider
    {
        public ContractMemberProvider(Adapters.BindingFlags bindingFlags, Func<MemberInfo, bool> filter) :
            base(bindingFlags, filter)
        {
        }

        protected override IEnumerable<MemberInfo> GetDataMembersForCaching(Type type)
        {
            var members = base.GetDataMembersForCaching(type);
            var hasContract = type.IsDefined(typeof(DataContractAttribute), true) ||
                              type.IsDefined(typeof(CollectionDataContractAttribute), true);
            return hasContract
                ? members
                    .ToDictionary(i => i, i => i.GetCustomAttribute<DataMemberAttribute>())
                    .Where(p => p.Value != null)
                    .OrderBy(p => p.Value.Order)
                    .Select(p => p.Key)
                : members;
        }

        protected readonly Dictionary<MemberInfo, string> MemberToKey = new Dictionary<MemberInfo, string>();

        public override string GetDataKey(MemberInfo member) =>
            MemberToKey.TryGetValue(member, out var key)
                ? key
                : MemberToKey[member] = member.GetCustomAttribute<DataMemberAttribute>()?.Name ?? member.Name;
    }
}
