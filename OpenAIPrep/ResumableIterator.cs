#pragma warning disable CA1416 // Validate platform compatibility
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepDiveTechnicals.OpenAIPrep
{
    public interface IResumableIterator<TState, TValue> where TState: BaseState, new()
    {
        TValue Next();

        TState GetState();

        void SetState(TState newState);
    }

    public sealed class ResumableListIterator : IResumableIterator<ListState, int>
    {
        private int _currentPointer = 0;
        private readonly IList<int> _seed;

        public ResumableListIterator(IEnumerable<int> seed)
        {
            if (seed is null)
            {
                throw new ArgumentNullException(nameof(seed));
            }

            _seed = [..seed];
        }

        public int Next()
        {
            if (_currentPointer >= _seed.Count)
            {
                throw new ArgumentOutOfRangeException("Iterator drained");
            }

            return _seed[_currentPointer++];
        }

        public ListState GetState()
        {
            return new ListState { Pointer = _currentPointer };
        }

        public void SetState(ListState newState)
        {
            if (newState is null)
            {
                throw new ArgumentNullException("Null state");
            }
            if (newState.Pointer < 0 || newState.Pointer > _seed.Count)
            {
                throw new ArgumentOutOfRangeException("Invalid State assignment");
            }

            _currentPointer = newState.Pointer;
        }
    }

    public sealed class ResumableNDJsonFileIterator<NDObject> : IResumableIterator<FileState, NDObject>
    {
        private long _currentByteOffset = 0;
        private readonly string _path;
        private long _lastTimeWriteEpochUTC;

        public ResumableNDJsonFileIterator(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            _path = path;
            _lastTimeWriteEpochUTC = File.GetLastWriteTimeUtc(path).Ticks;
        }

        public NDObject Next()
        {
            var lastAccess = File.GetLastWriteTimeUtc(_path).Ticks;
            if (lastAccess != _lastTimeWriteEpochUTC)
            {
                throw new InvalidOperationException("File has changed.");
            }

            byte[] byteArray;
            using var fileStream = File.OpenRead(_path);
            var lockOffset = _currentByteOffset;
            var lockLength = fileStream.Length - _currentByteOffset;
            fileStream.Lock(lockOffset, lockLength);
            
            try 
            {
                if (_currentByteOffset >= fileStream.Length)
                {
                    throw new InvalidOperationException($"{nameof(Next)}: EOF");
                }

                if (!fileStream.CanSeek)
                {
                    throw new InvalidOperationException(nameof(fileStream.CanSeek));
                }
                // set position to current offset
                fileStream.Position = _currentByteOffset;
                if (!fileStream.CanRead)
                {
                    throw new InvalidOperationException(nameof(fileStream.CanRead));
                }

                var lineBuffer = new MemoryStream();
                int byt;
                // read one line in bytes until \n or EOF
                while ((byt = fileStream.ReadByte()) != -1)
                {
                    if (byt == '\n') // found new line
                    {
                        break;
                    }

                    lineBuffer.WriteByte((byte)byt);
                }

                byteArray = lineBuffer.ToArray();
                if (byteArray.Length > 0 && byteArray[^1] == '\r')
                {
                    byteArray = byteArray[..^1]; // exclude \r
                }

                _currentByteOffset = fileStream.Position;

                if (byteArray.Length == 0 && byt == -1)
                {
                    throw new InvalidOperationException("EOF");
                }
            }
            finally
            {
                fileStream.Unlock(lockOffset, lockLength);
            }

            return JsonSerializer.Deserialize<NDObject>(byteArray);
        }

        public FileState GetState()
        {
            return new FileState { ByteOffset = _currentByteOffset, LastTimeWriteEpochUTC = _lastTimeWriteEpochUTC };
        }

        public void SetState(FileState newState)
        {
            if (newState is null)
            {
                throw new ArgumentNullException("Null state");
            }
            if (newState.ByteOffset < 0)
            {
                throw new ArgumentOutOfRangeException("Invalid State assignment");
            }

            _currentByteOffset = newState.ByteOffset;
            _lastTimeWriteEpochUTC = newState.LastTimeWriteEpochUTC;
        }
    }

    /*public sealed class ResumableMultiNDJsonFileIterator<NDObject> : IResumableIterator<MultiFileState, NDObject>
    {
        private readonly IList<string> _paths;
        private int _currentFileIndex;
        private readonly ConcurrentDictionary<int, (long, long)> _byteOffsetsPerFile;

        public ResumableMultiNDJsonFileIterator(IEnumerable<string> path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            _paths = [.. path];
            _currentFileIndex = 0;
            _byteOffsetsPerFile = new ConcurrentDictionary<int, (long, long)>
            {
                [0] = (0, -1) // their offset and their lenghts if initialized. If not keep as -1 as an indicator
            };
        }

        public NDObject Next()
        {
            var fileStream = File.OpenRead(_paths[_currentFileIndex]);
            if (fileStream.Position >= fileStream.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(Next));
            }

            if (!fileStream.CanRead)
            {
                throw new InvalidOperationException(nameof(fileStream));
            }

            using var file = new StreamReader(fileStream);
            var ndObjectSerialized = file.ReadLine();
            _currentByteOffset = fileStream.Position;
            return JsonSerializer.Deserialize<NDObject>(ndObjectSerialized, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
        }

        public MultiFileState GetState()
        {
            return new FileState { ByteOffset = _currentByteOffset };
        }

        public void SetState(MultiFileState newState)
        {
            if (newState is null)
            {
                throw new ArgumentNullException("Null state");
            }
            if (newState.ByteOffset < 0 || newState.ByteOffset >= _fileLength)
            {
                throw new ArgumentOutOfRangeException("Invalid State assignment");
            }

            _currentByteOffset = newState.ByteOffset;
        }
    }
*/
    public sealed class MultiFileState : BaseState {  public Dictionary<int, FileState> ByteOffsetPerFileIndex { get; set; } }
    public sealed class FileState : BaseState {  public long ByteOffset { get; set; } public long LastTimeWriteEpochUTC { get; set; } }
    public sealed class ListState : BaseState { public int Pointer { get; set; } }
    public abstract class BaseState { };

    public sealed class ResumableIteratorTests
    {
        public void ResumableListIterator_VariousScenarios_Succeeds()
        {
            var iterator = new ResumableListIterator(new List<int> { 1, 2, 3, 4, 5 });
            Xunit.Assert.Equal(1, iterator.Next());
            Xunit.Assert.Equal(2, iterator.Next());
            var state = iterator.GetState();
            Xunit.Assert.Equal(2, state.Pointer);
            Xunit.Assert.Equal(3, iterator.Next());
            iterator.SetState(state);
            Xunit.Assert.Equal(3, iterator.Next());
            Xunit.Assert.Equal(4, iterator.Next());
            Xunit.Assert.Equal(5, iterator.Next());
            Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => iterator.Next());
            Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => iterator.SetState(new ListState { Pointer = 10 }));
        }

        public void ResumableJsonFileIterator_VariousScenarios_Succeeds()
        {
            var iterator = new ResumableNDJsonFileIterator<NDJsonOb>("C:\\Users\\apeppas\\Repositories\\Personal\\DeepDive\\OpenAIPrep\\Data\\NDJsonDummyObjectSet.txt");
            Xunit.Assert.Equal(1, iterator.Next().Key);
            Xunit.Assert.Equal(2, iterator.Next().Key);
            var state = iterator.GetState();
            Xunit.Assert.Equal(3, iterator.Next().Key);
            iterator.SetState(state);
            Xunit.Assert.Equal(3, iterator.Next().Key);
            Xunit.Assert.Equal(4, iterator.Next().Key);
            Xunit.Assert.Throws<InvalidOperationException>(() => iterator.Next());
            Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => iterator.SetState(new FileState { ByteOffset = -1502150125021501 }));
        }

        public record NDJsonOb { [JsonPropertyName("key")]public int Key { get; set; } }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility