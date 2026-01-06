using DeepDiveTechnicals.OpenAIPrep;

using FluentAssertions;

namespace DeepDiveTechnicals.Tests
{
    public class IslandsCountTests
    {
        [Fact]
        public void IslandCount_Succeeds()
        {
            var islandsCount = new WarmUp.IslandsCount();
            /// 0 1 1
            /// 0 0 0
            /// 1 0 0
            /// 
            var maze = new int[4, 4]
            {
                {0,1,1,1 },
                {0,0,0,0 },
                {1,1,0,0 },
                {0,0,1,1 },
            };

            var result = islandsCount.Count(maze);

            result.Should().Be(3);
        }

        [Fact]
        public void IslandCount_FullIsland()
        {
            var islandsCount = new WarmUp.IslandsCount();
            /// 0 1 1
            /// 0 0 0
            /// 1 0 0
            /// 
            var maze = new int[4, 4]
            {
            {1,1,1,1 },
            {1,1,1,1 },
            {1,1,1,1 },
            {1,1,1,1 },
            };

            var result = islandsCount.Count(maze);

            result.Should().Be(1);
        }

        [Fact]
        public void IslandCount_NoIsland()
        {
            var islandsCount = new WarmUp.IslandsCount();
            /// 0 1 1
            /// 0 0 0
            /// 1 0 0
            /// 
            var maze = new int[4, 4]
            {
            {0,0,0,0 },
            {0,0,0,0 },
            {0,0,0,0 },
            {0,0,0,0 },
            };

            var result = islandsCount.Count(maze);

            result.Should().Be(0);
        }
    }
}
