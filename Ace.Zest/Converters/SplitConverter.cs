using System;
using System.Globalization;
using System.Windows.Data;

namespace Ace.Converters
{
	public class SplitConverter : IValueConverter
	{
		public StringSplitOptions SplitOptions { get; set; }
		public int Count { get; set; } = int.MaxValue;
		public string Separators
		{
			get => new string(_separators);
			set => value.ToCharArray().To(out _separators);
		}

		private char[] _separators = { (char)0x0A, (char)0x0D, };

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			value.As("").Split(_separators, Count, SplitOptions);

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}