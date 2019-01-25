using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
#if XAMARIN
using ContentProperty = Xamarin.Forms.ContentPropertyAttribute;
#endif

namespace Ace.Converters
{
    [ContentProperty("Converters")]
    public class AggregateConverter : IValueConverter
    {
        public List<IValueConverter> Converters { get; } = new List<IValueConverter>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Converters.Aggregate(value, (v, c) => c.Convert(v, targetType, parameter, culture));

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
