using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Art.Serialization.Escapers;

namespace Art.Serialization.Converters
{
    public class Simplex : List<string>
    {
        public override string ToString() => this.Aggregate("", (a, b) => a + b);
        

        public static Dictionary<string, string> stringToEscape = new Dictionary<string, string>();
        public static Dictionary<string, bool> stringToVerbatim = new Dictionary<string, bool>();
        public static StringBuilder builder = new StringBuilder();
        
        public Simplex Escape(EscapeProfile escaper, int segmentIndex)
        {
            //return this;
            var segment = this[segmentIndex];
            //var useVerbatim = segment.Contains("\\") || segment.Contains("/");
       
            var useVerbatim = stringToVerbatim.TryGetValue(segment, out var v)
                ? v
                : stringToVerbatim[segment] = segment.Contains("\\") || segment.Contains("/");

            var escapeChars = useVerbatim ? escaper.VerbatimEscapeChars : escaper.EscapeChars;

            //hits = (useVerbatim ? hits.Where(h => h.Marker == "\"") : hits.Where(h => h.Marker != "\"")).ToArray();
            //this[segmentIndex] = //stringToEscape.TryGetValue(segment, out var v) ? v : stringToEscape[segment] =
                //new StringBuilder().Escape(segment, ProvideHits(segment), Escaper.EscapeRules, "\\").ToString();
            this[segmentIndex] = //stringToEscape.TryGetValue(segment, out var v) ? v : stringToEscape[segment] =
                escaper.AppendWithEscape(builder.Clear(), segment, escapeChars, useVerbatim).ToString();
            if (useVerbatim) Insert(segmentIndex - 1, escaper.VerbatimPattern);
            return this;
        }

        private static List<Marker.Hit> ProvideHits(string segment)
        {
            var hits = new List<Marker.Hit>();
            Escaper.EscapeRules.ForEach(r => hits.AddRange(Marker.GetHits(segment, r.Key)));
            hits.Sort((x, y) => x.Offset - y.Offset);
            return hits;
        }
    }

    public class SimplexConverter : StringConverter, IConverter<Simplex, object>
    {
        public bool AppendSyffixes = true;
        public bool AppendTypeInfo = true;
        public EscapeProfile Escaper { get; }

        public SimplexConverter(EscapeProfile escaper) => Escaper = escaper;

        public StringBuilder Convert(StringBuilder builder, object value, params object[] args)
        {
            if (value is string stringValue)
            {
                for (int i = 0; i < stringValue.Length; i++)
                {
                    var c = stringValue[i];
                    c = c;
                }
                return builder.Append(Escaper.HeadPatterns[0]).Append(stringValue).Append(Escaper.TailPatterns[0]);
            }
            //else return builder.Append((value ?? NullLiteral));

            stringValue = base.Convert(value);
            if (stringValue != null)
                return value != null && AppendSyffixes && TypeToSyffix.TryGetValue(value.GetType(), out var suffix)
                    ? builder.Append(stringValue).Append(suffix)
                    : builder.Append(stringValue);

            stringValue = ConvertComplex(value) ?? 
                          value.ToString();
            
            if (!AppendTypeInfo)
                return builder.Append(Escaper.HeadPatterns[0]).Append(stringValue).Append(Escaper.TailPatterns[0]);
            
            var type = value.GetType();
            var typeName = TypeToNameCache.TryGetValue(type, out var name)
                ? name
                : TypeToNameCache[type] = type.Assembly == SystemAssembly || type == typeof(Uri)
                    ? type.Name
                    : type.AssemblyQualifiedName;

            return builder.Append(Escaper.HeadPatterns[0]) /* " */
                .Append(stringValue) /* value */
                .Append(Escaper.TailPatterns[0]) /* " */
                .Append(Escaper.HeadPatterns[1]) /* < */
                .Append(typeName) /* type */
                .Append(Escaper.TailPatterns[1]); /* > */

        }

        public readonly Assembly SystemAssembly = typeof(object).Assembly;
        private Dictionary<Type, string> TypeToNameCache = new Dictionary<Type, string>();

        public object Convert(Simplex simplex, params object[] args)
        {
            var value = simplex.Count == 1 ? simplex[0] : simplex[1];
            if (simplex.Count == 3) return value;
            return simplex.Count > 3 ? ConvertComplex(value, simplex[4]) : base.Convert(value);
        }
    }
}
