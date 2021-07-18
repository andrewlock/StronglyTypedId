using Xunit;

namespace StronglyTypedIds.Tests
{
    public class EmbeddedResourceTests
    {
        [Fact]
        public void StronglyTypedIdAttributeSource_IsSameAsCompiledSource()
        {
            var embeddedInGenerator = EmbeddedSources.StronglyTypedIdAttributeSource;
            var compiledInGenerator = GetCompiledResource("StronglyTypedIdAttribute");

            Assert.Equal(embeddedInGenerator, compiledInGenerator);
        }

        [Fact]
        public void StronglyTypedIdBackingTypeSource_IsSameAsCompiledSource()
        {
            var embeddedInGenerator = EmbeddedSources.StronglyTypedIdBackingTypeSource;
            var compiledInGenerator = GetCompiledResource("StronglyTypedIdBackingType");

            Assert.Equal(embeddedInGenerator, compiledInGenerator);
        }

        [Fact]
        public void StronglyTypedIdJsonConverterSource_IsSameAsCompiledSource()
        {
            var embeddedInGenerator = EmbeddedSources.StronglyTypedIdJsonConverterSource;
            var compiledInGenerator = GetCompiledResource("StronglyTypedIdJsonConverter");

            Assert.Equal(embeddedInGenerator, compiledInGenerator);
        }

        static string GetCompiledResource(string filename)
        {
            return TestHelpers.LoadEmbeddedResource($"StronglyTypedIds.Tests.Sources.{filename}.cs")
                .Replace("namespace StronglyTypedIds.Sources", "namespace StronglyTypedIds");
        }
    }
}