using System;
using System.Reflection;

namespace System;
#if !MAUI && !XAMARIN
public static class AttributeE
{
    public static bool IsDefined(Type type, Type attributeType)
    {
        return true; //type.GetTypeInfo().Attributes == TypeAttributes.Serializable;
    }
}

public static class ReflectionE
{
    public static PropertyInfo GetProperty(this Type type, string name)
    {
        return type.GetTypeInfo().GetDeclaredProperty(name);
    }
}
#endif