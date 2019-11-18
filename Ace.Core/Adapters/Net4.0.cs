#if NET40

using System.Collections;

// ReSharper disable once CheckNamespace

namespace System.ComponentModel
{
    public class DataErrorsChangedEventArgs : EventArgs
    {
        public DataErrorsChangedEventArgs(string propertyName) => PropertyName = propertyName;

        public virtual string PropertyName { get; }
    }

    public interface INotifyDataErrorInfo
    {
        bool HasErrors { get; }

        IEnumerable GetErrors(string propertyName);

        event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}

namespace System.Reflection
{
    public static class ReflectionExtensions
    {
        public static MethodInfo GetRuntimeMethod(this Type type, string name, Type[] types) => type.GetMethod(name, types);  
        public static EventInfo GetRuntimeEvent(this Type type, string name) => type.GetEvent(name);
    }
}

namespace System.Runtime.CompilerServices
{
	internal class CallerMemberNameAttribute : Attribute
	{ }
}
#endif