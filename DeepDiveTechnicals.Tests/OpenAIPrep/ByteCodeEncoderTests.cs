using System;
using System.Collections.Generic;
using System.Text;

using DeepDiveTechnicals.OpenAIPrep;

namespace DeepDiveTechnicals.Tests.OpenAIPrep
{
    public sealed class ByteCodeEncoderTests
    {
        [Fact]
        public void EncodeDecode_Byte_NoHeapAlloc_Tests()
        {
            var encoder = new ByteCoderEncoder();

            var encoded = encoder.Encode(new("key1", "loooooong"));
            var decoded = encoder.Decode(new MemoryStream(encoded.GetBuffer()));
            Assert.Equal("key1", decoded.Key);
            Assert.Equal("loooooong", decoded.Value);
        }
    }
}
