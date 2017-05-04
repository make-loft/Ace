using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Art.Serialization.Converters
{
    public class StringConverter : IConverter<object, string>, IConverter<string, object>
    {
        public CultureInfo ActiveCulture = CultureInfo.InvariantCulture;

        public string NullLiteral = "null";
        public string TrueLiteral = "true";
        public string FalseLiteral = "false";
        public string RealNumbersFormat = "G";
        public string IntegerNumbersFormat = "G";
        public string DateTimeOffsetFormat = "O";
        public string DateTimeFormat = "O";
        public string TimeSpanFormat = "G";
        public string GuidFormat = "D";

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

        public Dictionary<string, Type> SyffixToType = new Dictionary<string, Type>
        {
            {"B", typeof(byte)},
            {"C", typeof(char)},
            {"I", typeof(int)},
            {"U", typeof(uint)},
            {"UI", typeof(uint)},
            {"IU", typeof(uint)},
            {"L", typeof(long)},
            {"UL", typeof(ulong)},
            {"LU", typeof(ulong)},
            {"F", typeof(float)},
            {"D", typeof(double)},
            {"M", typeof(decimal)}
        };

        public string Convert(object value, params object[] args) =>
            value == null
                ? NullLiteral
                : value is bool booleanValue
                    ? booleanValue
                        ? TrueLiteral
                        : FalseLiteral
                    : ConvertPrimitive(value);

        public object Convert(string value, params object[] args) =>
            value == NullLiteral
                ? null
                : value == TrueLiteral
                    ? true
                    : value == FalseLiteral
                        ? false
                        : ConvertPrimitive(value);

        public string ConvertPrimitive(object value, params object[] args)
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

        public string ConvertComplex(object value, params object[] args)
        {
            switch (value)
            {
                case Enum e:
                    return e.ToString();
                case Type t:
                    return t.AssemblyQualifiedName;
                case Uri u:
                    return u.ToString();
                case Guid d:
                    return d.ToString(GuidFormat);
                case DateTime d:
                    return d.ToString(DateTimeFormat, ActiveCulture);
                case DateTimeOffset d:
                    return d.ToString(DateTimeOffsetFormat, ActiveCulture);
                case TimeSpan d:
                    return d.ToString(TimeSpanFormat, ActiveCulture);
                default:
                    return null;
            }
        }

        public object ConvertPrimitive(string value, params object[] args)
        {
            if (value.Length > 0 && char.IsDigit(value[value.Length - 1]))
            {
                if (int.TryParse(value, NumberStyles.Any, ActiveCulture, out var i)) return i;
                if (double.TryParse(value, NumberStyles.Any, ActiveCulture, out var r)) return r;
            }

            var number = value.ToUpper();
            if (value.EndsWith("B") && byte.TryParse(number.Substring(0, number.Length - 1), out var b)) return b;
            if (value.EndsWith("C") && byte.TryParse(number.Substring(0, number.Length - 1), out var c)) return c;
            if ((value.EndsWith("UL") || value.EndsWith("LU")) &&
                ulong.TryParse(number.Substring(0, number.Length - 2), out var ul)) return ul;
            if (value.EndsWith("U") && uint.TryParse(number.Substring(0, number.Length - 1), out var u)) return u;
            if (value.EndsWith("L") && long.TryParse(number.Substring(0, number.Length - 1), out var l)) return l;
            if (value.EndsWith("D") && double.TryParse(number.Substring(0, number.Length - 1), out var d)) return d;
            if (value.EndsWith("F") && float.TryParse(number.Substring(0, number.Length - 1), out var f)) return f;
            if (value.EndsWith("M") && decimal.TryParse(number.Substring(0, number.Length - 1), out var m)) return m;
            return null;
        }

        public virtual object ConvertComplex(string value, params object[] args)
        {
            if (value == null) return null;
            var typeName = (string) args[0];
            switch (typeName)
            {
                case "Uri":
                    return new Uri(value);
                case "DateTime":
                    return value.EndsWith("Z")
                        ? DateTime.Parse(value, ActiveCulture, DateTimeStyles.AdjustToUniversal)
                        : DateTime.Parse(value, ActiveCulture);
                default:
                    var o = typeof(RegexOptions);
                    var type = Type.GetType(typeName) ?? Type.GetType("System." + typeName);
                    if (type != null && type.IsEnum) return Enum.Parse(type, value, true);
                    var parseMethod = type?.GetMethod("Parse", new[] {typeof(string)});
                    return parseMethod?.Invoke(null, new object[] {value});
            }
        }
    }
}
