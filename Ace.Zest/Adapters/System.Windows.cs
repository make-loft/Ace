using Ace;

using Xamarin.Forms;

// ReSharper disable once EmptyNamespace
namespace System.Windows.Markup
{
}

namespace System.Windows
{
	public delegate void PropertyChangedCallback(BindableObject d, DependencyPropertyChangedEventArgs e);
	public delegate bool ValidateValueCallback(object o);

	public class PropertyMetadata
	{
		public object DefaultValue { get; }
		public BindingMode BindingMode { get; set; }
		public ValidateValueCallback ValidateValueCallback;
		public PropertyChangedCallback PropertyChangedCallback;

		public PropertyMetadata() { }

		public PropertyMetadata(object defaultValue) => DefaultValue = defaultValue;

		public PropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback) : this(defaultValue) =>
			PropertyChangedCallback = propertyChangedCallback;

		public PropertyMetadata(PropertyChangedCallback propertyChangedCallback) =>
			PropertyChangedCallback = propertyChangedCallback;
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
		public void SetValue(DependencyProperty property, object value)
		{
			if (value.Is(out Binding b)) SetBinding(property.CoreProperty, b);
			else SetValue(property.CoreProperty, value);
		}
	}

	public class DependencyProperty
	{
		public static readonly object UnsetValue = new();

		public static DependencyProperty Register(string name, Type type, Type declaringType, PropertyMetadata m) =>
			new DependencyProperty
			{
				CoreProperty = BindablePropertyExtensions.Register(name, type, declaringType, m)
			};

		public static DependencyProperty RegisterAttached(string name, Type type, Type declaringType, PropertyMetadata m) =>
			new DependencyProperty
			{
				CoreProperty = BindablePropertyExtensions.RegisterAttached(name, type, declaringType, m)
			};

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

	public static class BindablePropertyExtensions
	{
		public static BindableProperty
			Register(string name, Type type, Type declaringType, PropertyMetadata m) => m.Is()
			? BindableProperty.Create(name, type, declaringType, m.DefaultValue, m.BindingMode, null,
				(b, oldValue, newValue) =>
					m.PropertyChangedCallback?.Invoke(b, new DependencyPropertyChangedEventArgs(oldValue, newValue)))
			: BindableProperty.Create(name, type, declaringType);

		public static BindableProperty
			RegisterAttached(string name, Type type, Type declaringType, PropertyMetadata m) => m.Is()
			? BindableProperty.CreateAttached(name, type, declaringType,
				m.DefaultValue ?? GetValidDefaultValue(type), m.BindingMode, (o, t) => m?.ValidateValueCallback?.Invoke(o) ?? true,
				(b, oldValue, newValue) =>
					m.PropertyChangedCallback?.Invoke(b, new DependencyPropertyChangedEventArgs(oldValue, newValue)))
			: BindableProperty.CreateAttached(name, type, declaringType, GetValidDefaultValue(type));

		private static object GetValidDefaultValue(Type type) => type.IsValueType ? Activator.CreateInstance(type) : default;
	}
}