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

		public static string Apply(this string text, Modifier modifiers) =>
			modifiers.Is(Modifier.Original)
				? text
				: Modifiers.Where(m => (modifiers & m).Is(m)).Aggregate(text, (t, m) => t.ApplySingle(m));

		private static string ApplySingle(this string text, Modifier modifier) =>
			modifier.Is(Modifier.Original) ? text :
			modifier.Is(Modifier.ToUpper) ? text.ToUpper() :
			modifier.Is(Modifier.ToLower) ? text.ToLower() :
			modifier.Is(Modifier.RemoveUnderlines) ? text.Replace("_", "") :
			text;

		public static string Apply(this string text, string stringFormat) =>
			stringFormat.Is() ? string.Format(stringFormat, text) : text;
	}

	public class ModifierConverter : IValueConverter
	{
		public Modifier Modifiers { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			value.As<string>()?.Apply(Modifiers);

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			throw new NotImplementedException();
	}
}