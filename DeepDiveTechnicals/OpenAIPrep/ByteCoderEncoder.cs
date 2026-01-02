using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Unicode;

using FluentAssertions;

namespace DeepDiveTechnicals.OpenAIPrep
{
    public sealed class ByteCoderEncoder
    {
        public MemoryStream Encode(KeyValuePair<string, string> item)
        {
            ReadOnlySpan<byte> crlf = "\r\n"u8;
            using var stream = new MemoryStream();

            WriteRawByte(stream, '$');
            WriteInt(stream, Encoding.UTF8.GetByteCount(item.Key));
            stream.Write(crlf);
            WriteString(stream, item.Key);
            stream.Write(crlf);

            WriteRawByte(stream, '$');
            WriteInt(stream, Encoding.UTF8.GetByteCount(item.Value));
            stream.Write(crlf);
            WriteString(stream, item.Value);
            stream.Write(crlf);

            return stream;
        }

        public KeyValuePair<string,string> Decode(Stream stream)
        {
            int byt;
            bool foundKey = false;
            var key = string.Empty;
            var value = string.Empty;
            while ((byt = stream.ReadByte()) != -1)
            {
                if ((char)byt == '$')
                {
                    if (!foundKey)
                    {
                        foundKey = true;
                        key = FindKey(stream as MemoryStream);
                    }
                    else
                    {
                        value = FindValue(stream as MemoryStream);
                        return new(key, value);
                    }
                }
                else if ((char)byt == '\r')
                {
                    byt = stream.ReadByte();
                    if ((char)byt == '\n')
                    {
                        continue;
                    }
                    else
                    {
                        throw new InvalidDataException("Malformed stream");
                    }
                }
            }

            return new KeyValuePair<string,string>(key, value);
        }

        private string FindKey(MemoryStream stream)
        {
            int byt;
            Span<byte> keyLength = stackalloc byte[11];
            var index = 0; 

            while ((byt = stream.ReadByte()) != -1)
            {
                if ((char)byt>='0' && (char)byt<='9')
                {
                    keyLength[index++] = (byte)byt;
                }
                else if ((char)byt == '\r')
                {
                    byt = stream.ReadByte();
                    if ((char)byt == '\n')
                    {
                        Utf8Parser.TryParse(keyLength, out int keyLengthInt, out _);
                        var rented = ArrayPool<byte>.Shared.Rent(keyLengthInt);
                        stream.ReadExactly(rented, 0, keyLengthInt);
                        var key = Encoding.UTF8.GetString(rented[..keyLengthInt]);
                        ArrayPool<byte>.Shared.Return(rented);
                        return key;
                    }
                    else
                    {
                        throw new InvalidDataException("Malformed stream");
                    }
                }
            }

            throw new ArgumentException("Key not found");
        }

        private string FindValue(MemoryStream stream)
        {
            int byt;
            Span<byte> valueLength = stackalloc byte[11];
            var index = 0;

            while ((byt = stream.ReadByte()) != -1)
            {
                if ((char)byt >= '0' && (char)byt <= '9')
                {
                    valueLength[index++] = (byte)byt;
                }
                else if ((char)byt == '\r')
                {
                    byt = stream.ReadByte();
                    if ((char)byt == '\n')
                    {
                        Utf8Parser.TryParse(valueLength, out int valueLengthInt, out _);

                        var rented = ArrayPool<byte>.Shared.Rent(valueLengthInt);
                        stream.ReadExactly(rented, 0, valueLengthInt);
                        var value = Encoding.UTF8.GetString(rented[..valueLengthInt]);
                        ArrayPool<byte>.Shared.Return(rented);
                        return value;
                    }
                    else
                    {
                        throw new InvalidDataException("Malformed stream");
                    }
                }
            }

            throw new ArgumentException("Value not found");
        }

        private void WriteRawByte(Stream stream, char c)
        {
            stream.WriteByte((byte)c);
        }

        private void WriteInt(Stream stream, int value)
        {
            Span<byte> bytes = stackalloc byte[11];

            Utf8Formatter.TryFormat(value, bytes, out var bytesWritten);
            stream.Write(bytes[..bytesWritten]);
        }

        private void WriteString(Stream stream, string value)
        {
            var bytesNeeded = Encoding.UTF8.GetByteCount(value);
            var rented = ArrayPool<byte>.Shared.Rent(bytesNeeded);
            Encoding.UTF8.GetBytes(value, rented.AsSpan());
            stream.Write(rented.AsSpan()[..bytesNeeded]);
            ArrayPool<byte>.Shared.Return(rented);
        }
    }
}
