using System;
using System.Globalization;
using System.Windows;
using Ace.Converters;

namespace Ace.Markup
{
	public class Localizing : Patterns.ABindingExtension
	{
		public static bool ForceStringFormatByDefault = false;

		public Localizing()
		{
			Source = LocalizationSource.Wrap;
			Path = new PropertyPath(LocalizationSource.Wrap.ActivePath);
		}

		public Localizing(string key) : this() => Key = key;

		public string Key { get; set; }

		public Modifier Modifiers { get; set; }

		public bool ForceStringFormat { get; set; } = ForceStringFormatByDefault;

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			ForceStringFormat
				? LocalizationSource.Wrap[Key].Apply(Modifiers).Apply(StringFormat)
				: LocalizationSource.Wrap[Key].Apply(Modifiers);
	}
}
