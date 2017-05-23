using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Art.Serialization.Serializers
{
    public class NullSerializer : Serializer<object>
    {
        public string NullLiteral = "null";

        public override bool CanApply(object value, KeepProfile keepProfile) => value == null;

        public override string ConvertStrong(object value) => NullLiteral;
    }

    public class BooleanSerializer : Serializer<bool>
    {
        public string TrueLiteral = "true";
        public string FalseLiteral = "false";

        public override string ConvertStrong(bool value) => value ? TrueLiteral : FalseLiteral;
    }
    
    public class StringSerializer : Serializer<string>
    {
        public override string ConvertStrong(string value) => value;
    }

    public class NumberSerializer : Serialization.Serializer
    {
        public string IntegerNumbersFormat = null; //"G"; /* null for best performance */
        public string RealNumbersFormat = "G";
        public bool AppendSyffixes = false;
        
        public Dictionary<Type, string> TypeToSyffix = new Dictionary<Type, string>
        {
            {typeof(byte), "B"},
            {typeof(char), "C"},
            //{ typeof(int), "I"},
            {typeof(uint), "U"},
            {typeof(long), "L"},
            {typeof(ulong), "UL"},
            {typeof(float), "F"},
            //{ typeof(double), "D"},
            {typeof(decimal), "M"}
        };

        public override bool CanApply(object value, KeepProfile keepProfile) => value.GetType().IsPrimitive;

        public override StringBuilder FillBuilder(StringBuilder builder, object value, KeepProfile keepProfile,
            int indentLevel = 1)
        {
            return AppendSyffixes
                ? builder.Append(Convert(value)).Append(TypeToSyffix[value.GetType()])
                : builder.Append(Convert(value));
        }

        public override string Convert(object value)
        {
            switch (value)
            {
                case sbyte b:
                    return b.ToString(IntegerNumbersFormat, ActiveCulture);
                case byte b:
                    return b.ToString(IntegerNumbersFormat, ActiveCulture);
                case char c:
                    return ((int) c).ToString(IntegerNumbersFormat, ActiveCulture);
                case short i:
                    return i.ToString(IntegerNumbersFormat, ActiveCulture);
                case ushort i:
                    return i.ToString(IntegerNumbersFormat, ActiveCulture);
                case int i:
                    return i.ToString(IntegerNumbersFormat, ActiveCulture);
                case uint i:
                    return i.ToString(IntegerNumbersFormat, ActiveCulture);
                case long i:
                    return i.ToString(IntegerNumbersFormat, ActiveCulture);
                case ulong i:
                    return i.ToString(IntegerNumbersFormat, ActiveCulture);
                case float n:
                    return n.ToString(RealNumbersFormat, ActiveCulture);
                case double n:
                    return n.ToString(RealNumbersFormat, ActiveCulture);
                case decimal m:
                    return m.ToString(RealNumbersFormat, ActiveCulture);
                default:
                    return null;
            }
        }
    }
}