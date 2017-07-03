using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Art.Replication.MemberProviders
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

        public CoreMemberProvider(BindingFlags bindingFlags, Func<MemberInfo, bool> filter)
        {
            BindingFlags = bindingFlags;
            Filter = filter;
        }

        private static readonly Type EnumerableType = typeof(IEnumerable);

        protected override IEnumerable<MemberInfo> GetDataMembersForCaching(Type type) =>
            type.EnumerateMembers(BindingFlags)
                .Where(Filter)
                .Where(m => !EnumerableType.IsAssignableFrom(type) && m.Name != "Item");
    }
}
