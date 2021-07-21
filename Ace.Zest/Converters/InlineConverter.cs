using System;
using System.Globalization;
using System.Windows.Data;
using Ace.Converters.Patterns;
using ConvertingHandler = System.EventHandler<Ace.Converters.Patterns.ConverterEventArgs>;

namespace Ace.Converters
{
	public class InlineConverter : IInlineConverter, ICompositeConverter
	{
		public IValueConverter PostConverter { get; set; }
		public object PostConverterParameter { get; set; }
		public event ConvertingHandler Converting;
		public event ConvertingHandler ConvertingBack;

		public InlineConverter() { }

		public InlineConverter(ConvertingHandler converting = default, ConvertingHandler convertingBack = default)
		{
			Converting = converting;
			ConvertingBack = convertingBack;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var args = new ConverterEventArgs(value, targetType, parameter, culture);
			Converting?.Invoke(this, args);
			return PostConverter.IsNot()
				? args.ConvertedValue
				: PostConverter.Convert(args.ConvertedValue, targetType, PostConverterParameter, culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var args = new ConverterEventArgs(value, targetType, parameter, culture);
			ConvertingBack?.Invoke(this, args);
			return PostConverter.IsNot()
				? args.ConvertedValue
				: PostConverter.ConvertBack(args.ConvertedValue, targetType, PostConverterParameter, culture);
		}
	}
}