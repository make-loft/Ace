using System;
using Windows.UI.Xaml.Data;

namespace Aero.Markup.Base
{
    public abstract class BindingExtension : Binding, IValueConverter
    {
        protected BindingExtension()
        {
            Source = Converter = this;
        }

        protected BindingExtension(object source) // set Source to null for using DataContext
        {
            Source = source;
            Converter = this;
        }

        protected BindingExtension(RelativeSource relativeSource)
        {
            RelativeSource = relativeSource;
            Converter = this;
        }

        public abstract object Convert(object value, Type targetType, object parameter, string culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
