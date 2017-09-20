using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ace.Replication.MemberProviders
{
    public class CoreMemberProviderForKeyValuePair : MemberProvider
    {
        public override bool CanApply(Type type) => type == typeof(KeyValuePair<,>) || type == typeof(DictionaryEntry);

        protected override IEnumerable<MemberInfo> GetDataMembersForCaching(Type type) =>
            type.GetMembers().Where(m => m is PropertyInfo);
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

        protected override IEnumerable<MemberInfo> GetDataMembersForCaching(Type type) =>
            type.EnumerateMembers(BindingFlags)
                .Where(Filter)
                .Where(m => !typeof(IEnumerable).IsAssignableFrom(type) && m.Name != "Item");
    }
}
