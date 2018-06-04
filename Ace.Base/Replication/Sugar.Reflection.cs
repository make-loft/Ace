using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ace.Replication
{
	public static class Member
	{
		public const BindingFlags DefaultFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
		
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
			switch (member)
			{
				case PropertyInfo property:
					property.SetValue(obj, value, null);
					break;
				case FieldInfo field:
					field.SetValue(obj, value);
					break;
			}
		}

		private static readonly string DictionaryTypeName = TypeOf.Dictionary.Name;
		private static readonly Assembly DictionaryTypeAssembly = TypeOf.Dictionary.Assembly;
		
		public static bool IsGenericDictionaryWithKey<TKey>(this Type type) =>
			type.Name.Is(DictionaryTypeName) &&
			type.Assembly.Is(DictionaryTypeAssembly) &&
			type.GetGenericArguments()[0].Is(TypeOf<TKey>.Raw);

		public static IEnumerable<MemberInfo> EnumerateMembers(this Type type, BindingFlags bindingFlags) =>
			type.BaseType?.EnumerateMembers(bindingFlags)
				.Concat(type.GetMembers(bindingFlags | BindingFlags.DeclaredOnly)) ??
			type.GetMembers(bindingFlags);
		
		public static IEnumerable<MemberInfo> EnumerateMember(this Type type, string name, BindingFlags bindingFlags) =>
			type.BaseType?.EnumerateMember(name, bindingFlags)
				.Concat(type.GetMember(name, bindingFlags | BindingFlags.DeclaredOnly)) ??
			type.GetMember(name, bindingFlags);
	}
}
