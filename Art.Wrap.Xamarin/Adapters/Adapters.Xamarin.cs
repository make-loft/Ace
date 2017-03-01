using System;
using System.Reflection;
using System.Windows;
using Xamarin.Forms;

namespace System.Windows
{
    public class PropertyMetadata
    {
        public object DefaultValue { get; private set; }

        public PropertyMetadata(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}

namespace System.Windows.Data
{
    public interface IValueConverter : Xamarin.Forms.IValueConverter
    {}
}

// ReSharper disable once EmptyNamespace
namespace System.Windows.Markup
{


}

namespace Aero
{
    public class DependencyPropertyChangedEventArgs : EventArgs
    {
        
    }

    public class DependencyObject : BindableObject
    {
        public object GetValue(DependencyProperty property)
        {
            return GetValue(property.XamarinBase);
        }

        public void SetValue(DependencyProperty property, object value)
        {
            SetValue(property.XamarinBase, value);
        }
    }

    public class DependencyProperty
    {
        public static DependencyProperty Register(string name, Type type, Type decType, PropertyMetadata metadata)
        {
            return new DependencyProperty {XamarinBase = BindableProperty.Create(name, type, decType, metadata.DefaultValue)};
        }

        public static DependencyProperty RegisterAttached(string name, Type type, Type decType,
            PropertyMetadata metadata)
        {
            return new DependencyProperty { XamarinBase = BindableProperty.Create(name, type, decType, metadata.DefaultValue) };
        }

        public BindableProperty XamarinBase { get; set; }
    }

    public static class Adapters
    {
        public static MethodInfo GetMethod(this Type type, string methodName)
        {
            return type.GetTypeInfo().GetDeclaredMethod(methodName);
        }
    }
}