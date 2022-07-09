using System;
using System.Globalization;
#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Data;
#endif

namespace Ace.Markup.Converters
{
	public delegate object Convert(ConvertArgs args);

	public struct ConvertArgs
	{
		public readonly object Value;
		public readonly object Parameter;
		public readonly Type TargetType;
		public readonly CultureInfo Culture;

		public ConvertArgs(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Value = value; TargetType = targetType; Parameter = parameter; Culture = culture;
		}
	}

	public class Converter : Patterns.AMarkupExtension, IValueConverter
	{
		public static readonly Convert NotImplementedException = args => throw new NotImplementedException();

		public event Convert Convert;
		public event Convert ConvertBack;

		public Converter() { }
		public Converter(Convert convert) => Convert = convert;
		public Converter(Convert convert, Convert convertBack) : this(convert) => ConvertBack = convertBack;

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			(Convert ?? ConvertBack ?? NotImplementedException)(new(value, targetType, parameter, culture));

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			(ConvertBack ?? Convert ?? NotImplementedException)(new(value, targetType, parameter, culture));

		public override object Provide(object targetObject, object targetProperty = default) => this;
	}
}