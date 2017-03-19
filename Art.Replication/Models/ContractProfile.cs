using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Art.Replication.Patterns;

namespace Art.Replication.Models
{
    public class ContractProfile : ADataProfile
    {
        public override List<MemberInfo> GetDataMembers(Type type)
        {
            var serializableAttribute = type.GetCustomAttributes(true).FirstOrDefault(a=>a.GetType().Name.Contains(SerializableAttribute)))
            var dataMemberToAttribute = serializableAttribute != null
                ? type.GetMembers().Where(m=>m is FieldInfo || m is PropertyInfo).ToDictionary(i => i, i => (DataMemberAttribute)null).ToList()
                : type.GetMembers().ToDictionary(i => i, i => i.GetCustomAttribute<DataMemberAttribute>()).Where(p => p.Value != null)
                .OrderBy(p => p.Value.Order).ToList();
            return dataMemberToAttribute.Select(p => p.Key).ToList();
        }


        public override string GetDataKey(MemberInfo member) =>
            member.GetCustomAttribute<DataMemberAttribute>()?.Name ?? member.Name;
    }
}
