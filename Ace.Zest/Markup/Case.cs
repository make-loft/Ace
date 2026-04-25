using System;

using Ace.Markup.Patterns;
using System.Windows;
using Ace.Controls;
#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Markup;
#endif

namespace Ace.Markup;

[ContentProperty(nameof(Value))]
public class Case : DependencyObject, ICase<object, object>
{
	public static readonly object UndefinedValue = ValueConverter.UndefinedValue;

	public static readonly DependencyProperty KeyProperty
		= Type<Case>.Create(c => c.Key, UndefinedValue);

	public static readonly DependencyProperty ValueProperty
		= Type<Case>.Create(c => c.Value, UndefinedValue);

	public object Key
	{
		get => GetValue(KeyProperty);
		set => SetValue(KeyProperty, value);
	}

	public object Value
	{
		get => GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}

	public virtual bool MatchByKey(object key, StringComparison comparison)
		=> Key.Is(key) || Key.Is(UndefinedValue) || Key.Is(key, comparison);
}

[ContentProperty(nameof(Value))]
public class TypedCase : Case
{
	public new Type Key
	{
		get => (Type)GetValue(KeyProperty);
		set => SetValue(KeyProperty, value);
	}

	public override bool MatchByKey(object key, StringComparison comparison)
		=> base.Key.Is(UndefinedValue) || Key.Is(key?.GetType());
}
