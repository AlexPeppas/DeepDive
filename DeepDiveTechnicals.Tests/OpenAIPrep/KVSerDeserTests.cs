using System;
using System.Collections.Generic;
using System.Text;

using DeepDiveTechnicals.OpenAIPrep;

using FluentAssertions;

using Moq;
using Xunit;

namespace DeepDiveTechnicals.Tests.OpenAIPrep
{
    public sealed class KVSerDeserTests
    {
        [Fact]
        public void KVSerDeser_Serialize_Deserialize_MultipleVariants()
        {
            var source = new KVSerDeser();

            source.Set("key1", "value1");
            source.Set("key2", null);
            source.Set("3", "3");
            source.Set("dom", "-4");
            source.Set("obj", "{\"key\":\"this_is_a_value\"}");

            var serialized = source.Serialize();
            var deserialized = source.Deserialize(new MemoryStream(serialized));

            new KVSerDeser(seed:deserialized).Equals(source).Should().BeTrue();
        }
    }
}
