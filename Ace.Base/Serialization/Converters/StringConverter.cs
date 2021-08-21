namespace Ace.Serialization.Converters
{
	public class StringConverter : Converter
	{
		public override string Encode(object value) => value as string;
		public override object Decode(string value, string typeKey) => typeKey is null ? value : Undefined;
	}
}