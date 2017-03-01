using System.Reflection;

namespace Art.Replication
{
    public static partial class Sugar
    {
        public static bool CanRead(this MemberInfo member)
        {
            var property = member as PropertyInfo;
            return property != null ? property.CanRead : member is FieldInfo;
        }

        public static bool CanWrite(this MemberInfo member)
        {
            var property = member as PropertyInfo;
            return property != null ? property.CanWrite : member is FieldInfo;
        }

        public static bool CanReadWrite(this MemberInfo member) => CanRead(member) && CanWrite(member);

        public static object GetValue(this MemberInfo member, object obj)
        {
            var property = member as PropertyInfo;
            return property != null ? property.GetValue(obj, null) : (member as FieldInfo)?.GetValue(obj);
        }

        public static void SetValue(this MemberInfo member, object obj, object value)
        {
            var property = member as PropertyInfo;
            if (property != null) property.SetValue(obj, value);
            var field = member as FieldInfo;
            if (field != null) field.SetValue(obj, value);
        }
    }
}
