using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ace.Serialization.Escapers
{
	public static class Marker
	{
		public class Hit
		{
			public readonly int Offset;
			public readonly string Matrix;
			public readonly string Marker;

			public Hit(string matrix, string marker, int offset)
			{
				Matrix = matrix;
				Marker = marker;
				Offset = offset;
			}
		}

		public static List<Hit> GetHits(string matrix, string marker)
		{
			var hits = new List<Hit>();
			for (var offset = 0;; offset += marker.Length)
			{
				offset = matrix.IndexOf(marker, offset, StringComparison.Ordinal);
				if (offset < 0) return hits;
				hits.Add(new Hit(matrix, marker, offset));
			}
		}

		public static IEnumerable<Hit> EnumerateHits(string matrix, IList<string> markers)
		{
			for (var i = 0; i < matrix.Length; i++)
			{
				var marker = markers.FirstOrDefault(m => matrix.Match(m, i));
				if (marker == null) continue;
				yield return new Hit(matrix, marker, i);
				i += marker.Length;
			}
		}
	}

	public static class Escaper
	{
		public static readonly Dictionary<string, string> EscapeRules = New.Dictionary
		(
			"/".Of("/"),
			"\"".Of("\""),
			"\\".Of("\\"),
			"\b".Of("b"),
			"\f".Of("f"),
			"\n".Of("n"),
			"\r".Of("r"),
			"\t".Of("t")
		);

		public static StringBuilder Escape(this StringBuilder builder, string matrix,
			IEnumerable<Marker.Hit> hits, Dictionary<string, string> rules, string escapeMarker, int offset = 0)
		{
			foreach (var hit in hits)
			{
				builder.Append(matrix, offset, hit.Offset - offset);
				builder.Append(escapeMarker);
				var value = rules[hit.Marker];
				builder.Append(value);
				offset = hit.Offset + hit.Marker.Length;
			}

			return builder.Append(matrix, offset, matrix.Length - offset);
		}
	}

	public class GeneralEscaper
	{
		public char EscapeChar = '\\';
		public List<string> HeadPatterns = New.List("\"", "<", "'");
		public List<string> TailPatterns = New.List("\"", ">", "'");

		public static readonly Dictionary<string, string> EscapeRules = New.Dictionary
		(
			"/".Of("/"),
			"\"".Of("\""),
			"\\".Of("\\"),
			"\b".Of("b"),
			"\f".Of("f"),
			"\n".Of("n"),
			"\r".Of("r"),
			"\t".Of("t")
		);

		public string EscapeSequence = "\\";
		public string BreakSequence = "\"";

		public string Escape(StringBuilder builder, string originalSequence)
		{
			return Escape(originalSequence, EscapeRules, EscapeSequence, BreakSequence)
				.Aggregate(builder, (b, s) => b.Append(s)).ToString();
		}

		public static IEnumerable<string> Escape(
			string originalSequence, Dictionary<string, string> rules,
			string escapeMarker, string breakMarker)
		{
			var j = 0;
			for (var i = 0; i < originalSequence.Length; i++)
			{
				var rule = rules.FirstOrDefault(r => originalSequence.Match(r.Key, i));
				if (rule.Key == null)
				{
					if (originalSequence.Match(breakMarker, i)) yield break;
					//yield return originalSequence;
				}
				else
				{
					i += rule.Value.Length;
					yield return originalSequence.Substring(j, i);
					j = i;
					yield return escapeMarker;
					yield return rule.Value;
				}
			}
		}
	}
}