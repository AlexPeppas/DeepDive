using System;
using System.Collections.Generic;
using System.Text;

using DeepDiveTechnicals.OpenAIPrep;

namespace DeepDiveTechnicals.Tests.OpenAIPrep
{
    public sealed class FloodFillTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Image2D_FloodFill_VariousScenarios_Succeeds(bool useRecursion)
        {
            var maze = new FloodFill(FloodFill.GenerateRandomImage(3,3));
            var newColor = 0;

            var peek = maze.PeekCurrentImage;
            if (peek[1,1] == newColor)
            {
                newColor = 1;
            }

            var painted = maze.Fill(1, 1, newColor, useRecursion);
        }
    }
}
