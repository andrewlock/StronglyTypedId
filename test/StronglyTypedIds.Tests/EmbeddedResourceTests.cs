using Xunit;

namespace StronglyTypedIds.Tests
{
    public class EmbeddedResourceTests
    {
        public static TheoryData<string> EmbeddedResources { get; } = new()
        {
            "StronglyTypedIdAttribute",
            "StronglyTypedIdDefaultsAttribute",
            "StronglyTypedIdBackingType",
            "StronglyTypedIdConverter",
            "StronglyTypedIdImplementations",
        };

        [Theory]
        [MemberData(nameof(EmbeddedResources))]
        public void EmittedResourceIsSameAsCompiledResource(string resource)
        {
            var emittedByGenerator = GetEmittedResource(resource);
            var compiledInGenerator = GetGeneratorCompiledResource(resource);

            Assert.Equal(emittedByGenerator, compiledInGenerator);
        }

        [Theory]
        [MemberData(nameof(EmbeddedResources))]
        public void EmittedResourceIsSameAsAttributeDllResource(string resource)
        {
            var emittedByGenerator = GetEmittedResource(resource);
            var compiledInAttributesDll = GetAttributesCompiledResource(resource);

            Assert.Equal(emittedByGenerator, compiledInAttributesDll);
        }

        static string GetGeneratorCompiledResource(string filename)
        {
            return TestHelpers.LoadEmbeddedResource($"StronglyTypedIds.Tests.Sources.Generator.{filename}.cs")
                .Replace("namespace StronglyTypedIds.Sources", "namespace StronglyTypedIds")
                .Replace("public sealed class ", "internal sealed class ")
                .Replace("public enum ", "internal enum ");
        }

        static string GetAttributesCompiledResource(string filename)
        {
            return TestHelpers.LoadEmbeddedResource($"StronglyTypedIds.Tests.Sources.Attributes.{filename}.cs")
                .Replace("public sealed class ", "internal sealed class ")
                .Replace("public enum ", "internal enum ")
                .Replace(@"    [System.Diagnostics.Conditional(""STRONGLY_TYPED_ID_USAGES"")]
", string.Empty);
        }

        static string GetEmittedResource(string resource)
            => EmbeddedSources.LoadEmbeddedResource($"StronglyTypedIds.Templates.Sources.{resource}.cs")
                .Replace(@"    [System.Diagnostics.Conditional(""STRONGLY_TYPED_ID_USAGES"")]
", string.Empty)
                .Replace(@"#if !STRONGLY_TYPED_ID_EXCLUDE_ATTRIBUTES
", string.Empty)
                .Replace(@"
#endif // STRONGLY_TYPED_ID_EXCLUDE_ATTRIBUTES", string.Empty);
    }
}