﻿using System;
using System.Collections;
using System.Text;

namespace Art.Replication
{
    public interface IConverter<in TIn, out TOut>
    {
        TOut Convert(TIn value);
    }

    public interface IBodyProfile<in TIn, out TOut>
    {
        string GetHead(TIn body);
        string GetTail(TIn body);
        TOut MatchHead(string data, ref int offset);
        TOut MatchTail(string data, ref int offset);
    }

    public class BodyProfile<T> : IBodyProfile<T, bool>
    {
        public string Head { get; set; }
        public string Tail { get; set; }

        public string GetTail(T body) => Tail;
        public string GetHead(T body) => Head;
        public bool MatchHead(string data, ref int offset) => Match(data, ref offset, Head);
        public bool MatchTail(string data, ref int offset) => Match(data, ref offset, Tail);

        public bool Match(string data, ref int offset, string pattern)
        {
            var isMatched = data.Substring(offset, pattern.Length) == pattern;
            offset = isMatched ? offset + Tail.Length : offset;
            return isMatched;
        }
    }

    public class KeepProfile : IBodyProfile<Map, string>, IBodyProfile<Set, string>
    {
        public static KeepProfile GetFormatted()
        {
            var escapeProfile = new EscapeProfile();
            return new KeepProfile
            {
                EscapeProfile = escapeProfile,
                SimplexConverter = new SimplexConverter(escapeProfile)
                {
                    NullLiteral = "null",
                    TrueLiteral = "true",
                    FalseLiteral = "false",
                },
                MapPairSplitter = ": ",
                IndentChars = "  ",
                NewLineChars = Environment.NewLine,
                Delimiter = ","
            };
        }

        public ReplicationProfile ReplicationProfile { get; set; }

        public const string Map = "Map";
        public const string Set = "Set";
        public const string Simplex = "Simplex";

        public EscapeProfile EscapeProfile = new EscapeProfile();
        public SimplexConverter SimplexConverter;
        public IBodyProfile<Map, bool> MapBody = new BodyProfile<Map> {Head = "{", Tail = "}"};
        public IBodyProfile<Set, bool> SetBody = new BodyProfile<Set> {Head = "[", Tail = "]"};

        public string MapPairSplitter { get; set; } = ": ";

        public string Delimiter { get; set; }

        public bool UseTailDelimiter { get; set; }

        //public DateTimeFormat DateTimeFormat { get; set; }
        public string IndentChars { get; set; }

        public string NewLineChars { get; set; }

        public string GetHead(Map body) => MapBody.GetHead(body);

        public string GetTail(Map body) => MapBody.GetTail(body);

        public string GetHead(Set body) => SetBody.GetHead(body);

        public string GetTail(Set body) => SetBody.GetTail(body);

        public string MatchHead(string data, ref int offset)
        {
            MoveToItem(data, ref offset);
            return MapBody.MatchHead(data, ref offset)
                ? Map
                : SetBody.MatchHead(data, ref offset)
                    ? Set
                    : Simplex;
        }

        public string CaptureSimplex(string data, ref int offset)
        {
            MoveToSimplex(data, ref offset);
            return EscapeProfile.CaptureSimplex(data, ref offset);
        }

        public void MoveToSimplex(string data, ref int offset)
        {
            while (offset < data.Length && !(char.IsLetterOrDigit(data[offset]) ||
                                             EscapeProfile.AllowedSimplexSymbols.Contains(data[offset]))) offset++;
        }

        public void MoveToItem(string data, ref int offset)
        {
            while (offset < data.Length && !IsItem(data[offset])) offset++;
        }


        public bool IsItem(char c) => char.IsLetterOrDigit(c) || EscapeProfile.AllowedSimplexSymbols.Contains(c) ||
                                      c == '{' || c == '[';

        public string MatchTail(string data, ref int offset)
        {
            SkipWhiteSpace(data, ref offset);
            return MapBody.MatchTail(data, ref offset)
                ? Map
                : SetBody.MatchTail(data, ref offset)
                    ? Set
                    : Simplex;
        }

        public bool MatchTail(string data, ref int offset, bool isMap)
        {
            SkipWhiteSpace(data, ref offset);
            return isMap ? MapBody.MatchTail(data, ref offset) : SetBody.MatchTail(data, ref offset);
        }

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

        public void SkipWhiteSpace(string data, ref int offset)
        {
            while (offset < data.Length && char.IsWhiteSpace(data[offset])) offset++;
        }

        public string GetHeadIndent(int indentLevel, ICollection items, int index)
        {
            var indent = string.Empty;
            for (var i = 0; i < indentLevel; i++)
            {
                indent += IndentChars;
            }

            return NewLineChars + indent;
        }

        public string GetTailIndent(int indentLevel, ICollection items, int index)
        {
            return items.Count == ++index && !UseTailDelimiter
                ? GetHeadIndent(indentLevel - 1, items, index)
                : Delimiter;
        }

        public void AppendKey(StringBuilder builder, string key)
        {
            builder.Append(key);
            builder.Append(MapPairSplitter);
        }
    }
}
