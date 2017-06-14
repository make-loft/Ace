using System;
using System.Collections.Generic;
using System.Reflection;

namespace Art.Replication
{
    public static class Member
    {
        public static bool CanRead(this MemberInfo member) =>
            (member as PropertyInfo)?.CanRead ?? member is FieldInfo;

        public static bool CanWrite(this MemberInfo member) =>
            (member as PropertyInfo)?.CanWrite ?? member is FieldInfo;

        public static bool CanReadWrite(this MemberInfo member) =>
            CanRead(member) && CanWrite(member);

        public static object GetValue(this MemberInfo member, object obj) =>
            member is PropertyInfo property
                ? property.GetValue(obj, null)
                : (member as FieldInfo)?.GetValue(obj);

        public static Type GetMemberType(this MemberInfo member) =>
            (member as PropertyInfo)?.PropertyType ?? (member as FieldInfo)?.FieldType;

        public static void SetValue(this MemberInfo member, object obj, object value)
        {
            if (member is PropertyInfo property) property.SetValue(obj, value, null);
            else if (member is FieldInfo field) field.SetValue(obj, value);
        }

        public static void SetValueIfCanWrite(this MemberInfo member, object obj, object value)
        {
            if (!member.CanWrite()) return;
            if (member is PropertyInfo property) property.SetValue(obj, value, null);
            else if (member is FieldInfo field) field.SetValue(obj, value);
        }

        public static bool IsGenericDictionaryWithKey<TKey>(this Type type) =>
            type.Name == typeof(Dictionary<,>).Name &&
            type.Assembly == typeof(Dictionary<,>).Assembly &&
            type.GetGenericArguments()[0] == typeof(TKey);
    }
}
