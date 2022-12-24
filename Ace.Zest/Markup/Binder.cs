using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Ace.Markup
{
	using Convert = Patterns.Convert;

	class Binder : Binding, IValueConverter
	{
		public Binder() => Converter = this;

		public static readonly Convert NotImplementedException = args => throw new NotImplementedException();

		public event Convert Convert;
		public event Convert ConvertBack;

		object Xamarin.Forms.IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			(Convert ?? ConvertBack ?? NotImplementedException)(new(value, targetType, parameter, culture));

		object Xamarin.Forms.IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			(ConvertBack ?? Convert ?? NotImplementedException)(new(value, targetType, parameter, culture));
	}
}
