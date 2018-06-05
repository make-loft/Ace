using System.Collections;
using System.Collections.Generic;
using Ace.Serialization;

namespace Ace.Replication.Models
{
	public interface IModel : ICollection
	{
		object Capture(KeepProfile keepProfile, string data, ref int offset);

		IEnumerable<string> ToStringBeads(KeepProfile keepProfile, int indentLevel);
	}
	
	public interface IModel<T> : ICollection<T>, IModel
	{
		IEnumerable<string> GetSegmentBeads(T item, KeepProfile keepProfile, int indentLevel);
	}

	public static class ACollectionModel
	{
		public static IEnumerable<string> ConvertComplex<T>(this IModel<T> items, KeepProfile keepProfile, int indentLevel = 1)
		{
			var counter = 0;

			foreach (var item in items)
			{
				yield return keepProfile.GetHeadIndent(indentLevel, items, counter);

				var beads = items.GetSegmentBeads(item, keepProfile, indentLevel);
				foreach (var bead in beads)
				{
					yield return bead;
				}

				yield return keepProfile.GetTailIndent(indentLevel, items, counter++);
			}
		}
	
		public static IEnumerable<string> ToStringBeads<T>(this IModel<T> items, KeepProfile keepProfile, int indentLevel = 1)
		{
			if (keepProfile.AppendCountComments) yield return $"/*{((ICollection)items).Count}*/ ";
			yield return keepProfile.GetHead(items); /* "{" */
			foreach (var bead in ConvertComplex(items, keepProfile, indentLevel))
				yield return bead;
			yield return keepProfile.GetTail(items); /* "}" */
		}
	}
}