using System;
using System.ComponentModel;
using System.Globalization;
using Ace.Markup.Patterns;

namespace Ace.Markup
{
	public class Enum : ABindingExtension
	{
		public Enum(Type type) => Type = type;
		
		[TypeConverter(typeof(TypeTypeConverter))]
		public Type Type { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			System.Enum.GetValues(Type);
	}
}