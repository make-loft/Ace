using System;
using System.Collections.Generic;
using System.Linq;
using Ace.Replication.Models;
using Ace.Serialization.Serializators;

namespace Ace.Serialization
{
	public interface IBodyProfile<in TIn, out TOut>
	{
		string GetHead(TIn body);
		string GetTail(TIn body);
		TOut CreateSingleModel(string data, ref int offset);
		bool TryFindTail(string data, ref int offset);
	}

	public class BodyProfile<T> : IBodyProfile<T, bool>
	{
		public string Head { get; set; }
		public string Tail { get; set; }

		public string GetTail(T body) => Tail;
		public string GetHead(T body) => Head;
		public bool CreateSingleModel(string data, ref int offset) => Match(data, ref offset, Head);
		public bool TryFindTail(string data, ref int offset) => Match(data, ref offset, Tail);

		public bool Match(string data, ref int offset, string pattern)
		{
			var isMatched = data.Substring(offset, pattern.Length) == pattern;
			offset = isMatched ? offset + Tail.Length : offset;
			return isMatched;
		}
	}

	public class KeepProfile
	{
		public static readonly List<ASerializator> Serializators = New.List<ASerializator>
		(
			new MapDeepSerializator(),
			new SetDeepSerializator(),
			new ValueSerializator()
		);
			
		public object ReadItem(string data, ref int offset)
		{
			var model = CreateBlankModel(data, ref offset);
			return Serializators.FirstOrDefault(s => s.CanApply(model))?.Capture(model, this, data, ref offset);
		}

		public IEnumerable<string> ToStringBeads(object value, int indentLevel) =>
			Serializators.FirstOrDefault(s => s.CanApply(value))?.ToStringBeads(value, this, indentLevel);
		
		public string GetHead(Type type) => "<";
		public string GetTail(Type type) => ">";

		public static KeepProfile GetFormatted() => new KeepProfile();

		public EscapeProfile EscapeProfile = new EscapeProfile();
			
		public IBodyProfile<Map, bool> MapBody = new BodyProfile<Map> {Head = "{", Tail = "}"};
		public IBodyProfile<Set, bool> SetBody = new BodyProfile<Set> {Head = "[", Tail = "]"};

		public string MapPairSplitter { get; set; } = ": ";
		public string Delimiter { get; set; } = ",";
		public bool UseTailDelimiter { get; set; } = false;
		public string IndentChars { get; set; } = "  ";
		public string NewLineChars { get; set; } = Environment.NewLine;
		public bool AppendCountComments { get; set; } = false;
		public bool TrimKeys { get; set; } = true;

		public string KeyHead = "\"";
		public string KeyTail = "\"";
		public string GetKeyHead(object key) => key is "" || !TrimKeys ? KeyHead : null;
		public string GetKeyTail(object key) => key is "" || !TrimKeys ? KeyTail : null;

		public string GetHead(object body) =>
			body is Map m ? GetHead(m) :
			body is Set s ? GetHead(s) :
			body is null || body.GetType().IsPrimitive ? null :
			"\"";

		public string GetTail(object body) =>
			body is Map m ? GetTail(m) :
			body is Set s ? GetTail(s) :
			body is null || body.GetType().IsPrimitive ? null :
			"\"";

		public string GetHead(Map body) => MapBody.GetHead(body);
		public string GetTail(Map body) => MapBody.GetTail(body);
		public string GetHead(Set body) => SetBody.GetHead(body);
		public string GetTail(Set body) => SetBody.GetTail(body);

		public object CreateBlankModel(string data, ref int offset)
		{
			MoveToItem(data, ref offset);
			return
				MapBody.CreateSingleModel(data, ref offset) ? new Map() :
				SetBody.CreateSingleModel(data, ref offset) ? new Set() :
				(object) new Simplex();
		}

		public void SkipMapPairSplitter(string data, ref int offset)
		{
			SkipWhiteSpaceWithComments(data, ref offset);
			if (data.Match(MapPairSplitter, offset)) offset += MapPairSplitter.Length;
		}

		public string CaptureKey(string data, ref int offset)
		{
			var segments = CaptureSimplex(new Simplex(), data, ref offset);
			return segments.Count > 1 ? segments[1] : segments[0];
		}
		
		public Simplex CaptureSimplex(Simplex simplex, string data, ref int offset)
		{
			MoveToSimplex(data, ref offset);
			return EscapeProfile.CaptureSimplex(simplex, data, ref offset);
		}

		public void MoveToSimplex(string data, ref int offset)
		{
			SkipWhiteSpaceWithComments(data, ref offset);
			while (offset < data.Length && EscapeProfile.IsNonSimplex(data[offset])) offset++;
		}

		public void MoveToItem(string data, ref int offset)
		{
			SkipWhiteSpaceWithComments(data, ref offset);
			while (offset < data.Length && !IsItem(data[offset])) offset++;
		}

		public bool IsItem(char c) => !EscapeProfile.IsNonSimplex(c) || c == '{' || c == '[';

		public void SkipHeadIndent(string data, ref int offset)
		{
			while (offset < data.Length && char.IsWhiteSpace(data[offset])) offset++;
		}

		public void SkipTailIndent(string data, ref int offset)
		{
			while (offset < data.Length &&
				   (char.IsWhiteSpace(data[offset]) ||
					data[offset] == ',' || data[offset] == ';' || data[offset] == '.')) offset++;
		}

		public void SkipWhiteSpaceWithComments(string data, ref int offset)
		{
			do
			{
				while (offset < data.Length && char.IsWhiteSpace(data[offset])) offset++;
				if (!data.Match("/", offset)) return;
				if (data.Match("/*", offset)) offset = data.IndexOf("*/", offset, StringComparison.Ordinal) + 2;
				if (data.Match("//", offset)) offset = data.IndexOf(NewLineChars, offset, StringComparison.Ordinal) + 2;
				if (offset < 0) offset = data.Length;
			} while (offset < data.Length);
		}

		public string GetHeadIndent<TItem>(int indentLevel, ICollection<TItem> items, int index)
		{
			if (items is Set set && index < set.Count &&
				(set[index] == null || set[index].GetType().IsPrimitive)) return " ";

			var indent = string.Empty;
			for (var i = 0; i < indentLevel; i++)
			{
				indent += IndentChars;
			}

			return NewLineChars + indent;
		}

		public string GetTailIndent<TItem>(int indentLevel, ICollection<TItem> items, int index) =>
			items.Count == ++index
				? (UseTailDelimiter ? Delimiter : null) + GetHeadIndent(indentLevel - 1, items, index)
				: Delimiter;
	}
}
