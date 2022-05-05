using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Data;
using System.Windows.Markup;
#endif

namespace Ace.Converters
{
	[ContentProperty("Converters")]
	public class AggregateConverter : IValueConverter
	{
		public bool BackReverse { get; set; } = true;

		public List<IValueConverter> Converters { get; } = new();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			Converters.Aggregate(value, (v, c) => c.Convert(v, targetType, parameter, culture));

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			(BackReverse ? Converters.Reverse<IValueConverter>() : Converters)
			.Aggregate(value, (v, c) => c.ConvertBack(v, targetType, parameter, culture));
	}
}