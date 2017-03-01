﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Art.Wiz.Patterns;

namespace Art.Replication.Models.Data
{
    public class GeneralProfile : ADataProfile
    {
        private static readonly Type EnumerableType = typeof(IEnumerable);

        public override List<MemberInfo> GetDataMembers(Type type) => type.Name.StartsWith("KeyValuePair")
            ? type.GetMembers().Where(m => m is PropertyInfo).ToList()
            : type.GetMembers()
                .Where(Sugar.CanReadWrite)
                .Where(m => !EnumerableType.IsAssignableFrom(type) && m.Name != "Item")
                .ToList();

        public override string GetDataKey(MemberInfo member) => member.Name;
    }
}
