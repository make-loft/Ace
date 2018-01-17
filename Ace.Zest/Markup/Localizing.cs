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

		public Letter Case { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			LocalizationSource.Wrap[Key, culture].To(Case);
	}
}