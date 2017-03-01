using System;
using System.Collections.Generic;
using System.Reflection;

namespace Art.Wiz.Patterns
{
    public abstract class ADataProfile
    {
        public abstract List<MemberInfo> GetDataMembers(Type type);

        public abstract string GetDataKey(MemberInfo member);
    }
}
