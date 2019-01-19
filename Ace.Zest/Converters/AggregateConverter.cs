using Ace.Converters.Patterns;
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
    public class AggregateConverter : AValueConverter
    {
        public List<IValueConverter> Converters { get; } = new List<IValueConverter>();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var newValue = Converters.Aggregate(value, (v, c)=> c.Convert(v, targetType, parameter, culture));
            return GetDefined(newValue, value);
        }
    }
}
