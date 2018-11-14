using Xamarin.Forms;

// ReSharper disable once EmptyNamespace
namespace System.Windows.Markup
{
}

namespace System.Windows
{
	public delegate void PropertyChangedCallback(BindableObject d, DependencyPropertyChangedEventArgs e);

	public class PropertyMetadata
    {
        public object DefaultValue { get; }
	    public PropertyChangedCallback PropertyChangedCallback;

		public PropertyMetadata(object defaultValue) => DefaultValue = defaultValue;

	    public PropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
	    {
		    DefaultValue = defaultValue;
		    PropertyChangedCallback = propertyChangedCallback;
	    }

	    public PropertyMetadata(PropertyChangedCallback propertyChangedCallback)
	    {
		    PropertyChangedCallback = propertyChangedCallback;
	    }
	}

	public class DependencyPropertyChangedEventArgs : EventArgs
	{
		public DependencyPropertyChangedEventArgs(object oldValue, object newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		public object OldValue { get; }
		public object NewValue { get; }
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
		    new DependencyProperty
		    {
			    CoreProperty = BindableProperty.Create(name, type, decType, metadata.DefaultValue, BindingMode.OneWay, null,
				    (b, oldValue, newValue) =>
					    metadata.PropertyChangedCallback?.Invoke(b, new DependencyPropertyChangedEventArgs(oldValue, newValue)))
		    };

        public static DependencyProperty RegisterAttached(string name, Type type, Type decType, PropertyMetadata metadata) =>
            Register(name, type, decType, metadata ?? new PropertyMetadata(null));

        public BindableProperty CoreProperty { get; private set; }

	    public BindableProperty Unbox() => CoreProperty;
    }

	public static class ElementAdapters
	{
		public static void SetValue(this BindableObject item, DependencyProperty property, object value) =>
			item.SetValue(property.CoreProperty, value);

		public static object GetValue(this BindableObject item, DependencyProperty property) =>
			item.GetValue(property.CoreProperty);
	}
}