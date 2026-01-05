using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace DeepDiveTechnicals.OpenAIPrep
{
    public static class WellKnownNodes
    {
        public const string CurrentRoot = "MASTER";
    }

    public sealed class DistributedNodeCounting
    {
        public async Task<ImmutableHashSet<string>> StartCount(string requestId, NodeRuntime root)
        {
            return await root.OnMessageReceived(requestId, new Message { NodeOriginId = WellKnownNodes.CurrentRoot });
        }
    }

    public sealed class NodeRuntime
    {
        private readonly LocalLedger _ledger;

        public NodeRuntime()
        {
            _ledger = new LocalLedger();
        }

        public string NodeId { get; set; }

        public List<NodeRuntime> Neighbors { get; set; }

        public async Task<ImmutableHashSet<string>> Send(string requestId, NodeRuntime toNode)
        {
            // just pass through
            return await toNode.OnMessageReceived(requestId, new Message { NodeOriginId = NodeId });
        }

        public async Task<ImmutableHashSet<string>> OnMessageReceived(string requestId, IMessage payload)
        {
            if (_ledger.IsCycle(requestId, NodeId))
            {
                throw new CycleDetectionException(payload.NodeOriginId, NodeId);
            }
            else if (_ledger.IsVisited(requestId,NodeId))
            {
                return _ledger.Peek(requestId); // memoization cached nodes -- Idempotent behavior
            }

            _ledger.AddToCycleDetector(requestId, NodeId, LocalLedger.State.Visiting);

            ImmutableHashSet<string> nodesVisited;
            foreach(var neighbor in Neighbors)
            {
                if (_ledger.AlreadyVisited(requestId, neighbor.NodeId))
                {
                    // already visited this node, may be still pending, do not revisit
                    continue;
                }

                nodesVisited = await Send(requestId, neighbor);
                _ledger.AddVisitedNode(requestId, neighbor.NodeId);
                _ledger.AddVisitedNodes(requestId, nodesVisited.ToHashSet());
            }

            _ledger.AddToCycleDetector(requestId, NodeId, LocalLedger.State.Visited);
            return _ledger.Peek(requestId);
        }
    }

    public interface IMessage { string NodeOriginId { get; } }

    public sealed class Message : IMessage
    {
        public string NodeOriginId { get; set; }
    }

    public sealed class CycleDetectionException : Exception
    {
        private static readonly string ErrorMessage = $"Cycle detected at node {0} from node {1}";
        public CycleDetectionException(string sourceNode, string destinationNode)
            : base(message: string.Format(ErrorMessage, destinationNode, sourceNode))
        {
        }
    }

    internal sealed record LocalLedger
    {
        private readonly SemaphoreSlim _trxLock = new(1, 1);

        internal void AddVisitedNode(string requestId, string nodeId)
        {
            var lockAcquired = false;
            try
            {
                if (_trxLock.Wait(TimeSpan.FromSeconds(2)))
                {
                    lockAcquired = true;
                    if (KnownNodesVisisted.TryGetValue(requestId, out var nodes))
                    {
                        nodes.TryAdd(nodeId, byte.MinValue);
                    }
                    else
                    {
                        KnownNodesVisisted.TryAdd(requestId, new ConcurrentDictionary<string, byte>{ [nodeId] = byte.MinValue });
                    }
                }
            }
            finally
            {
                if (lockAcquired)
                {
                    _trxLock.Release(1);
                }
            }
        }

        internal void AddVisitedNodes(string requestId, HashSet<string> nodeIds)
        {
            var lockAcquired = false;
            try
            {
                if (_trxLock.Wait(TimeSpan.FromSeconds(2)))
                {
                    lockAcquired = true;
                    if (KnownNodesVisisted.TryGetValue(requestId, out var nodes))
                    {
                        foreach (var node in nodeIds)
                        {
                            nodes.TryAdd(node, byte.MinValue);
                        }
                    }
                    else
                    {
                        var newNodes = new ConcurrentDictionary<string, byte>();
                        foreach (var node in nodeIds)
                        {
                            newNodes.TryAdd(node, byte.MinValue);
                        }
                        KnownNodesVisisted.TryAdd(requestId, newNodes);
                    }
                }
            }
            finally
            {
                if (lockAcquired)
                {
                    _trxLock.Release(1);
                }
            }
        }

        internal bool AlreadyVisited(string requestId, string nodeId)
        {
            if (KnownNodesVisisted.TryGetValue(requestId, out var requestState))
            {
                return requestState.TryGetValue(nodeId, out _);
            }
            return false;
        }

        internal ImmutableHashSet<string> Peek(string requestId)
        {
            if (KnownNodesVisisted.TryGetValue(requestId, out var set))
            {
                return set.Keys.ToImmutableHashSet();
            }

            return [];
        }

        private ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> KnownNodesVisisted { get; set; } = new();

        internal bool IsCycle(string requestId, string key)
        {
            if (CycleDetection.TryGetValue(requestId, out var requestState))
            {
                if (requestState.TryGetValue(key, out var stateEnum))
                {
                    return stateEnum == State.Visiting;
                }
            }
            return false;
        }

        internal bool IsVisited(string requestId, string key)
        {
            if (CycleDetection.TryGetValue(requestId, out var requestState))
            {
                if (requestState.TryGetValue(key, out var stateEnum))
                {
                    return stateEnum == State.Visited;
                }
            }
            return false;
        }

        internal void AddToCycleDetector(string requestId, string key, State state)
        {
            if (CycleDetection.TryGetValue(requestId, out var cycle))
            {
                if (cycle.ContainsKey(key))
                {
                    cycle[key] = state;
                }
                else
                {
                    cycle.TryAdd(key, state);
                }
            }
            else
            {
                CycleDetection.TryAdd(requestId, new ConcurrentDictionary<string, State> { [key] = state });
            }
        }

        private ConcurrentDictionary<string, ConcurrentDictionary<string, State>> CycleDetection { get; set; } = new();

        internal enum State { Visiting, Visited };
    }
}
