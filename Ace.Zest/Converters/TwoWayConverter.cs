using System;
using System.Globalization;
#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Data;
#endif

namespace Ace.Converters
{
	public class TwoWayConverter : IValueConverter
	{
		public IValueConverter GetConverter { get; set; }
		public IValueConverter SetConverter { get; set; }

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			GetConverter.Convert(value, targetType, parameter, culture);

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			SetConverter.Convert(value, targetType, parameter, culture);
	}
}