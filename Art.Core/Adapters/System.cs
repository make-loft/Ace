using System;
using System.Reflection;

// ReSharper disable once CheckNamespace

namespace Aero
{
    public static class Attribute
    {
        public static bool IsDefined(Type type, Type attributeType)
        {
            return true; //type.GetTypeInfo().Attributes == TypeAttributes.Serializable;
        }
    }

    public static class Reflection
    {
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredProperty(name);
        }
    }
}