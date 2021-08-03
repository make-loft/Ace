using System;
using System.Globalization;
#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Data;
#endif

namespace Ace.Converters
{
	public delegate object Convert(object value, Type targetType, object parameter, CultureInfo culture);

	public class Converter : IValueConverter
	{
		public static readonly Convert NotImplementedException = (_, _, _, _) => throw new NotImplementedException();

		public event Convert Convert;
		public event Convert ConvertBack;

		public Converter() { }
		public Converter(Convert convert) => Convert = convert;
		public Converter(Convert convert, Convert convertBack) : this(convert) => ConvertBack = convertBack;

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			(Convert ?? NotImplementedException)(value, targetType, parameter, culture);

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			(ConvertBack ?? NotImplementedException)(value, targetType, parameter, culture);
	}
}