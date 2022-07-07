using System;
using System.Windows;

namespace Ace.Markup.Patterns
{
	public class ValueConverter : AValueConverter.Reflected
	{
		public static readonly object UndefinedValue = new();

		public static DependencyProperty For<T>(string name, object defaultValue = default) =>
			DependencyProperty.Register(name, typeof(object), typeof(T),
				new PropertyMetadata(defaultValue ?? UndefinedValue));

		public static readonly DependencyProperty ByDefaultProperty = For<ValueConverter>(nameof(ByDefault));

		public object ByDefault
		{
			get => GetValue(ByDefaultProperty);
			set => SetValue(ByDefaultProperty, value);
		}

		public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

		public override object Convert(object value) =>
			ByDefault.To(out var defaultValue).Is(UndefinedValue) ? value : defaultValue;
	}
}