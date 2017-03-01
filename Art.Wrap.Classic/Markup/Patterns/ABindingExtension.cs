using System;
using System.Globalization;
using System.Windows.Data;

namespace Aero.Markup.Patterns
{
    public abstract class ABindingExtension : Binding, IValueConverter
    {
        protected ABindingExtension()
        {
            Source = Converter = this;
        }

        protected ABindingExtension(object source) // Source, RelativeSource, null for DataContext
        {
            var relativeSource = source as RelativeSource;
            if (relativeSource == null && source != null) Source = source;
            else RelativeSource = relativeSource;
            Converter = this;
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}