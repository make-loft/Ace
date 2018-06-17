using System.Collections;
using System.Collections.Generic;

namespace Ace.Serialization
{
    public abstract class ASerializator
    {
        public virtual bool CanApply(object o) => true;

        public abstract object Capture(object value, KeepProfile profile, string data, ref int offset);

        public abstract IEnumerable<string> ToStringBeads(object value, KeepProfile profile, int indentLevel);
    }

    public abstract class ASerializator<T, TItem> : ASerializator
    {
        public override bool CanApply(object o) => o is T;

        public abstract object Capture(T map, KeepProfile profile, string data, ref int offset);

        public override object Capture(object value, KeepProfile profile, string data, ref int offset) =>
            Capture((T) value, profile, data, ref offset);

        public abstract IEnumerable<string> GetSegmentBeads(TItem pair, KeepProfile profile, int indentLevel);

        public override IEnumerable<string> ToStringBeads(object value, KeepProfile profile, int indentLevel) =>
            ToStringBeads((ICollection<TItem>)value, profile, indentLevel);
        
        public IEnumerable<string> ToStringBeads(ICollection<TItem> items, KeepProfile profile, int indentLevel = 1)
        {
            if (profile.AppendCountComments) yield return $"/*{((ICollection)items).Count}*/ ";
            yield return profile.GetHead(items); /* "{" */
            foreach (var bead in ConvertComplex(items, profile, indentLevel))
                yield return bead;
            yield return profile.GetTail(items); /* "}" */
        }
        
        public IEnumerable<string> ConvertComplex(ICollection<TItem> items, KeepProfile profile, int indentLevel = 1)
        {
            var counter = 0;

            foreach (var item in items)
            {
                yield return profile.GetHeadIndent(indentLevel, items, counter);

                var beads = GetSegmentBeads(item, profile, indentLevel);
                foreach (var bead in beads)
                {
                    yield return bead;
                }

                yield return profile.GetTailIndent(indentLevel, items, counter++);
            }
        }
    }
}