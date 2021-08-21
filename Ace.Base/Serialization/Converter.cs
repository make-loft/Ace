using System.Globalization;

namespace Ace.Serialization
{
	public class Converter
	{
		public static readonly object Undefined = new();
		public CultureInfo ActiveCulture = CultureInfo.InvariantCulture;

		public virtual string Encode(object value) => value?.ToString();
		public virtual object Decode(string value, string typeKey) => Undefined;
	}
}