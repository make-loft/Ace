using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Art.Replication.Models
{
    public class ContractProfile : GeneralProfile
    {
        public override List<MemberInfo> GetDataMembers(Type type, Func<MemberInfo, bool> filter)
        {
            var allMembers = base.GetDataMembers(type, filter);
            var serializableAttribute = type.GetCustomAttributes(true)
                .FirstOrDefault(a => a.GetType().Name.Contains("SerializableAttribute"));
            var dataMemberToAttribute = serializableAttribute != null
                ? allMembers
                    .ToDictionary(i => i, i => (DataMemberAttribute) null)
                    .ToList()
                : allMembers
                    .ToDictionary(i => i, i => i.GetCustomAttribute<DataMemberAttribute>())
                    .Where(p => p.Value != null)
                    .OrderBy(p => p.Value.Order)
                    .ToList();
            return dataMemberToAttribute.Select(p => p.Key).ToList();
        }


        public override string GetDataKey(MemberInfo member) =>
            member.GetCustomAttribute<DataMemberAttribute>()?.Name ?? member.Name;
    }
}
