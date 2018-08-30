using System;
using System.Windows;
using System.Windows.Markup;
using Ace.Converters.Patterns;
using static System.Windows.DependencyProperty;
using static Ace.Converters.Patterns.AValueConverter;
#if XAMARIN
using Xamarin.Forms;
#endif

namespace Ace.Markup
{
	[ContentProperty("Value")]
	public class Case : DependencyObject, ICase<object, object>
	{
		public static readonly DependencyProperty KeyProperty =
			Register("Key", typeof(object), typeof(Case), new PropertyMetadata(UndefinedValue));

		public object Key
		{
			get => GetValue(KeyProperty);
			set => SetValue(KeyProperty, value);
		}

		public static readonly DependencyProperty ValueProperty =
			Register("Value", typeof(object), typeof(Case), new PropertyMetadata(UndefinedValue));

		public object Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public virtual bool MatchByKey(object key, StringComparison comparison) =>
			key == Key || UndefinedValue == Key || EqualsAsStrings(key, Key, comparison);
	}

	[ContentProperty("Value")]
	public class TypedCase : Case
	{
		public new Type Key
		{
			get => (Type) GetValue(KeyProperty);
			set => SetValue(KeyProperty, value);
		}

		public override bool MatchByKey(object key, StringComparison comparison) =>
			base.Key == UndefinedValue || key?.GetType() == Key;
	}
}