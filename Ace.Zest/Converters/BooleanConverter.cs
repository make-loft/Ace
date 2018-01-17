using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ace.Converters
{
	public class BooleanConverter : DependencyObject, IValueConverter
	{
		public static readonly DependencyProperty OnTrueProperty =
			DependencyProperty.Register("OnTrue", typeof (object), typeof (BooleanConverter),
				new PropertyMetadata(default(object)));

		public static readonly DependencyProperty OnFalseProperty =
			DependencyProperty.Register("OnFalse", typeof (object), typeof (BooleanConverter),
				new PropertyMetadata(default(object)));

		public static readonly DependencyProperty OnNullProperty =
			DependencyProperty.Register("OnNull", typeof (object), typeof (BooleanConverter),
				new PropertyMetadata(default(object)));

		public static readonly DependencyProperty OnNotNullProperty =
			DependencyProperty.Register("OnNotNull", typeof (object), typeof (BooleanConverter),
				new PropertyMetadata(default(object)));

		public object OnTrue
		{
			get => GetValue(OnTrueProperty);
			set => SetValue(OnTrueProperty, value);
		}

		public object OnFalse
		{
			get => GetValue(OnFalseProperty);
			set => SetValue(OnFalseProperty, value);
		}

		public object OnNull
		{
			get => GetValue(OnNullProperty);
			set => SetValue(OnNullProperty, value);
		}

		public object OnNotNull
		{
			get => GetValue(OnNotNullProperty);
			set => SetValue(OnNotNullProperty, value);
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return OnNull;
			if (OnTrue == null && OnFalse == null) return OnNotNull;
			if (string.Equals(value.ToString(), true.ToString(), StringComparison.CurrentCultureIgnoreCase))
				return OnTrue;
			if (string.Equals(value.ToString(), false.ToString(), StringComparison.CurrentCultureIgnoreCase))
				return OnFalse;
			return OnNotNull;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == OnNull) return Default(targetType);
			if (value == OnNotNull) return Default(targetType);
			if (value == OnFalse) return false;
			if (value == OnTrue) return true;
			if (value == null) return null;

			if (OnNull != null &&
				string.Equals(value.ToString(), OnNull.ToString(), StringComparison.CurrentCultureIgnoreCase))
				return Default(targetType);

			if (OnFalse != null &&
				string.Equals(value.ToString(), OnFalse.ToString(), StringComparison.CurrentCultureIgnoreCase))
				return false;

			if (OnTrue != null &&
				string.Equals(value.ToString(), OnTrue.ToString(), StringComparison.CurrentCultureIgnoreCase))
				return true;

			return null;
		}

		public static object Default(Type type) => type.IsByRef ? null : Activator.CreateInstance(type);
	}
}