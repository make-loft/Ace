using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ace.Markup
{
	public delegate ValidationResult Validation(Patterns.ValidationArgs args);

	public class CustomRule : ValidationRule
	{
		public event Validation Validation;

		public override ValidationResult Validate(object value, CultureInfo culture) =>
			Validation?.Invoke(new(value, culture));

		public override ValidationResult Validate(object value, CultureInfo culture, BindingExpressionBase owner) =>
			Validation?.Invoke(new(value, culture, bindingExpression: owner));

		public override ValidationResult Validate(object value, CultureInfo culture, BindingGroup owner) =>
			Validation?.Invoke(new(value, culture, bindingGroup: owner));
	}
}
