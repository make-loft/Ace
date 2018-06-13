using System.Collections.Generic;
using System.Linq;
using Ace.Replication.Models;
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
			var model = keepProfile.CreateSingleModel(data, ref offset);
			var serializator = Serializators.FirstOrDefault(s => s.CanApply(model));
			serializator?.Capture(model, keepProfile, data, ref offset);
			return model is Simplex simplex ? simplex.Revert(keepProfile.SimplexConverter) : model;
		}

		public static IEnumerable<string> ToStringBeads(this object value, KeepProfile keepProfile, int indentLevel)
		{
			var serializator = Serializators.FirstOrDefault(s => s.CanApply(value));
			return serializator?.ToStringBeads(value, keepProfile, indentLevel);
		}
	}
}