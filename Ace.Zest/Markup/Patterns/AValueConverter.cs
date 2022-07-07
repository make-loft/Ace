using System;
using System.Globalization;
#if XAMARIN
using IValueConverter = Xamarin.Forms.IValueConverter;
#else
using System.Windows.Data;
#endif
using System.Windows;

namespace Ace.Markup.Patterns
{
	public abstract class AValueConverter : DependencyObject, IValueConverter
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

		public abstract class Reflected : DependencyObject, IValueConverter
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