using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aero.Markup.Base
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

    public abstract class BindingExtension : IMarkupExtension, IValueConverter
    {
        private Binding _binding = new Binding();

        protected BindingExtension()
        {
            _binding.Source = _binding.Converter = this;
        }

        protected BindingExtension(object source) // set Source to null for using DataContext
        {
            _binding.Source = source;
            _binding.Converter = this;
        }

        protected BindingExtension(RelativeSource relativeSource)
        {
            _binding.Source = relativeSource;
            _binding.Converter = this;
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return _binding;
        }
    }
}
