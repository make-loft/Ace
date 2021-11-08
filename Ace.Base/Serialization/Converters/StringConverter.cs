namespace Ace.Serialization.Converters
{
	public class StringConverter : Converter
	{
		public override string Encode(object value) => value as string;
		public override object Decode(string value, System.Type type) => type is null || type.Is(TypeOf.String.Raw) ? value : Undefined;
	}
}