using Xamarin.Forms;

// ReSharper disable once EmptyNamespace
namespace System.Windows.Markup
{


}

namespace System.Windows
{
    public class PropertyMetadata
    {
        public object DefaultValue { get; }

        public PropertyMetadata(object defaultValue) => DefaultValue = defaultValue;
    }

    public class DependencyPropertyChangedEventArgs : EventArgs
    {
    }

    public class DependencyObject : BindableObject
    {
        public object GetValue(DependencyProperty property) => GetValue(property.CoreProperty);

        public void SetValue(DependencyProperty property, object value) => SetValue(property.CoreProperty, value);
    }

    public class DependencyProperty
    {
        public static readonly object UnsetValue = new object();

        public static DependencyProperty Register(string name, Type type, Type decType, PropertyMetadata metadata) =>
            new DependencyProperty {CoreProperty = BindableProperty.Create(name, type, decType, metadata.DefaultValue)};

        public static DependencyProperty RegisterAttached(string name, Type type, Type decType, PropertyMetadata metadata) =>
            new DependencyProperty {CoreProperty = BindableProperty.Create(name, type, decType, metadata.DefaultValue)};

        public BindableProperty CoreProperty { get; set; }
    }
}