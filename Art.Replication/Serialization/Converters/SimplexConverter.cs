using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Art.Replication;

namespace Art.Serialization.Converters
{
    public class Simplex : List<string>
    {
        public override string ToString() => this.Aggregate("", (a, b) => a + b);

        public Simplex Modify(EscapeProfile escapeProfile, int segmentIndex)
        {
            var segment = this[segmentIndex];
            var useVerbatim = segment.Contains("\\") || segment.Contains("/");
            var escapeChars = useVerbatim ? escapeProfile.VerbatimEscapeChars : escapeProfile.EscapeChars;
            this[segmentIndex] = escapeProfile.AppendWithEscape(new StringBuilder(), segment, escapeChars, useVerbatim).ToString();
            if (useVerbatim) Insert(segmentIndex - 1, "@");
            return this;
        }
    }

    public class SimplexConverter : StringConverter, IConverter<Simplex, object>, IConverter<object, Simplex>
    {
        public EscapeProfile EscapeConverter { get; }

        public SimplexConverter(EscapeProfile escapeConverter) =>
            EscapeConverter = escapeConverter;

        public string HeadQuote = "\"";
        public string TailQuote = "\"";
        public bool AppendSyffixes = true;

        public new Simplex Convert(object value, params object[] args)
        {
            if (value is string stringValue)
                return new Simplex {HeadQuote, stringValue, TailQuote}.Modify(EscapeConverter, 1);

            stringValue = base.Convert(value);
            if (stringValue != null)
                return value != null && AppendSyffixes && TypeToSyffix.TryGetValue(value.GetType(), out var suffix)
                    ? new Simplex {stringValue, suffix}
                    : new Simplex {stringValue};

            stringValue = ConvertComplex(value) ?? value.ToString();
            var type = value.GetType();
            var typeName = type.Assembly == typeof(object).Assembly || type == typeof(Uri)
                ? type.Name
                : type.AssemblyQualifiedName;
            return new Simplex
                {
                    HeadQuote,
                    stringValue,
                    TailQuote,
                    "<",
                    typeName,
                    ">"
                }.Modify(EscapeConverter, 4)
                .Modify(EscapeConverter, 1);
        }

        public object Convert(Simplex simplex, params object[] args)
        {
            var value = simplex.Count == 1 ? simplex[0] : simplex[1];
            if (simplex.Count == 3) return value;
            return simplex.Count > 3 ? ConvertComplex(value, simplex[4]) : base.Convert(value);
        }
    }
}
