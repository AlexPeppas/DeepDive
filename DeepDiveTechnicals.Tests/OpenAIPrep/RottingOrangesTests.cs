using FluentAssertions;

using DeepDiveTechnicals.OpenAIPrep;

namespace DeepDiveTechnicals.Tests.OpenAIPrep
{
    public sealed class RottingOrangesTests
    {
        [Fact]
        public void RottenMaze_CalculationSucceds()
        {
            var maze = new int[3,4];
            maze[0, 0] = 0;
            maze[0, 1] = 0;
            maze[0, 2] = 1;
            maze[0, 3] = 2;

            maze[1, 0] = 2;
            maze[1, 1] = 1;
            maze[1, 2] = 0;
            maze[1, 3] = 0;

            maze[2, 0] = 0;
            maze[2, 1] = 1;
            maze[2, 2] = 0;
            maze[2, 3] = 2;

            var rottingOranges = new RottingOranges(maze);
            var rottingTime = rottingOranges.CalculateTimeUntilEveryOrangeRottens();
            rottingTime.Should().Be(2);
        }

        [Fact]
        public void ImpossibleMaze_ReturnsMinus1()
        {
            var maze = new int[3, 4];
            maze[0, 0] = 0;
            maze[0, 1] = 0;
            maze[0, 2] = 1;
            maze[0, 3] = 2;

            maze[1, 0] = 2;
            maze[1, 1] = 1;
            maze[1, 2] = 0;
            maze[1, 3] = 0;

            maze[2, 0] = 0;
            maze[2, 1] = 1;
            maze[2, 2] = 0;
            maze[2, 3] = 1;

            var rottingOranges = new RottingOranges(maze);
            var rottingTime = rottingOranges.CalculateTimeUntilEveryOrangeRottens();
            rottingTime.Should().Be(-1);
        }

        [Fact]
        public void NoFreshOrangeInitially_Returns0()
        {
            var maze = new int[3, 4];
            maze[0, 0] = 0;
            maze[0, 1] = 0;
            maze[0, 2] = 2;
            maze[0, 3] = 2;

            maze[1, 0] = 2;
            maze[1, 1] = 2;
            maze[1, 2] = 0;
            maze[1, 3] = 0;

            maze[2, 0] = 0;
            maze[2, 1] = 2;
            maze[2, 2] = 0;
            maze[2, 3] = 2;

            var rottingOranges = new RottingOranges(maze);
            var rottingTime = rottingOranges.CalculateTimeUntilEveryOrangeRottens();
            rottingTime.Should().Be(0);
        }

        [Fact]
        public void EdgeCase_ParallelRotting_CalculatesSuccessfully()
        {
            var maze = new int[2, 2];
            maze[0, 0] = 2;
            maze[0, 1] = 1;

            maze[1, 0] = 1;
            maze[1, 1] = 1;

            var rottingOranges = new RottingOranges(maze);
            var rottingTime = rottingOranges.CalculateTimeUntilEveryOrangeRottens();
            rottingTime.Should().Be(2);
        }
    }
}
