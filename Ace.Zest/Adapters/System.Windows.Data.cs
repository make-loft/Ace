﻿using Ace;

using System.Reflection;
using Xamarin.Forms;

namespace Xamarin.Forms
{
	public static class BindingOperations
	{
        static readonly MethodInfo getContextMethod = TypeOf<BindableObject>.Raw.GetTypeInfo().GetDeclaredMethod("GetContext");
        static readonly FieldInfo expressionField = TypeOf<Binding>.Raw.GetTypeInfo().GetDeclaredField("_expression");

        public static Binding GetBinding(this BindableObject o, BindableProperty p)
        {
            var context = getContextMethod.Invoke(o, new[] { p });
            var bindingField = context.GetType().GetTypeInfo().GetDeclaredField("Binding");
            return (Binding)bindingField.GetValue(context);
        }

        public static object GetBindingExpression(this Binding b) => expressionField.GetValue(b);
        public static void ClearBinding(this BindableObject o, BindableProperty p) => o.RemoveBinding(p);
		public static void SetBinding(this BindableObject o, BindableProperty p, Binding b) => o.SetBinding(p, b);
		public static void SetBinding(this BindableObject o, BindableProperty p, System.Windows.Data.Binding b) =>
			o.SetBinding(p, b.CoreBinding);
	}
}

namespace System.Windows
{
    public class PropertyPath
    {
        public string Path { get; }
        public PropertyPath(string path) => Path = path;
		public PropertyPath(BindableProperty property) => Path = property.PropertyName;
	}
}

namespace System.Windows.Data
{
    public class PathConverter : TypeConverter
    {
        public override bool CanConvertFrom(Type sourceType) => sourceType == typeof(string);
        public override object ConvertFromInvariantString(string value) => value.Is() ? new PropertyPath(value) : default;
    }

	public interface IValueConverter : Xamarin.Forms.IValueConverter { }

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

    public class Binding : Xamarin.Forms.Xaml.IMarkupExtension
    {
        public const string IndexerName = "Item[]";

        public static readonly object DoNothing = default;

        public readonly Xamarin.Forms.Binding CoreBinding = new Xamarin.Forms.Binding();

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
            get => (IValueConverter) CoreBinding.Converter;
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

        public object RelativeSource { get; set; }
        public object FallbackValue { get; set; }
    }
}