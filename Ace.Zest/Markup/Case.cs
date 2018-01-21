// ReSharper disable RedundantUsingDirective
using System;
using System.Windows;
using System.Windows.Markup;
using Ace.Converters.Patterns;
using ContentProperty = Xamarin.Forms.ContentPropertyAttribute;

namespace Ace.Markup
{
	[ContentProperty("Value")]
	public class Case : DependencyObject, ICase<object, object>
	{
		public static readonly object UndefinedValue = new object();
		
		public static readonly DependencyProperty KeyProperty =
			DependencyProperty.Register("Key", typeof(object), typeof(Case), new PropertyMetadata(UndefinedValue));

		public object Key
		{
			get => GetValue(KeyProperty);
			set => SetValue(KeyProperty, value);
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(object), typeof(Case), new PropertyMetadata(UndefinedValue));

		public object Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public virtual bool MatchByKey(object key) =>
			key == Key || Key == UndefinedValue ||
			string.Compare(key?.ToString(), Key?.ToString(), StringComparison.OrdinalIgnoreCase) == 0;

		public virtual bool MatchByValue(object value) => value == Value || Equals(value, UndefinedValue);
	}

	[ContentProperty("Value")]
	public class TypedCase : Case
	{
		public new Type Key
		{
			get => (Type) GetValue(KeyProperty);
			set => SetValue(KeyProperty, value);
		}

		public override bool MatchByKey(object key) => base.Key == UndefinedValue || key?.GetType() == Key;
	}
}