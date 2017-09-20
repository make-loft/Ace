using System.IO;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace

namespace System.Runtime.Serialization.Json
{
    public class JsonReaderWriterFactory
    {
        public static Stream CreateJsonWriter(
            Stream stream, Encoding encoding, bool ownsStream = false,
            bool indent = true, string indentChars = "  ")
        {
            return new JsonWriter(stream, encoding, indent, indentChars);
        }
    }

    public class JsonWriter : Stream
    {
        private readonly Stream _stream;
        private readonly Encoding _encoding;
        private readonly bool _indent;
        private readonly string _indentChars;
        private int _indentLevel;

        public JsonWriter(Stream stream, Encoding encoding, bool indent, string indentChars)
        {
            _stream = stream;
            _encoding = encoding;
            _indent = indent;
            _indentChars = indentChars;
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_indent) buffer = InsertIndents(buffer, offset, count);
            _stream.Write(buffer, offset, count);
        }

        private Regex _safe = new Regex(@"[^\\]" + "(?<replace>\":)");

        public byte[] InsertIndents(byte[] buffer, int offset, int count)
        {
            var json = _encoding.GetString(buffer, 0, buffer.Length);
            for (var i = 0; i < json.Length; i++)
            {
                var indent = Environment.NewLine;
                switch (json[i])
                {
                    case '{':
                    case '[':
                        if (json[i+1] == ']') indent = _indentChars;
                        else indent += Repeat(_indentChars, ++_indentLevel);
                        json = json.Insert(i + 1, indent);
                        i += indent.Length;
                        break;
                    case ',':
                        indent += Repeat(_indentChars, _indentLevel);
                        json = json.Insert(i + 1, indent);
                        i += indent.Length;
                        break;
                    case ':':
                        if (json[i - 2] != '\\' && json[i - 1] == '\"')
                        {
                            json = json.Insert(i + 1, " ");
                            json = json.Insert(i, " ");
                            i += 2;
                        }
                        break;
                    case '}':
                    case ']':
                        indent += Repeat(_indentChars, --_indentLevel);
                        json = json.Insert(i, indent);
                        i += indent.Length;
                        break;
                }
            }

            return _encoding.GetBytes(json.ToCharArray());
        }

        private static string Repeat(string item, int times)
        {
            var repeatedData = string.Empty;
            for (var i = 0; i < times; i++)
            {
                repeatedData += item;
            }

            return repeatedData;
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }
    }
}
