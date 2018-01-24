using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;
using Ace.Converters.Patterns;
using ContentProperty = Xamarin.Forms.ContentPropertyAttribute;

namespace Ace.Converters
{
	[ContentProperty("Cases")]
	public class SwitchConverter : AValueConverter
	{
		public List<ICase<object, object>> Cases { get; } = new List<ICase<object, object>>();

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var @case = Cases.FirstOrDefault(c => c.MatchByKey(value, StringComparison));
			return GetDefined(@case == null ? ByDefault : @case.Value, value);
		}
	}
}