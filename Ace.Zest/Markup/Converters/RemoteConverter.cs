using System;
using System.Globalization;
#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Data;
#endif

namespace Ace.Markup.Converters
{
	public class RemoteConverter : IValueConverter
	{
		public IValueConverter Source { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			Source?.Convert(value, targetType, parameter, culture);

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			Source?.ConvertBack(value, targetType, parameter, culture);
	}
}