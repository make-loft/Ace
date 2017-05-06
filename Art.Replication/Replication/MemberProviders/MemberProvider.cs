﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Art.Replication.MemberProviders
{
    public class MemberProvider
    {
        public virtual bool CanApply(Type type) => true;

        public virtual string GetDataKey(MemberInfo member) => member.Name;

        protected virtual IEnumerable<MemberInfo> GetDataMembersInternal(Type type) => type.GetMembers();

        protected Dictionary<Type, List<MemberInfo>> TypeToMembers = new Dictionary<Type, List<MemberInfo>>();

        public virtual List<MemberInfo> GetDataMembers(Type type) =>
            TypeToMembers.TryGetValue(type, out var members)
                ? members
                : TypeToMembers[type] = GetDataMembersInternal(type).ToList();
    }
}
