using System.Collections.Generic;
using Ace.Replication.Models;

namespace Ace.Serialization.Serializators
{
	public class SetDeepSerializator : ASerializator<Set, object>
	{	 
		public override IEnumerable<string> GetSegmentBeads(object item, KeepProfile profile, int indentLevel) =>
			profile.ToStringBeads(item, indentLevel + 1);
		
		public override object Capture(Set set, KeepProfile profile, string data, ref int offset)
		{
			while (!profile.SetBody.TryFindTail(data, ref offset)) /* "]" */
			{
				profile.SkipHeadIndent(data, ref offset);

				set.Add(profile.ReadItem(data, ref offset));

				profile.SkipTailIndent(data, ref offset);
			}

			return set;
		}
	}
}