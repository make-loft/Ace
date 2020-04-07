using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Ace.Converters.Patterns;
#if XAMARIN
using ContentProperty = Xamarin.Forms.ContentPropertyAttribute;
#else
using System.Windows.Markup;
#endif

namespace Ace.Converters
{
	[ContentProperty("Cases")]
	public class SwitchConverter : AValueConverter
	{
		public string DiagnosticKey { get; set; }

		public List<ICase<object, object>> Cases { get; } = new List<ICase<object, object>>();

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var comparison = StringComparison;
			var matchedCase = Cases.FirstOrDefault(c => c.MatchByKey(value, comparison));
			var matchedValue = matchedCase.Is()	? matchedCase.Value : ByDefault;
			var convertedValue = matchedValue.Is(UndefinedValue) ? value : matchedValue;

			if (DiagnosticKey.Is())
			{
				var diagnosticMessage = matchedCase.Is()
					? $"{DiagnosticKey}: '{matchedValue}' matched by key '{matchedCase.Key}' for '{value}' and converted to '{convertedValue}'"
					: $"{DiagnosticKey}: The default value '{matchedValue}' matched for '{value}' and converted to '{convertedValue}'";

				Trace.WriteLine(diagnosticMessage);
			}

			return convertedValue;
		}
	}
}