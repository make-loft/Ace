using System.Globalization;
using System.Windows.Data;

namespace Ace.Markup.Patterns;

public readonly struct ValidationArgs(object value, CultureInfo culture,
	BindingExpressionBase bindingExpression = default, BindingGroup bindingGroup = default)
{
	public object Value { get; } = value;
	public CultureInfo Culture { get; } = culture;
	public BindingExpressionBase BindingExpression { get; } = bindingExpression;
	public BindingGroup BindingGroup { get; } = bindingGroup;
}
