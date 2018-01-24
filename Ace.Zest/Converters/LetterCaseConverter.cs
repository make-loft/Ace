using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Ace.Converters
{
	[Flags]
	public enum Modifier
	{
		Original = 0,
		ToLower = 1,
		ToUpper = 2,
		RemoveUnderlines = 4
	}

	public static class LetterSugar
	{
		private static readonly Modifier[] Modifiers =
			Enum.GetValues(typeof(Modifier)).Cast<Modifier>().Skip(1).ToArray();

		public static string Modify(this string text, Modifier modifiers) =>
			modifiers == Modifier.Original
				? text
				: Modifiers.Where(m => (modifiers & m) == m).Aggregate(text, (t, m) => t.Apply(m));

		private static string Apply(this string text, Modifier modifier) =>
			modifier == Modifier.Original ? text :
			modifier == Modifier.ToUpper ? text.ToUpper() :
			modifier == Modifier.ToLower ? text.ToLower() :
			modifier == Modifier.RemoveUnderlines ? text.Replace("_", "") :
			text;
	}

	public class ModifierConverter : IValueConverter
	{
		public Modifier Modifiers { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			(value as string)?.Modify(Modifiers);

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			throw new NotImplementedException();
	}
}