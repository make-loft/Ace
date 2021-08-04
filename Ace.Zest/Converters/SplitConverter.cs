using Ace.Converters.Patterns;

using System;

namespace Ace.Converters
{
	public class SplitConverter : AValueConverter.Reflected
	{
		public StringSplitOptions SplitOptions { get; set; }
		public int Count { get; set; } = int.MaxValue;
		public string Separators
		{
			get => new(_separators);
			set => value.ToCharArray().To(out _separators);
		}

		private char[] _separators = { (char)0x0A, (char)0x0D, };

		public override object Convert(object value) =>	value.As("").Split(_separators, Count, SplitOptions);
	}
}