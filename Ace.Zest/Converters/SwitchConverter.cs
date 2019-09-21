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
			var newValue = matchedCase.Is() ? matchedCase.Value : ByDefault;

			if (DiagnosticKey.Is())
			{
				var diagnosticMessage = matchedCase.Is()
					? $"{DiagnosticKey}: '{newValue}' matched by key '{matchedCase.Key}' for '{value}'"
					: $"{DiagnosticKey}: The default value '{newValue}' matched for '{value}'";

				Trace.WriteLine(diagnosticMessage);
			}

			return newValue;
		}
	}
}