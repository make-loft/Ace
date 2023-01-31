using System;
using System.Globalization;
using System.Windows;
using System.Linq;

#if XAMARIN
using Xamarin.Forms;
#else
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Markup;
#endif

namespace Ace.Markup
{
	[ContentProperty(nameof(Set))]
	public class Smart : Patterns.ABindingExtension
	{
		private object _defaultValue;

		public Smart() : base(default)
		{
			Mode = BindingMode.TwoWay;
			Path = new(nameof(SmartObject.Smart));
		}

		public Smart(string set) : this() => Set = set;

		public Smart(string key, string defaultValue) : this()
		{
			Key = key;
			DefaultValue = defaultValue;
		}

		public string Set
		{
			set => Initialize(value);
		}

		public string Key { get; set; }

		public object DefaultValue
		{
			get => _defaultValue;
			set => value.To(out _defaultValue).With
			(
				FallbackValue = FallbackValue.Is(DependencyProperty.UnsetValue)
					? value
					: DependencyProperty.UnsetValue
			);
		}

		public bool Segregate { get; set; }
		public new IValueConverter Converter { get; set; }
		public SmartObject SmartObject { get; private set; }

		[TypeConverter(typeof(TypeTypeConverter))]
		public Type StoreKey
		{
			get => Source?.GetType();
			set => Source = Ace.Store.Get(value);
		}

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SmartObject = (Source ?? value) as SmartObject;
			value = SmartObject?[Key, DefaultValue, Segregate];
			if (Segregate) value = (value as Segregator)?.Value;
			if (value.IsNot() && targetType.IsValueType) value = DefaultValue;
			return Converter.IsNot() ? value : Converter.Convert(value, targetType, parameter, culture);
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (SmartObject.IsNot()) return default;
			value = Converter.IsNot() ? value : Converter.ConvertBack(value, targetType, parameter, culture);
			if (Segregate && SmartObject[Key].Is(out Segregator s)) s.Value = value;
			else SmartObject[Key] = value;
			return Segregate ? default : SmartObject;
		}

		private void Initialize(string set = "")
		{
			set = set.Replace("[", "").Replace("]", "");
			var parts = set.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length > 0) Key = parts[0].Trim();
			if (parts.Length > 1) DefaultValue = parts[1].Trim();
			if (parts.Length > 2)
				Segregate = SegregationLiterals.Contains(parts[2].Trim().ToLower());
		}

		private static readonly string[] SegregationLiterals = { "true", "segregate" };
	}
}