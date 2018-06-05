using System.Collections.Generic;
using Ace.Replication.Models;

namespace Ace.Serialization
{
	public static class Serializer
	{
		public static object ReadItem(this string matrix, KeepProfile keepProfile, ref int offset) =>
			keepProfile.CreateSingleModel(matrix, ref offset).Capture(keepProfile, matrix, ref offset);

		public static IEnumerable<string> ToStringBeads(this object value,
			KeepProfile keepProfile, int indentLevel = 1) => (value is IModel model
			? model
			: keepProfile.SimplexConverter.Convert(value, keepProfile)).ToStringBeads(keepProfile, indentLevel);
	}
}