using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;
using Ace.Converters.Patterns;
#if XAMARIN
using ContentProperty = Xamarin.Forms.ContentPropertyAttribute;
#endif

namespace Ace.Converters
{
	[ContentProperty("Cases")]
	public class SwitchConverter : AValueConverter
	{
		public bool Diagnostics { get; set; }

		public List<ICase<object, object>> Cases { get; } = new List<ICase<object, object>>();

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var comparison = StringComparison;
			var matchedCase = Cases.FirstOrDefault(c => c.MatchByKey(value, comparison));
			var newValue = matchedCase.Is() ? matchedCase.Value : ByDefault;

			if (Diagnostics)
			{
				if (matchedCase.Is())
					Trace.WriteLine($"Key is {matchedCase.Key}: Value is {newValue}");
				else Trace.WriteLine($"The key for {value} is not defined");
			}

			return newValue;
		}
	}
}