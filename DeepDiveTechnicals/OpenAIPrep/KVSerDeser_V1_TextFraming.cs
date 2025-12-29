using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DeepDiveTechnicals.OpenAIPrep;

public sealed class KVSerDeser : IEquatable<KVSerDeser>
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    private Dictionary<string, string?> _kvP = new();


    public KVSerDeser()
    {

    }

    public KVSerDeser(Dictionary<string, string?> seed)
    { 
        _kvP = seed; 
    }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    public void Set(string key, string value)
    {
        _kvP.TryAdd(key, value);
    }

    public string Get(string key)
    {
        return _kvP[key];
    }

    public byte[] Serialize()
    {
        using var buffer = new MemoryStream();
        using var bufferWriter = new StreamWriter(buffer, encoding: Encoding.UTF8);        
        bufferWriter.Write('*');
        bufferWriter.Write(_kvP.Keys.Count);
        bufferWriter.Write("\r\n");
        foreach (var (key, value) in _kvP)
        {
            bufferWriter.Write('$');
            bufferWriter.Write(key.Length);
            bufferWriter.Write("\r\n");
            bufferWriter.Write(key);
            bufferWriter.Write("\r\n");

            bufferWriter.Write('$');
            if (value is null)
            {
                // Null representation Redis style
                bufferWriter.Write(-1);
            }
            else
            {
                bufferWriter.Write(value.Length);
                bufferWriter.Write("\r\n");
                bufferWriter.Write(value);
            }
            bufferWriter.Write("\r\n");
        }

        bufferWriter.Flush();
        return buffer.ToArray();
    }

    public Dictionary<string,string> Deserialize(Stream input)
    {
        using var bufferReader = new StreamReader(input);
        var result = new Dictionary<string, string>();
        var expectedSerializedResultSize = 0;

        var keyFound = false;
        string currentKey = string.Empty;

        int byt;
        while ((byt = bufferReader.Read()) != -1) // -1 is EoF
        {
            if (byt == '*')
            {
                var line = bufferReader.ReadLine();
                expectedSerializedResultSize = int.Parse(line); // let's assume a size less than 10 for now for testing;
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
                    result.Add(currentKey, FindValue(bufferReader));
                    continue;
                }
                else
                {
                    keyFound = true;
                    currentKey = FindKey(bufferReader);
                    continue;
                }
            }

            if (byt == '\n' || byt == '\r')
            {
                // half delimiter
                continue;
            }
        }

        return result;
    }

    private static string FindKey(StreamReader bufferReader)
    {
        var lengthStr = new StringBuilder();
        string keyResult;
        do
        {
            var byt = bufferReader.Read();
            if (byt == '\r')
            {
                continue;
            }
            else if (byt == '\n')
            {
                // in case the key is a stringified nubmer, that's why we don't check only for a-z A-Z ascii
                var length = Convert.ToInt32(lengthStr.ToString()); 

                var key = new char[length];// allocate length memory
                bufferReader.Read(key, 0, length);
                keyResult = string.Concat(key);
                break;
            }
            else
            {
                lengthStr.Append((char)byt);
            }
        }
        while (true);

        return keyResult;
    }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    private string? FindValue(StreamReader bufferReader)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
        {
            string valueResult;
            var lengthStr = new StringBuilder();
            do
            {
                var byt = bufferReader.Read();
                if (byt == '\r')
                {
                    continue;
                }
                else if (byt == '\n')
                {
                    var length = Convert.ToInt16(lengthStr.ToString());

                    var value = new char[length];// allocate length memory
                    bufferReader.Read(value, 0, length);
                    valueResult = string.Concat(value);
                    break;
                }
                else
                {
                    if (byt == '-')
                    {
                        if (bufferReader.Read() == '1')
                        {
                            // null value;
                            return null;
                        }
                        else
                        {
                            throw new InvalidCastException($"Invalid value prefix. A value prefix should be the length of the value or {-1} if it's null");
                        }
                    }
                    lengthStr.Append((char)byt);
                }
            }
            while (true);

            return valueResult;
        }
    }

    public bool Equals(KVSerDeser other)
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
            this.Equals(other as KVSerDeser);
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