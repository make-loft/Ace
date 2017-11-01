using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace System.Windows
{
    public class PropertyPath
    {
        public string Path { get; }

        public PropertyPath(string path) => Path = path;
    }
}

namespace System.Windows.Data
{
    public class PathConverter : TypeConverter
    {
        public static readonly List<Assembly> RegisteredAssemblies = new List<Assembly>();

        public override bool CanConvertFrom(Type sourceType) => sourceType == typeof(string);

        public override object ConvertFromInvariantString(string value) => new PropertyPath(value);
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
        public RelativeSourceMode Mode { get; set; }
    }

    public static class BindingOperations
    {
        public static void SetBinding(DependencyObject o, DependencyProperty p, Binding b) => 
            o.SetBinding(p.CoreProperty, b.CoreBinding);
    }

    public class Binding : Xamarin.Forms.Xaml.IMarkupExtension
    {
        public readonly Xamarin.Forms.Binding CoreBinding = new Xamarin.Forms.Binding();

        public object ProvideValue(IServiceProvider serviceProvider) => CoreBinding;

        public Binding()
        {
        }

        public Binding(string path)
        {
            Path = new PropertyPath(path);
        }

        [TypeConverter(typeof(PathConverter))]
        public PropertyPath Path
        {
            get => new PropertyPath(CoreBinding.Path);
            set => CoreBinding.Path = value.Path;
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