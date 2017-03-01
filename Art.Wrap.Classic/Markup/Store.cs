using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Aero.Markup
{
    public class Store : Patterns.ABindingExtension
    {
        public Store() : base(new RelativeSource {Mode = RelativeSourceMode.Self})
        {
        }

        public Store(Type key) : base(new RelativeSource { Mode = RelativeSourceMode.Self })
        {
            Key = key;
        }

        [TypeConverter(typeof (XamlTypeConverter))]
        public Type Key { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var itemType = Key;
            var methodInfo = typeof (Aero.Store).GetMethod("Get").
                MakeGenericMethod(itemType.DeclaringType ?? itemType);
            var item = methodInfo.Invoke(null, new object[] {new object[0]});

            RoutedCommandsAdapter.SetCommandBindings(value, item);

            return item;
        }
    }
}