using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Ace.Replication.Models;

namespace Ace.Serialization
{
	public class EscapeProfile
	{
		public char EscapeSequence = '\\';
		public char VerbatimEscapeSequence = '\"';

		public string VerbatimPattern = "@";
		public List<string> HeadPatterns = New.List("\"", "(", "'");
		public List<string> TailPatterns = New.List("\"", ")", "'");

		public readonly Dictionary<char, string> VerbatimEscapeChars = New.Dictionary('\"'.Of("\""));

		public readonly Dictionary<char, string> EscapeChars = New.Dictionary
		(
			'\"'.Of("\""),
			'\\'.Of("\\"),
			'/'.Of("/"),
			'\b'.Of("b"),
			'\f'.Of("f"),
			'\n'.Of("n"),
			'\r'.Of("r"),
			'\t'.Of("t")
		);

		public readonly Dictionary<char, char> VerbatimUnescapeChars = New.Dictionary('"'.Of('\"'));

		public readonly Dictionary<char, char> UnescapeChars = New.Dictionary
		(
			'"'.Of('\"'),
			'\\'.Of('\\'),
			'/'.Of('/'),
			'b'.Of('\b'),
			'f'.Of('\f'),
			'n'.Of('\n'),
			'r'.Of('\r'),
			't'.Of('\t')
		);

		public StringBuilder AppendWithEscape(StringBuilder builder, string value, Dictionary<char, string> escapeChars,
			bool verbatim, char escapeSequence, bool asciMode = false)
		{
			if (value == null) return builder;

			foreach (var c in value)
			{
				if (escapeChars.TryGetValue(c, out var s))
				{
					builder.Append(escapeSequence);
					builder.Append(s);
				}
				else if (asciMode && !verbatim)
				{
					int i = c;
					if (i < 32 || 127 < i) builder.AppendFormat("\\u{0:x04}", i);
					else builder.Append(c);
				}
				else builder.Append(c);
			}

			return builder;
		}

		private readonly StringBuilder _builder = new StringBuilder();
		public string Convert(string value)
		{
			var useVerbatim = value.Contains("\\") || value.Contains("/");
			var escapeChars = useVerbatim ? VerbatimEscapeChars : EscapeChars;
			var escapeSequence = useVerbatim ? VerbatimEscapeSequence : EscapeSequence;
			_builder.Clear();
			if (useVerbatim) _builder.Append(VerbatimPattern);
			AppendWithEscape(_builder, value, escapeChars, useVerbatim, escapeSequence);
			return _builder.ToString();
		}

		public readonly List<char> NonSimplexChars =
			New.List('{', '}', '[', ']', ',', ';', ':'); // and all whitespaces

		public bool IsNonSimplex(char c) => char.IsWhiteSpace(c) || NonSimplexChars.Contains(c);

		public Simplex CaptureSimplex(Simplex simplex, string data, ref int offset)
		{
			var builder = new StringBuilder();

			for (; offset < data.Length;)
			{
				var c = data[offset];
				if (char.IsWhiteSpace(c) || NonSimplexChars.Contains(c))
				{
					if (builder.Length > 0) simplex.Add(builder.ToString());
					//if (!char.IsWhiteSpace(c)) simplex.Add(c.ToString());
					break;
				}

				var verbatimFlag = data.Match(VerbatimPattern, offset);
				if (verbatimFlag)
				{
					//simplex.Add(VerbatimPattern);
					offset += VerbatimPattern.Length;
				}

				var o = offset;
				var headPattern = HeadPatterns.FirstOrDefault(p => data.Match(p, o));
				if (headPattern.Is())
				{
					var escapeChar = verbatimFlag ? VerbatimEscapeSequence : EscapeSequence;
					var unescapeStrategy = verbatimFlag ? VerbatimUnescapeChars : UnescapeChars;
					simplex.Add(headPattern);
					offset += headPattern.Length;
					var index = HeadPatterns.IndexOf(headPattern);
					var tailPattern = TailPatterns[index];
					AppendEscapedLiteral(builder, data, ref offset, unescapeStrategy, escapeChar, tailPattern,
						verbatimFlag);
					simplex.Add(builder.ToString());
					builder.Clear();

					simplex.Add(tailPattern);
					offset += tailPattern.Length;
					if (offset > data.Length) throw new Exception("Unexpected end of escaped value: " + builder);
				}
				else
				{
					builder.Append(c);
					offset++;
				}
			}

			return simplex;
		}

		public static void AppendEscapedLiteral(
			StringBuilder builder, string data, ref int offset,
			Dictionary<char, char> unescapeStrategy, char escapeChar, string breakPattern, bool verbatim, bool asciMode = false)
		{
			for (; offset < data.Length; offset++)
			{
				var c = data[offset];
				var escapeFlag = c == escapeChar;
				if (escapeFlag)
				{
					var d = data[offset + 1];
					if (unescapeStrategy.TryGetValue(d, out var s))
					{
						builder.Append(s);
						offset++;
					}
					else if (asciMode && !verbatim && d == 'u')
					{
						c = (char) int.Parse(data.Substring(offset + 2, 4), NumberStyles.AllowHexSpecifier);
						builder.Append(c);
						offset += 5;
					}
					else if (data.Match(breakPattern, offset)) break;
					else builder.Append(c);
				}
				else
				{
					if (data.Match(breakPattern, offset)) break;
					builder.Append(c);
				}
			}
		}
	}
}
