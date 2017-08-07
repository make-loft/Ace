using System;
using System.Collections;
using System.Reflection;
using System.Text;
using Art.Replication;

namespace Art.Serialization
{
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
        public StringBuilder WriteKey(StringBuilder builder, string key, int indentLevel)
        {
            return builder
                .Append(GetKeyHead(key))
                .Append(key)
                .Append(GetKeyTail(key))
                .Append(MapPairSplitter);
        }
        
        public static Assembly SystemAssembly = typeof(object).Assembly;
        public static Assembly ExtendedAssembly = typeof(Uri).Assembly;

        public string GetTypeName(Type type) =>
            type.Assembly == SystemAssembly || type.Assembly == ExtendedAssembly
                ? type.Name
                : type.AssemblyQualifiedName;

        public string GetHead(Type type) => "<";
        public string GetTail(Type type) => ">";

        public static KeepProfile GetFormatted() => new KeepProfile();

        public const string Map = "Map";
        public const string Set = "Set";

        public EscapeProfile EscapeProfile = new EscapeProfile();
        public SimplexConverter SimplexConverter = new SimplexConverter();
            
        public IBodyProfile<Map, bool> MapBody = new BodyProfile<Map> {Head = "{", Tail = "}"};
        public IBodyProfile<Set, bool> SetBody = new BodyProfile<Set> {Head = "[", Tail = "]"};

        public string MapPairSplitter { get; set; } = ": ";
        public string Delimiter { get; set; } = ",";
        public bool UseTailDelimiter { get; set; } = false;
        public string IndentChars { get; set; } = "  ";
        public string NewLineChars { get; set; } = Environment.NewLine;
        public bool AppendCountComments = false;

        public string KeyHead = null; //"\"";
        public string KeyTail = null; //"\"";
        public string GetKeyHead(object key) => KeyHead;
        public string GetKeyTail(object key) => KeyTail;

        public string GetHead(object body) => body is Map m
            ? GetHead(m)
            : body is Set s
                ? GetHead(s)
                : body == null || body.GetType().IsPrimitive
                    ? null
                    : "\"";
        
        public string GetTail(object body) => body is Map m
            ? GetTail(m)
            : body is Set s
                ? GetTail(s)
                : body == null || body.GetType().IsPrimitive
                    ? null
                    : "\"";

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
                    : null;
        }

        public void SkipMapPairSplitter(string data, ref int offset)
        {
            SkipWhiteSpaceWithComments(data, ref offset);
            if (data.Match(MapPairSplitter, offset)) offset += MapPairSplitter.Length;
        }

        public string CaptureKey(string data, ref int offset)
        {
            var simplex = CaptureSimplex(data, ref offset);
            return simplex.Count > 1 ? simplex[1] : simplex[0];
        }
        
        public Simplex CaptureSimplex(string data, ref int offset)
        {
            MoveToSimplex(data, ref offset);
            return EscapeProfile.CaptureSimplex(data, ref offset);
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

        public string MatchTail(string data, ref int offset)
        {
            SkipWhiteSpaceWithComments(data, ref offset);
            return MapBody.MatchTail(data, ref offset)
                ? Map
                : SetBody.MatchTail(data, ref offset)
                    ? Set
                    : null;
        }

        public bool MatchTail(string data, ref int offset, bool isMap)
        {
            SkipWhiteSpaceWithComments(data, ref offset);
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

        public string GetHeadIndent(int indentLevel, ICollection items, int index)
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

        public string GetTailIndent(int indentLevel, ICollection items, int index) =>
            items.Count == ++index
                ? (UseTailDelimiter ? Delimiter : null) + GetHeadIndent(indentLevel - 1, items, index)
                : Delimiter;
    }
}
