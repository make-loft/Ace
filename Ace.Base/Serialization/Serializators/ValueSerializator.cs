using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Ace.Replication.Models;
using Ace.Serialization.Converters;
using Ace.Serialization.Escapers;

namespace Ace.Serialization.Serializators
{
	public class ValueSerializator : ASerializator
	{
		public bool AppendTypeInfo = true;

		public readonly List<Converter> Converters = New.List<Converter>
		(
			new NullConverter(),
			new BooleanConverter(),
			new NumericConverter(),
			new StringConverter(),
			new IsoDateTimeConverter(),
			new ComplexConverter()
		);
		
		public override object Capture(object value, KeepProfile profile, string data, ref int offset) =>
			Revert(profile.CaptureSimplex((Simplex) value, data, ref offset));

		public override IEnumerable<string> ToStringBeads(object value, KeepProfile profile, int indentLevel) => 
			Convert(value, profile);
		   
		public static readonly Assembly SystemAssembly = TypeOf<object>.Assembly;
		public static readonly Assembly ExtendedAssembly = TypeOf<Uri>.Assembly;

		public virtual string GetTypeName(Type type) =>
			type.Assembly.Is(SystemAssembly) || type.Assembly.Is(ExtendedAssembly)
				? type.Name
				: type.AssemblyQualifiedName;
		
		protected readonly Simplex Simplex = new Simplex();
	
		public Simplex Convert(object value, KeepProfile profile)
		{
			var convertedValue = Converters.Select(c => c.Convert(value)).FirstOrDefault(s => s.IsNot(null)) ??
								 throw new Exception("Can not convert value " + value);
			
			Simplex.Clear();
			Simplex.Add(profile.GetHead(value));
			Simplex.Add(convertedValue);
			Simplex.Add(profile.GetTail(value));
			
			var type = value?.GetType();
			if (type is null || type.IsPrimitive) return Simplex;
			if (type.Is(TypeOf.String.Raw)) return Escape(profile.EscapeProfile, Simplex, 1);

			if (!AppendTypeInfo) return Escape(profile.EscapeProfile, Simplex, 1);
			
			Simplex.Add(profile.GetHead(type));
			Simplex.Add(GetTypeName(type));
			Simplex.Add(profile.GetTail(type));
			return Escape(profile.EscapeProfile, Simplex, 1);
		}

		public object Revert(Simplex simplex)
		{
			var segments = simplex;
			if (segments.Count == 3) return segments[1]; /* optimization for strings */
			var convertedValue = segments.Count == 1 ? segments[0] : segments[1];
			var typeCode = segments.Count == 6 ? segments[4] : null;
			return Converters.Select(c => c.Revert(convertedValue, typeCode)).First(v => v.IsNot(Converter.Undefined));
		}
		
		public static Dictionary<string, bool> stringToVerbatim = new Dictionary<string, bool>();

		public Dictionary<int, StringBuilder> ThreadIdToStringBuilder = new Dictionary<int, StringBuilder>();
		
		public Simplex Escape(EscapeProfile escaper, Simplex segments,  int segmentIndex)
		{
			var threadId = Thread.CurrentThread.ManagedThreadId;
			if (!ThreadIdToStringBuilder.TryGetValue(threadId, out var builder))
				builder = ThreadIdToStringBuilder[threadId] = new StringBuilder(256);

			//return this;
			var segment = segments[segmentIndex];
			//var useVerbatim = segment.Contains("\\") || segment.Contains("/");

			var useVerbatim = stringToVerbatim.TryGetValue(segment, out var v)
				? v
				: stringToVerbatim[segment] = segment.Contains("\\") || segment.Contains("/");

			var escapeChars = useVerbatim ? escaper.VerbatimEscapeChars : escaper.EscapeChars;
			var escapeSequence = useVerbatim ? escaper.VerbatimEscapeSequence : escaper.EscapeSequence;

			//hits = (useVerbatim ? hits.Where(h => h.Marker == "\"") : hits.Where(h => h.Marker != "\"")).ToArray();
			segments[segmentIndex] = //stringToEscape.TryGetValue(segment, out var v) ? v : stringToEscape[segment] =
				//builder.Clear().Escape(segment, ProvideHits(segment), Escaper.EscapeRules, "\\").ToString();
				// this[segmentIndex] = //stringToEscape.TryGetValue(segment, out var v) ? v : stringToEscape[segment] =
				escaper.AppendWithEscape(builder.Clear(), segment, escapeChars, useVerbatim, escapeSequence).ToString();
			if (useVerbatim) segments.Insert(segmentIndex - 1, escaper.VerbatimPattern);
			return segments;
		}

		private static List<Marker.Hit> ProvideHits(string segment)
		{
			var hits = new List<Marker.Hit>();
			Escaper.EscapeRules.ForEach(r => hits.AddRange(Marker.GetHits(segment, r.Key)));
			hits.Sort((x, y) => x.Offset - y.Offset);
			return hits;
		}
	}
}