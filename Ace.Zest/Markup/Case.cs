using System;
using System.Windows;
using Ace.Converters.Patterns;
using static Ace.Converters.Patterns.ValueConverter;
#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Markup;
#endif

namespace Ace.Markup
{
	[ContentProperty(nameof(Value))]
	public class Case : ICase<object, object>
	{
		protected readonly DependencyObject Attached = new();

		public static readonly DependencyProperty KeyProperty = Attach(nameof(Key));
		public static readonly DependencyProperty ValueProperty = Attach(nameof(Value));

		public object Key
		{
			get => Attached.GetValue(KeyProperty);
			set => Attached.SetValue(KeyProperty, value);
		}

		public object Value
		{
			get => Attached.GetValue(ValueProperty);
			set => Attached.SetValue(ValueProperty, value);
		}

		public virtual bool MatchByKey(object key, StringComparison comparison) =>
			Key.Is(key) || Key.Is(UndefinedValue) || Key.Is(key, comparison);
	}

	[ContentProperty(nameof(Value))]
	public class TypedCase : Case
	{
		public new Type Key
		{
			get => (Type)Attached.GetValue(KeyProperty);
			set => Attached.SetValue(KeyProperty, value);
		}

		public override bool MatchByKey(object key, StringComparison comparison) =>
			base.Key.Is(UndefinedValue) || Key.Is(key?.GetType());
	}
}