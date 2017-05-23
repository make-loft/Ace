using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Art.Replication;
using Art.Serialization.Serializers;

namespace Art.Serialization
{
    public abstract class Serializer
    {
        public CultureInfo ActiveCulture = CultureInfo.InvariantCulture;

        public virtual bool CanApply(object value, KeepProfile keepProfile) => true;

        public abstract StringBuilder FillBuilder(StringBuilder builder, object value, KeepProfile keepProfile, int indentLevel = 1);

        public virtual string Convert(object value) => value?.ToString();
    }
    
    public abstract class Serializer<T> : Serializer
    {
        public override bool CanApply(object value, KeepProfile keepProfile) => value is T;

        public abstract string ConvertStrong(T value);

        public override string Convert(object value) => ConvertStrong((T)value);

        public override StringBuilder FillBuilder(StringBuilder builder, object value, KeepProfile keepProfile,
            int indentLevel = 1) =>
            value is T
                ? builder.Append(keepProfile.GetHead(value)).Append(ConvertStrong((T) value))
                    .Append(keepProfile.GetTail(value))
                : builder.Append(keepProfile.GetHead(value)).Append(Convert(value))
                    .Append(keepProfile.GetTail(value));
    }
}