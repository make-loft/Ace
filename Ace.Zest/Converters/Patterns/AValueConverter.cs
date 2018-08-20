using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ace.Converters.Patterns
{
	public abstract class AValueConverter : DependencyObject, ITwoWayConverter
	{
		public static readonly object UndefinedValue = new object();

		public static readonly DependencyProperty ByDefaultProperty = DependencyProperty.Register(
			"ByDefault", typeof(object), typeof(AValueConverter), new PropertyMetadata(UndefinedValue));

		public static DependencyProperty Register<TProperty, TOwner>(string name, object defaultValue = null) =>
			DependencyProperty.Register(name, typeof(TProperty), typeof(TOwner),
				new PropertyMetadata(defaultValue ?? UndefinedValue));

		public static DependencyProperty Register(string name, object defaultValue = null) =>
			DependencyProperty.Register(name, typeof(object), typeof(AValueConverter),
				new PropertyMetadata(defaultValue ?? UndefinedValue));

		public object ByDefault
		{
			get => GetValue(ByDefaultProperty);
			set => SetValue(ByDefaultProperty, value);
		}

		protected static object GetDefined(object activeValue, object defaultValue) =>
			activeValue.Is(UndefinedValue) ? defaultValue : activeValue;

		public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			BackConverter.IsNot()
				? throw new Exception("Specify 'BackConverter' for ConvertBack")
				: BackConverter.Convert(value, targetType, parameter, culture);

		public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

		public IValueConverter BackConverter { get; set; }

		public static bool EqualsAsStrings(object a, object b, StringComparison comparison) =>
			Equals(a, b)
			|| string.Compare(a as string, b?.ToString(), comparison).Is(0)
			|| string.Compare(a?.ToString(), b as string, comparison).Is(0);
	}
}