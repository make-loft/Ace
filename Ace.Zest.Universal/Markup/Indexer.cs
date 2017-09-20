using System;
using Windows.UI.Xaml.Data;

namespace Aero.Markup
{
    public class Indexer : IValueConverter
    {
        public string StoreKey { get; set; }

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var context = StoreKey == null
                ? value as ContextObject
                : new Store().Convert(null, null, StoreKey, string.Empty) as ContextObject;
            if (context == null) return null;
            var indexProperty = (string) parameter;
            value = context[indexProperty];
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var context = StoreKey == null
                ? value as ContextObject
                : new Store().Convert(null, null, StoreKey, string.Empty) as ContextObject;
            if (context == null) return null;
            var indexProperty = (string)parameter;
            context[indexProperty] = value;
            return indexProperty;
        }
    }
}
