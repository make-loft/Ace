using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Ace.Serialization
{
	public class EscapeProfile
	{
		public char EscapeSequence = '\\';
		public char VerbatimEscapeSequence = '\"';

		public string VerbatimPattern = "@";
		public List<string> HeadPatterns = new List<string> {"\"", "<", "'"};
		public List<string> TailPatterns = new List<string> {"\"", ">", "'"};

		public Dictionary<char, string> VerbatimEscapeChars = new Dictionary<char, string> {{'\"', "\""}};

		public Dictionary<char, string> EscapeChars = new Dictionary<char, string>
		{
			{'\"', "\""},
			{'\\', "\\"},
			{'/', "/"},
			{'\b', "b"},
			{'\f', "f"},
			{'\n', "n"},
			{'\r', "r"},
			{'\t', "t"},
		};

		public Dictionary<char, char> VerbatimUnescapeChars = new Dictionary<char, char> {{'"', '\"'}};

		public Dictionary<char, char> UnescapeChars = new Dictionary<char, char>
		{
			{'"', '\"'},
			{'\\', '\\'},
			{'/', '/'},
			{'b', '\b'},
			{'f', '\f'},
			{'n', '\n'},
			{'r', '\r'},
			{'t', '\t'},
		};


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

		public string Convert(string value)
		{
			var useVerbatim = value.Contains("\\") || value.Contains("/");
			var escapeChars = useVerbatim ? VerbatimEscapeChars : EscapeChars;
			var escapeSequence = useVerbatim ? VerbatimEscapeSequence : EscapeSequence;
			var builder = new StringBuilder();
			if (useVerbatim) builder.Append(VerbatimPattern);
			AppendWithEscape(builder, value, escapeChars, useVerbatim, escapeSequence);
			return builder.ToString();
		}

		public readonly List<char> NonSimplexChars =
			new List<char> {'{', '}', '[', ']', ',', ';', ':'}; // and all whitespaces

		public bool IsNonSimplex(char c) => char.IsWhiteSpace(c) || NonSimplexChars.Contains(c);

		public Simplex CaptureSimplex(string data, ref int offset)
		{
			var simplex = new Simplex();
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
				if (headPattern != null)
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
						if (s == breakPattern[0]) offset++;
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
