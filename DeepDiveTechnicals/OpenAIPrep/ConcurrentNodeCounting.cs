using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace DeepDiveTechnicals.OpenAIPrep
{
    public sealed class ConcurrentNodeCounting
    {
        public sealed record Node(string Id)
        {
            public List<Node> Neighbors { get; set; } = new();
        }

        private readonly ConcurrentDictionary<string, Lazy<Task<ImmutableHashSet<Node>>>> _inFlight = new();

        public Task<ImmutableHashSet<Node>> CountNodeAsync(Node node, ImmutableHashSet<string> path)
        {
            if (path.Contains(node.Id))
            {
                throw new CycleDetectionException(path.Last(), node.Id);
            }

            if (_inFlight.TryGetValue(node.Id ,out var lazy))
            {
                return lazy.Value; // already running
            }

            lazy = new Lazy<Task<ImmutableHashSet<Node>>>(() => ComputeAsync(node, path.Add(node.Id)), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

            var winner = _inFlight.GetOrAdd(node.Id, lazy);

            return winner.Value;
        }

        public async Task<ImmutableHashSet<Node>> ComputeAsync(Node node, ImmutableHashSet<string> path)
        {
            if (node.Id == "C") await Task.Delay(100);

            var childTasks = node.Neighbors.Select(node => CountNodeAsync(node, path)).ToArray();
            var results = await Task.WhenAll(childTasks);

            var pathBuilder = ImmutableHashSet.CreateBuilder<Node>();
            pathBuilder.Add(node);

            foreach (var set in results) pathBuilder.UnionWith(set);

            return pathBuilder.ToImmutable();
        }
    }
}
