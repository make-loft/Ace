using System;
using System.Windows;

namespace Ace.Converters.Patterns
{
	public class ValueConverter : AValueConverter.Reflected
	{
		protected readonly DependencyObject Attached = new();
		public static readonly object UndefinedValue = new();

		public static DependencyProperty Attach(string name, object defaultValue = default) =>
			DependencyProperty.RegisterAttached(name, typeof(object), typeof(ValueConverter),
				new PropertyMetadata(defaultValue ?? UndefinedValue));

		public static readonly DependencyProperty ByDefaultProperty = Attach(nameof(ByDefault));

		public object ByDefault
		{
			get => Attached.GetValue(ByDefaultProperty);
			set => Attached.SetValue(ByDefaultProperty, value);
		}

		public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

		public override object Convert(object value) =>
			ByDefault.To(out var defaultValue).Is(UndefinedValue) ? value : defaultValue;
	}
}