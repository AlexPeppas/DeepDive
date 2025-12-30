using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;

namespace DeepDiveTechnicals.OpenAIPrep;
#nullable enable
public sealed class KVSerDeser_V2_ByteBuffers : IEquatable<KVSerDeser_V2_ByteBuffers>
{
    private Dictionary<string, string?> _kvP = new();
    
    public KVSerDeser_V2_ByteBuffers()
    {

    }

    public KVSerDeser_V2_ByteBuffers(Dictionary<string, string?> seed)
    {
        _kvP = seed;
    }

    public void Set(string key, string value)
    {
        _kvP[key] = value;
    }

    public string? Get(string key)
    {
        _kvP.TryGetValue(key, out var value);

        return value;
    }

    private void WriteByte(MemoryStream stream, byte b)
    {
        stream.WriteByte(b);
    }

    /// <summary>
    /// Everything happens on the stack, zero heap allocations
    /// Less GC Pressure
    /// Better CPU cache utilization
    /// Faster Execution
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="numb"></param>
    private void WriteAsciiInt(MemoryStream stream, int numb)
    {
        Span<byte> span = stackalloc byte[12];
        Utf8Formatter.TryFormat(numb, span, out var bytesWritten);
        stream.Write(span[..bytesWritten]);
        // if instead I did Encoding.UTF8.GetBytes(numb.Tostring()) I would allocate a new string on the heap and get bytes would allocate a new byte[] on the heap
    }

    private void WriteUtf8String(MemoryStream stream, string value)
    {
        var byteNeeded = Encoding.UTF8.GetByteCount(value);
        var rented = ArrayPool<byte>.Shared.Rent(byteNeeded);
        Encoding.UTF8.GetBytes(value.AsSpan(), rented);
        stream.Write(rented.AsSpan()[..byteNeeded]);
        ArrayPool<byte>.Shared.Return(rented);
    }

    public byte[] Serialize()
    {
        ReadOnlySpan<byte> CRLF = "\r\n"u8; // encodes as utf8 the string at compile time for string literals

        using var bufferWriter = new MemoryStream();
        WriteByte(bufferWriter, (byte) '*');
        WriteAsciiInt(bufferWriter, _kvP.Keys.Count);
        bufferWriter.Write(CRLF);

        foreach (var (key, value) in _kvP)
        {
            WriteByte(bufferWriter, (byte)'$');
            WriteAsciiInt(bufferWriter, Encoding.UTF8.GetByteCount(key));
            bufferWriter.Write(CRLF);
            WriteUtf8String(bufferWriter, key);
            bufferWriter.Write(CRLF);

            WriteByte(bufferWriter, (byte)'$');
            if (value is null)
            {
                // Null representation Redis style
                WriteAsciiInt(bufferWriter, -1);
            }
            else
            {

                WriteAsciiInt(bufferWriter, Encoding.UTF8.GetByteCount(value));
                bufferWriter.Write(CRLF);
                WriteUtf8String(bufferWriter, value);
            }
            bufferWriter.Write(CRLF);
        }

        bufferWriter.Flush();
        return bufferWriter.ToArray();
    }

    public Dictionary<string, string?> Deserialize(Stream input)
    {
        var result = new Dictionary<string, string?>();
        var expectedSerializedResultSize = 0;

        var keyFound = false;
        string currentKey = string.Empty;

        int byt;

        Span<byte> dictionarySizeEst = stackalloc byte[11]; // int max for the dictionary's length
        var sizeIndex = 0;

        while ((byt = input.ReadByte()) != -1) // -1 is EoF
        {
            if (byt == '*')
            {
                do{
                    byt = input.ReadByte();
                    
                    if (byt >= '0' && byt <= '9')
                    {
                        dictionarySizeEst[sizeIndex++] = (byte)byt;
                        continue;
                    }
                    if (byt == '\r')
                    {
                        byt = input.ReadByte();
                        if (byt == '\n')
                        {
                            // CRLF found
                            break;
                        }
                        else
                        {
                            throw new InvalidDataException("CRLF not found");
                        }
                    }
                    else
                    {
                        // if it's neither number nor CRLF throw. Length must be numerical
                        throw new InvalidCastException($"Invalid length prefix");
                    }
                }
                while (true);

                if (!Utf8Parser.TryParse(dictionarySizeEst[..sizeIndex], out expectedSerializedResultSize, out int _))
                {
                    throw new InvalidDataException("Bad dictionary size.");
                }
                sizeIndex = 0;
                dictionarySizeEst.Clear();
                continue; // Init, used for bulk if needed later
            }

            if (byt == '$')
            {
                // fail early
                if (result.Count == expectedSerializedResultSize)
                {
                    throw new InvalidOperationException("Serialized value contains more elements than expected");
                }

                if (keyFound)
                {
                    keyFound = false;
                    result.Add(currentKey, FindValue(input)!);
                    continue;
                }
                else
                {
                    keyFound = true;
                    currentKey = FindKey(input);
                    continue;
                }
            }

            if (byt == '\r')
            {
                byt = input.ReadByte();
                if (byt == '\n')
                {
                    continue;
                }
                else
                {
                    throw new InvalidDataException("CRLF not found");
                }
            }
        }

        return result!;
    }

    private static string FindKey(Stream bufferReader)
    {
        Span<byte> lengthInBytes = stackalloc byte[11]; // key length is assumed 11 bytes which is Int.Max for Int32. more than enough
        var index = 0;
        string keyResult;
        do
        {
            var byt = bufferReader.ReadByte();

            if (byt == -1)
            {
                throw new IndexOutOfRangeException($"Unexpected EoF, the value is truncated!");
            }
            if (byt == '\r')
            {
                byt = bufferReader.ReadByte();
                if (byt == '\n')
                {
                    // in case the key is a stringified nubmer, that's why we don't check only for a-z A-Z ascii
                    _ = Utf8Parser.TryParse(lengthInBytes[..index], out int length, out _);

                    var keyBuffered = new byte[length];// allocate length memory
                    bufferReader.ReadExactly(keyBuffered, 0, length);
                    keyResult = Encoding.UTF8.GetString(keyBuffered);
                    break;
                }
                else
                {
                    throw new InvalidDataException("CRLF not found");
                }
            }
            else
            {
                lengthInBytes[index++] = (byte)byt;
            }
        }
        while (true);

        return keyResult;
    }

    private string? FindValue(Stream bufferReader)
    {
        string valueResult;
        Span<byte> lengthInBytes = stackalloc byte[11]; // key length is assumed 11 bytes which is Int.Max for Int32. more than enough
        var index = 0;
        do
        {
            var byt = bufferReader.ReadByte();
            if (byt == -1)
            {
                throw new IndexOutOfRangeException($"Unexpected EoF, the value is truncated!");
            }

            if (byt == '\r')
            {
                byt = bufferReader.ReadByte();
                if (byt == '\n')
                {
                    _ = Utf8Parser.TryParse(lengthInBytes[..index], out int length, out _);

                    var valueBuffered = new byte[length];// allocate length memory
                    bufferReader.ReadExactly(valueBuffered, 0, length);
                    valueResult = Encoding.UTF8.GetString(valueBuffered);
                    break;
                }
                else
                {
                    throw new InvalidDataException("CRLF not found");
                }
            }
            else
            {
                // if we localize a -1 it means that the value is null. Run a short-loop validation
                if (byt == '-')
                {
                    do
                    {
                        // guard against malformed prefixes with this short-loop
                        byt = bufferReader.ReadByte();
                        if (byt == '1')
                        {
                            byt = bufferReader.ReadByte();
                            if (byt == '\r')
                            {
                                byt = bufferReader.ReadByte();
                                if (byt == '\n')
                                {
                                    return null;
                                }
                                else
                                {
                                    throw new InvalidDataException("CRLF not found");
                                }
                            }
                            else 
                            {
                                throw new InvalidDataException("CRLF not found");
                            }
                        }
                        else
                        {
                            throw new InvalidCastException($"Invalid value prefix. A value prefix should be the length of the value or {-1} if it's null");
                        }
                    }
                    while (true);
                }

                lengthInBytes[index++] = (byte)byt;
            }
        }
        while (true);

        return valueResult;
    }

    public bool Equals(KVSerDeser_V2_ByteBuffers other)
    {
        return this is not null &&
            other is not null &&
            other._kvP.Count == this._kvP.Count
            && this._kvP.Keys.All(key => other._kvP.ContainsKey(key) && this._kvP[key] == other._kvP[key]);
    }

    public override bool Equals(object other)
    {
        return this is not null &&
            other is not null &&
            this.Equals(other as KVSerDeser_V2_ByteBuffers);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(_kvP.Count);
        foreach (var item in _kvP)
        {
            hash.Add(item.GetHashCode());
        }

        return hash.ToHashCode();
    }
}
