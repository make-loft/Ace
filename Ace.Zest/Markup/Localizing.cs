using System;
using System.Globalization;
using System.Windows;
using Ace.Converters;

namespace Ace.Markup
{
	public class Localizing : Patterns.ABindingExtension
	{
		public Localizing()
		{
			Source = LocalizationSource.Wrap;
			Path = new PropertyPath(LocalizationSource.Wrap.ActivePath);
		}

		public Localizing(string key) : this() => Key = key;

		public string Key { get; set; }

		public Modifier Modifiers { get; set; }
		
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
#if NETSTANDARD
			LocalizationSource.Wrap[Key].Apply(Modifiers);
#else
			LocalizationSource.Wrap[Key].Apply(Modifiers).Apply(StringFormat);
#endif
	}
}