﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Art.Replication.Patterns;

namespace Art.Replication.Models
{
    public class GeneralProfile : ADataProfile
    {
        private static readonly Type EnumerableType = typeof(IEnumerable);

        public override List<MemberInfo> GetDataMembers(Type type, Func<MemberInfo, bool> filter) =>
            type.Name.StartsWith("KeyValuePair") || type == typeof(DictionaryEntry)
                ? type.GetMembers().Where(m => m is PropertyInfo).ToList()
                : type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => !EnumerableType.IsAssignableFrom(type) && m.Name != "Item")
                    .Where(filter)
                    .ToList();

        public override string GetDataKey(MemberInfo member) => member.Name;
    }
}
