using System.Collections;
using System.Collections.Generic;

namespace Ace.Serialization
{
    public abstract class ASerializator
    {
        public virtual bool CanApply(object o) => true;

        public abstract object Capture(object value, KeepProfile keepProfile, string data, ref int offset);

        public abstract IEnumerable<string> ToStringBeads(object value, KeepProfile keepProfile, int indentLevel);
    }

    public abstract class ASerializator<T, TItem> : ASerializator
    {
        public override bool CanApply(object o) => o is T;

        public abstract object Capture(T map, KeepProfile keepProfile, string data, ref int offset);

        public override object Capture(object value, KeepProfile keepProfile, string data, ref int offset) =>
            Capture((T) value, keepProfile, data, ref offset);

        public abstract IEnumerable<string> GetSegmentBeads(TItem pair, KeepProfile keepProfile, int indentLevel);

        public override IEnumerable<string> ToStringBeads(object value, KeepProfile keepProfile, int indentLevel) =>
            ToStringBeads((ICollection<TItem>)value, keepProfile, indentLevel);
        
        public IEnumerable<string> ToStringBeads(ICollection<TItem> items, KeepProfile keepProfile, int indentLevel = 1)
        {
            if (keepProfile.AppendCountComments) yield return $"/*{((ICollection)items).Count}*/ ";
            yield return keepProfile.GetHead(items); /* "{" */
            foreach (var bead in ConvertComplex(items, keepProfile, indentLevel))
                yield return bead;
            yield return keepProfile.GetTail(items); /* "}" */
        }
        
        public IEnumerable<string> ConvertComplex(ICollection<TItem> items, KeepProfile keepProfile, int indentLevel = 1)
        {
            var counter = 0;

            foreach (var item in items)
            {
                yield return keepProfile.GetHeadIndent(indentLevel, items, counter);

                var beads = GetSegmentBeads(item, keepProfile, indentLevel);
                foreach (var bead in beads)
                {
                    yield return bead;
                }

                yield return keepProfile.GetTailIndent(indentLevel, items, counter++);
            }
        }
    }
}