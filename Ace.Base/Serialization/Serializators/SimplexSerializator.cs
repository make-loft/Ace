using System.Collections.Generic;
using Ace.Replication.Models;

namespace Ace.Serialization.Serializators
{
    public class SimplexSerializator : ASerializator
    {
        public override object Capture(object value, KeepProfile keepProfile, string data, ref int offset) =>
            keepProfile.CaptureSimplex((Simplex) value, data, ref offset);

        public override IEnumerable<string> ToStringBeads(object value, KeepProfile keepProfile, int indentLevel) => 
            keepProfile.SimplexConverter.Convert(value, keepProfile);
    }
}