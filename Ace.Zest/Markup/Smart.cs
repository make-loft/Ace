using System;
using System.Globalization;
using System.Windows;
using IValueConverter = System.Windows.Data.IValueConverter;

#if XAMARIN
using Xamarin.Forms;
using TypeTypeConverter = Xamarin.Forms.TypeTypeConverter;
#else
using System.ComponentModel;
using System.Windows.Data;
#endif

namespace Ace.Markup
{
	public class Smart : Patterns.ABindingExtension
	{
		private object _defaultValue;

		public Smart() : base(null)
		{
			Mode = BindingMode.TwoWay;
			Path = new PropertyPath(nameof(SmartObject.Smart));
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
			set => FallbackValue = FallbackValue.Is(DependencyProperty.UnsetValue)
				? _defaultValue = value
				: DependencyProperty.UnsetValue;
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
			if (SmartObject.IsNot()) return null;
			value = Converter.IsNot() ? value : Converter.ConvertBack(value, targetType, parameter, culture);
			if (Segregate && SmartObject[Key].Is(out Segregator s)) s.Value = value;
			else SmartObject[Key] = value;
			return Segregate ? null : SmartObject;
		}

		private void Initialize(string set = "")
		{
			set = set.Replace("[", "").Replace("]", "");
			var parts = set.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length > 0) Key = parts[0].Trim();
			if (parts.Length > 1) DefaultValue = parts[1].Trim();
			if (parts.Length > 2)
				Segregate = parts[2].Trim().ToLower() == "true" || parts[2].Trim().ToLower() == "segregate";
		}
	}
}