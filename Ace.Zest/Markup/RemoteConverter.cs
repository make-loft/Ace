using System;
using System.Globalization;
using System.Windows.Data;

namespace Ace.Markup
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