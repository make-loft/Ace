using System;
using System.Globalization;
using System.Windows.Data;
using Aero;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Binding = System.Windows.Data.Binding;
using IValueConverter = Xamarin.Forms.IValueConverter;

namespace System.Windows
{
    public class PropertyPath
    {
        public string Path { get; private set; }

        public PropertyPath(string path)
        {
            Path = path;
        }
    }
}

namespace System.Windows.Data
{
    public enum RelativeSourceMode
    {
        PreviousData,
        TemplatedParent,
        Self,
        FindAncestor,
    }

    public class RelativeSource
    {
        public RelativeSourceMode Mode { get; set; }
    }
}

namespace System.Windows.Data
{
    public static class BindingOperations
    {
        public static void SetBinding(DependencyObject o, DependencyProperty p, Binding b)
        {
            o.SetBinding(p.XamarinBase, b.XamarinBase);
        }
    }

    public class Binding
    {
        public readonly Xamarin.Forms.Binding XamarinBase = new Xamarin.Forms.Binding();

        public Binding()
        {
        }

        public Binding(string path)
        {
            Path = new PropertyPath(path);
        }

        public PropertyPath Path
        {
            get { return new PropertyPath(XamarinBase.Path); }
            set { XamarinBase.Path = value.Path; }
        }

        public object Source
        {
            get { return XamarinBase.Source; }
            set { XamarinBase.Source = value; }
        }

        public BindingMode Mode
        {
            get { return XamarinBase.Mode; }
            set { XamarinBase.Mode = value; }
        }

        public IValueConverter Converter
        {
            get { return (IValueConverter) XamarinBase.Converter; }
            set { XamarinBase.Converter = value; }
        }

        public object ConverterParameter
        {
            get { return XamarinBase.ConverterParameter; }
            set { XamarinBase.ConverterParameter = value; }
        }
    }
}

namespace Aero.Markup.Patterns
{
    public abstract class ABindingExtension : Binding, IMarkupExtension, IValueConverter
    {
        protected ABindingExtension()
        {
            XamarinBase.Source = XamarinBase.Converter = this;
        }

        protected ABindingExtension(object source) // set Source to null for using DataContext
        {
            XamarinBase.Source = source;
            XamarinBase.Converter = this;
        }

        protected ABindingExtension(RelativeSource relativeSource)
        {
            XamarinBase.Source = relativeSource;
            XamarinBase.Converter = this;
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return XamarinBase;
        }
    }
}