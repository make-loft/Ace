using System.Globalization;

namespace Ace.Serialization
{
	public class Converter
	{
		public static readonly object Undefined = new object();
		public CultureInfo ActiveCulture = CultureInfo.InvariantCulture;

		public virtual string Convert(object value) => value?.ToString();
		public virtual object Revert(string value, string typeCode) => Undefined;
	}
}