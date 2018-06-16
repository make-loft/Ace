using System.Collections.Generic;
using System.Linq;
using Ace.Serialization.Serializators;

namespace Ace.Serialization
{
	public static class Serializer
	{
		public static readonly List<ASerializator> Serializators = New.List<ASerializator>
		(
			new MapDeepSerializator(),
			new SetDeepSerializator(),
			new SimplexSerializator()
		);
			
		public static object ReadItem(this string data, KeepProfile keepProfile, ref int offset)
		{
			var model = keepProfile.CreateBlankModel(data, ref offset);
			return Serializators.FirstOrDefault(s => s.CanApply(model))?.Capture(model, keepProfile, data, ref offset);
		}

		public static IEnumerable<string> ToStringBeads(this object value, KeepProfile keepProfile, int indentLevel) =>
			Serializators.FirstOrDefault(s => s.CanApply(value))?.ToStringBeads(value, keepProfile, indentLevel);
	}
}