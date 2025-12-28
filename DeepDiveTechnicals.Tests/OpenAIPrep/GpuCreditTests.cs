using DeepDiveTechnicals.OpenAIPrep;

using FluentAssertions;

namespace DeepDiveTechincals.Tests
{
    public sealed class GpuCreditTests
    {
        [Fact]
        public void Credits_VariousScenarios_Succeeds()
        {
            // Arrange
            IGpuCredit ledger = new GpuCredit();
            var now = DateTime.UtcNow;
            // Act
            ledger.Add(now, 3, now + TimeSpan.FromSeconds(30));
            ledger.Add(now + TimeSpan.FromSeconds(1), 5, now + TimeSpan.FromSeconds(10));

            var result1 = ledger.Cost(now, 5);
            var result2 = ledger.Cost(now + TimeSpan.FromSeconds(1), 2);
            Thread.Sleep(10000); // sleep 10 seconds
            var result3 = ledger.Cost(now + TimeSpan.FromSeconds(10), 1);
            var result4 = ledger.Cost(now + TimeSpan.FromSeconds(10), 1);

            result1.Should().BeFalse();
            result2.Should().BeTrue();
            result3.Should().BeTrue();
            result4.Should().BeTrue();
        }
    }
}