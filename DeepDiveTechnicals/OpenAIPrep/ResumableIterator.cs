#pragma warning disable CA1416 // Validate platform compatibility
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace DeepDiveTechnicals.OpenAIPrep
{
    public interface IResumableIterator<TState, TValue> where TState: BaseState, new()
    {
        TValue Next();

        TState GetState();

        void SetState(TState newState);
    }

    #region Resumable List Iterator
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
    #endregion

    #region Resumable File Iterator
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
            if (_currentByteOffset >= fileStream.Length)
            {
                throw new EOFException();
            }
            if (!fileStream.CanSeek)
            {
                throw new InvalidOperationException(nameof(fileStream.CanSeek));
            }

            fileStream.Lock(lockOffset, lockLength);
            
            try 
            {
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
                    throw new EOFException();
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
    #endregion

    #region Resumable MultiFile Iterator
    public sealed class ResumableMultiNDJsonFileIterator<NDObject> : IResumableIterator<MultiFileState, NDObject>
    {
        private int _currentFileIndex;
        private readonly IList<string> _paths;
        private readonly IList<ResumableNDJsonFileIterator<NDObject>> _fileIterators;
        private MultiFileState _state;
        private readonly SemaphoreSlim _criticialLock = new SemaphoreSlim(1, 1);

        public ResumableMultiNDJsonFileIterator(IEnumerable<string> path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            _paths = [.. path];
            _currentFileIndex = 0;
            _state = new MultiFileState { StateByFileId = [] };
            _fileIterators = [];
        }

        public async Task<ResumableMultiNDJsonFileIterator<NDObject>> InitAsync()
        {
            var acquired = false;
            try 
            {
                if (await _criticialLock.WaitAsync(TimeSpan.FromSeconds(5)))
                {
                    acquired = true;
                    var fileIndex = 0;
                    var stateBuilder = ImmutableDictionary.CreateBuilder<int, FileState>();
                    foreach (var path in _paths)
                    {
                        var lastWrite = File.GetLastWriteTimeUtc(path);
                        stateBuilder.Add(fileIndex++, new FileState { ByteOffset = 0, LastTimeWriteEpochUTC = lastWrite.Ticks });
                        _fileIterators.Add(new ResumableNDJsonFileIterator<NDObject>(path));
                    }

                    _state.StateByFileId = stateBuilder.ToImmutable();
                }
            }
            finally 
            {
                if (acquired)
                {
                    _criticialLock.Release(1);
                }
                else
                {
                    throw new InvalidOperationException("Failed to INIT");
                }
            }

            return this;
        }

        public NDObject Next()
        {
            if (_currentFileIndex >= _paths.Count)
            {
                throw new InvalidOperationException($"{nameof(Next)}: Files Drained");
            }

            if (_currentFileIndex < 0)
            {
                throw new InvalidOperationException($"{nameof(Next)}: File index cannot be negative.");
            }

            var currentIterator = _fileIterators[_currentFileIndex];

            try
            {
                var item = currentIterator.Next();
                _state.StateByFileId = _state.StateByFileId.SetItem(_currentFileIndex, currentIterator.GetState());// ensure we update the global internal state
                return item;
            }
            catch (EOFException)
            {
                // current file drained fully, try to move to next
                _currentFileIndex++;

                // all files exhausted. Throw
                if (_currentFileIndex >= _paths.Count)
                {
                    throw new FilesExhaustedException();
                }

                return Next();
            }
            catch
            {
                throw;
            }
        }

        public MultiFileState GetState()
        {
            var acquired = false;
            var frozenState = new MultiFileState();
            try
            {
                if (_criticialLock.Wait(TimeSpan.FromSeconds(5)))
                {
                    acquired = true;
                    var stateBuilder = ImmutableDictionary.CreateBuilder<int, FileState>();
                    foreach (var (index, fileState) in _state.StateByFileId)
                    {
                        stateBuilder.Add(index, fileState.Clone());
                    }

                    frozenState.StateByFileId = stateBuilder.ToImmutable();
                }
            }
            finally
            {
                if (acquired)
                {
                    _criticialLock.Release(1);
                }
                else
                {
                    throw new InvalidOperationException("Failed to GetState");
                }
            }

            return frozenState;
        }

        public void SetState(MultiFileState newState)
        {
            if (newState is null)
            {
                throw new ArgumentNullException("Null state");
            }
            if (newState.StateByFileId.Count != _paths.Count )
            {
                throw new ArgumentOutOfRangeException("Invalid State assignment");
            }

            var acquired = false;
            try
            {
                if (_criticialLock.Wait(TimeSpan.FromSeconds(5)))
                {
                    acquired = true;
                    var index = 0;
                    foreach (var childState in newState.StateByFileId)
                    {
                        // if somehting fails internally, let the corresponding iterator handle it.
                        _fileIterators[index++].SetState(childState.Value);
                    }

                    _state = newState;
                }
            }
            finally
            {
                if (acquired)
                {
                    _criticialLock.Release(1);
                }
                else
                {
                    throw new InvalidOperationException("Failed to GetState");
                }
            }
        }
    }
    #endregion Resumable MultiFile Iterator

    #region Models
    public sealed class MultiFileState : BaseState { public ImmutableDictionary<int, FileState> StateByFileId { get; set; } = []; }
    public sealed class FileState : BaseState {  
        public long ByteOffset { get; init; } public long LastTimeWriteEpochUTC { get; init; } 

        internal FileState Clone()
        {
            return new FileState { ByteOffset = ByteOffset, LastTimeWriteEpochUTC = LastTimeWriteEpochUTC };
        }
    }
    public sealed class ListState : BaseState { public int Pointer { get; set; } }
    public abstract class BaseState { };

    public record NDJsonOb { [JsonPropertyName("key")] public int Key { get; set; } }
    #endregion Models

    #region Exceptions
    internal sealed class FilesExhaustedException : Exception
    {
        private const string ErrorMessage = "Next: Files Exhaused";

        public FilesExhaustedException()
            : base(ErrorMessage)
        {

        }
    }
    internal sealed class EOFException : Exception
    {

        private const string ErrorMessage = "Next: EOF";

        public EOFException()
            : base(ErrorMessage)
        {
            
        }
    }
    #endregion Exceptions

    #region UTs
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

        public async Task MultiFileResumableJsonFileIterator_VariousScenarios_Succeeds()
        {
            var paths = new List<string>
            {
                "C:\\Users\\apeppas\\Repositories\\Personal\\DeepDive\\OpenAIPrep\\Data\\NDJsonDummyObjectSet.txt",
                "C:\\Users\\apeppas\\Repositories\\Personal\\DeepDive\\OpenAIPrep\\Data\\NDJsonDummyObjectSet2.txt",
                "C:\\Users\\apeppas\\Repositories\\Personal\\DeepDive\\OpenAIPrep\\Data\\NDJsonDummyObjectSet3.txt"
            };

            var iterator = new ResumableMultiNDJsonFileIterator<NDJsonOb>(paths);
            iterator = await iterator.InitAsync();
            Xunit.Assert.Equal(1, iterator.Next().Key);
            Xunit.Assert.Equal(2, iterator.Next().Key);
            var state = iterator.GetState();
            Xunit.Assert.Equal(3, iterator.Next().Key);
            iterator.SetState(state);
            Xunit.Assert.Equal(3, iterator.Next().Key);
            Xunit.Assert.Equal(4, iterator.Next().Key);
            Xunit.Assert.Equal(5, iterator.Next().Key);
            Xunit.Assert.Equal(6, iterator.Next().Key);
            Xunit.Assert.Equal(7, iterator.Next().Key);
            Xunit.Assert.Equal(8, iterator.Next().Key);
            Xunit.Assert.Equal(9, iterator.Next().Key);
            Xunit.Assert.Equal(10, iterator.Next().Key);
            Xunit.Assert.Throws<FilesExhaustedException>(() => iterator.Next());
            Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => iterator.SetState(new MultiFileState()));
        }
    }
    #endregion UTs
}
#pragma warning restore CA1416 // Validate platform compatibility