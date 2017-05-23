using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Art.Replication;

namespace Art.Serialization.Serializers
{
    public static partial class Serializer
    {
        public static StringBuilder AppendRecursive(this StringBuilder builder, object value, KeepProfile keepProfile,
            int indentLevel = 1)
        {
            var serializer = keepProfile.Serializers.FirstOrDefault(c => c.CanApply(value, keepProfile)) ??
                             throw new Exception("Can not serialize " + value);
            
            return serializer.FillBuilder(builder, value, keepProfile, indentLevel);
        }
    }
}
