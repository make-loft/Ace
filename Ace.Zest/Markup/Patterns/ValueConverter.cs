using System;
using System.Windows;

using Ace.Controls;

namespace Ace.Markup.Patterns;

public class ValueConverter : AValueConverter.Reflected
{
	public static readonly object UndefinedValue = new();

	public static readonly DependencyProperty ByDefaultProperty
		= Type<ValueConverter>.Create(c => c.ByDefault, UndefinedValue);

	public object ByDefault
	{
		get => GetValue(ByDefaultProperty);
		set => SetValue(ByDefaultProperty, value);
	}

	public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

	public override object Convert(object value)
		=> ByDefault.To(out var defaultValue).Is(UndefinedValue) ? value : defaultValue;
}
