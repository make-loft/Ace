using Ace;

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
#if MAUI
using _Binding = Microsoft.Maui.Controls.Binding;
#endif
#if XAMARIN
using _Binding = Xamarin.Forms.Binding;
using TypeConverter = Xamarin.Forms.TypeConverter;
using TypeConverterAttribute = Xamarin.Forms.TypeConverterAttribute;
#endif

namespace System.Windows
{
	public class PropertyPath
	{
		public string Path { get; }
		public PropertyPath(string path) => Path = path;
		public PropertyPath(BindableProperty property) => Path = property.PropertyName;
	}
	public class PathConverter : TypeConverter
	{
#if MAUI
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			=> sourceType == typeof(string);
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			 => value.Is() ? new PropertyPath(value as string) : default;
#endif
#if XAMARIN
		public override bool CanConvertFrom(Type sourceType)
            => sourceType == typeof(string);
		public override object ConvertFrom(CultureInfo culture, object value)
			 => value.Is() ? new PropertyPath(value as string) : default;
#endif
#if DESKTOP
        public override bool CanConvertFrom(Type sourceType) => sourceType == typeof(string);
    public override object ConvertFromInvariantString(string value) => value.Is() ? new PropertyPath(value) : default;
#endif
	}
}

namespace System.Windows.Data
{

	public static class BindingOperations
	{
		static readonly MethodInfo getContextMethod = TypeOf<BindableObject>.Raw.GetTypeInfo().GetDeclaredMethod("GetContext");
		static readonly FieldInfo expressionField = TypeOf<Binding>.Raw.GetTypeInfo().GetDeclaredField("_expression");

		public static _Binding GetBinding(this BindableObject o, BindableProperty p)
		{
			var context = getContextMethod.Invoke(o, [p]);
			var bindingField = context.GetType().GetTypeInfo().GetDeclaredField("Binding");
			return (_Binding)bindingField.GetValue(context);
		}

		public static object GetBindingExpression(this Binding b) => expressionField.GetValue(b);
		public static void ClearBinding(this BindableObject o, BindableProperty p) => o.RemoveBinding(p);
#if MAUI
		public static void SetBinding(this BindableObject o, BindableProperty p, Microsoft.Maui.Controls.Binding b)
			=> o.SetBinding(p, b);
#endif
#if XAMARIN
        public static void SetBinding(this BindableObject o, BindableProperty p, Xamarin.Forms.Binding b)
			=> o.SetBinding(p, b);
#endif
		public static void SetBinding(this BindableObject o, BindableProperty p, Binding b)
			=> o.SetBinding(p, b.CoreBinding);
	}

#if XAMARIN
    public interface IValueConverter : Xamarin.Forms.IValueConverter { }
#endif
#if MAUI
	public interface IValueConverter : Microsoft.Maui.Controls.IValueConverter { }
#endif

	public enum RelativeSourceMode
	{
		PreviousData,
		TemplatedParent,
		Self,
		FindAncestor
	}

	public class RelativeSource
	{
		public RelativeSource(RelativeSourceMode mode) => Mode = mode;
		public RelativeSourceMode Mode { get; set; }
	}

	public abstract class BindingGroup { }
	public abstract class BindingExpressionBase { }
	public class Binding : IMarkupExtension
	{
		public const string IndexerName = "Item[]";

		public static readonly object DoNothing = default;
#if XAMARIN
        public readonly Xamarin.Forms.Binding CoreBinding = new();
#endif
#if MAUI
		public readonly Microsoft.Maui.Controls.Binding CoreBinding = new();
#endif
		public object ProvideValue(IServiceProvider serviceProvider) => CoreBinding;

		public Binding()
		{
		}

		public Binding(string path) => Path = new PropertyPath(path);

		[TypeConverter(typeof(PathConverter))]
		public PropertyPath Path
		{
			get => CoreBinding.Path.Is(out var path) ? new PropertyPath(path) : default;
			set => CoreBinding.Path = value?.Path;
		}

		public object Source
		{
			get => CoreBinding.Source;
			set => CoreBinding.Source = value;
		}

		public BindingMode Mode
		{
			get => CoreBinding.Mode;
			set => CoreBinding.Mode = value;
		}

		public IValueConverter Converter
		{
			get => (IValueConverter)CoreBinding.Converter;
			set => CoreBinding.Converter = value;
		}

		public object ConverterParameter
		{
			get => CoreBinding.ConverterParameter;
			set => CoreBinding.ConverterParameter = value;
		}

		public string StringFormat
		{
			get => CoreBinding.StringFormat;
			set => CoreBinding.StringFormat = value;
		}

		public RelativeSource RelativeSource { get; set; }
		public object FallbackValue { get; set; }

		public string ElementName { get; set; }
	}
}