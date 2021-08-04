using System;
using System.Globalization;
#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Data;
#endif

namespace Ace.Converters.Patterns
{
	public abstract class AValueConverter : IValueConverter
	{
		public static object Stub() => throw new NotImplementedException();

		public virtual object Convert(object value) => Stub();
		public virtual object Convert(object value, object parameter) => Convert(value);
		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			Convert(value, parameter);

		public virtual object ConvertBack(object value) => Stub();
		public virtual object ConvertBack(object value, object parameter) => ConvertBack(value);
		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			ConvertBack(value, parameter);

		public abstract class Reflected : IValueConverter
		{
			public virtual object Convert(object value) => Stub();
			public virtual object Convert(object value, object parameter) => Convert(value);
			public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
				Convert(value, parameter);

			object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
				Convert(value, targetType, parameter, culture);
		}
	}
}