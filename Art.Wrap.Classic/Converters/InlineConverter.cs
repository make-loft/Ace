using System;
using System.Globalization;
using System.Windows.Data;
using Art.Converters.Patterns;

namespace Art.Converters
{
    public class InlineConverter : IInlineConverter, ICompositeConverter
    {
        public IValueConverter PostConverter { get; set; }
        public object PostConverterParameter { get; set; }
        public event EventHandler<ConverterEventArgs> Converting;
        public event EventHandler<ConverterEventArgs> ConvertingBack;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var args = new ConverterEventArgs(value, targetType, parameter, culture);
            Converting?.Invoke(this, args);
            return PostConverter == null
                ? args.ConvertedValue
                : PostConverter.Convert(args.ConvertedValue, targetType, PostConverterParameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var args = new ConverterEventArgs(value, targetType, parameter, culture);
            ConvertingBack?.Invoke(this, args);
            return PostConverter == null
                ? args.ConvertedValue
                : PostConverter.ConvertBack(args.ConvertedValue, targetType, PostConverterParameter, culture);
        }
    }
}