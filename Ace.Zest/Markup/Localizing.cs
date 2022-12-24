using System;
using System.Globalization;

using Ace.Converters;

#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Markup;
#endif

namespace Ace.Markup
{
	[ContentProperty(nameof(Key))]
	public class Localizing : Patterns.ABindingExtension
	{
		public static bool ForceStringFormatByDefault = false;

		public Localizing()
		{
			Source = LocalizationSource.Wrap;
			Path = new(LocalizationSource.Wrap.ActivePath);
		}

		public Localizing(string key) : this() => Key = key;

		public string Key { get; set; }

		public Modifier Modifiers { get; set; }

		public bool ForceStringFormat { get; set; } = ForceStringFormatByDefault;

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			LocalizationSource.Wrap[Key].Apply(Modifiers).Apply(ForceStringFormat ? StringFormat : default);
	}
}
